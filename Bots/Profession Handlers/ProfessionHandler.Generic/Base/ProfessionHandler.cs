using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic.Combat
{
    public class ProfessionHandler
    {
        private double perkDelay;
        private double Delay;
        protected Queue<CombatActionQueueItem> _actionQueue = new Queue<CombatActionQueueItem>();
        protected List<ItemRule> _itemRules = new List<ItemRule>();
        protected List<PerkRule> _perkRules = new List<PerkRule>();
        protected List<SpellRule> _spellRules = new List<SpellRule>();

        protected delegate bool ItemConditionProcessor(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget);
        protected delegate bool PerkConditionProcessor(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget);
        protected delegate bool SpellConditionProcessor(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget);

        public static ProfessionHandler Instance { get; private set; }

        private int LastQueue = 0;

        public static void Set(ProfessionHandler ProfessionHandler)
        {
            Instance = ProfessionHandler;
        }

        protected void Update(object sender, float deltaTime)
        {
            if (GenericProfessionHandler.Now < GenericProfessionHandler._lastZonedTime) return;
            if (!DynelManager.LocalPlayer.IsAlive) return;
            if (Game.IsZoning) return;
            if (!GenericProfessionHandler._settings["Enable"].AsBool()) return;

            GenericProfessionHandler.SkillLock.RemoveWhere(x => (int)Time.AONormalTime > x.Item2);

            OnUpdate(deltaTime);
        }

        protected virtual void OnUpdate(float deltaTime)
        {
            var fightingTarget = DynelManager.LocalPlayer.FightingTarget;

            if (fightingTarget != null && fightingTarget.IsPlayer) return;

            foreach (var rule in _itemRules.OrderBy(r => r.Priority))
            {
                if (rule.ItemConditionProcessor == null) continue;
                if (rule.Context == RuleContext.InCombat && fightingTarget == null) continue;
                if (rule.Context == RuleContext.OutOfCombat && fightingTarget != null) continue;
                if (rule.Ids == null) continue;
                if (_actionQueue.Any(q => q.CombatAction is Item i && rule.Ids.Contains(i.Id))) continue;

                var item = Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity))).Where(x => rule.Ids.Contains(x.Id)).OrderByDescending(x => x.QualityLevel).FirstOrDefault(x => x.MeetsSelfUseReqs());

                if (item == null) continue;

                (SimpleChar Target, bool ShouldSetTarget) actionTarget = (null, false);

                if (rule.ItemConditionProcessor.Invoke(item, fightingTarget, ref actionTarget))
                {
                    var itemDelay = item.AttackDelay > 0 ? item.AttackDelay : 5f;
                    _actionQueue.Enqueue(new CombatActionQueueItem(item, actionTarget.Target, actionTarget.ShouldSetTarget, itemDelay));
                    perkDelay = Time.AONormalTime + 0.5;
                }
            }

            if (!_actionQueue.Any(x => x.CombatAction is Item))
            {
                if (Time.AONormalTime > perkDelay)
                {
                    if (PerkAction.List.Count(p => p.IsExecuting || p.IsPending) < GenericProfessionHandler._settings["MAX_CONCURRENT_PERKS"].AsInt32())
                    {
                        foreach (var rule in _perkRules.OrderBy(r => r.Priority))
                        {
                            if (Item.HasPendingUse) break;
                            if (_actionQueue.Any(x => x.CombatAction is Item)) break;
                            if (_actionQueue.Count(x => x.CombatAction is PerkAction) >= GenericProfessionHandler._settings["MAX_CONCURRENT_PERKS"].AsInt32()) break;
                            if (rule.PerkConditionProcessor == null) continue;
                            if (rule.Context == RuleContext.InCombat && fightingTarget == null) continue;
                            if (rule.Context == RuleContext.OutOfCombat && fightingTarget != null) continue;

                            if (!PerkAction.Find(rule.PerkHash, out PerkAction perk)) continue;
                            if (perk.IsPending || perk.IsExecuting || !perk.IsAvailable) continue;
                            if (_actionQueue.Any(x => x.CombatAction is PerkAction action && action == perk)) continue;

                            (SimpleChar Target, bool ShouldSetTarget) actionTarget = (null, false);

                            if (rule.PerkConditionProcessor.Invoke(perk, fightingTarget, ref actionTarget))
                            {
                                var perkDelayValue = perk.AttackDelay > 0  ? perk.AttackDelay : 5f;

                                _actionQueue.Enqueue(new CombatActionQueueItem(perk, actionTarget.Target, actionTarget.ShouldSetTarget, perkDelayValue));
                                perkDelay = Time.AONormalTime + 0.5;
                            }
                        }
                    }
                }
            }

            if (Time.AONormalTime >= Delay)
            {
                if (!Spell.HasPendingCast && DynelManager.LocalPlayer.MovementStatePermitsCasting)
                {
                    foreach (var spellRule in _spellRules.OrderBy(r => r.Priority))
                    {
                        if (spellRule.SpellConditionProcessor == null) continue;
                        if (spellRule.SpellGroup == null) continue;
                        if (spellRule.Context == RuleContext.InCombat && fightingTarget == null) continue;
                        if (spellRule.Context == RuleContext.OutOfCombat && fightingTarget != null) continue;

                        foreach (int spellId in spellRule.SpellGroup)
                        {
                            if (!Spell.Find(spellId, out Spell curSpell)) continue;
                            if (!curSpell.IsReady) continue;
                            if (!curSpell.MeetsSelfUseReqs()) continue;
                            if (Spell.HasPendingCast) break;

                            (SimpleChar Target, bool ShouldSetTarget) actionTarget = (null, false);

                            if (spellRule.SpellConditionProcessor.Invoke(curSpell, fightingTarget, ref actionTarget))
                                curSpell.Cast(actionTarget.Target, actionTarget.ShouldSetTarget);
                        }
                    }
                }

                if (_actionQueue.Count > 0)
                {
                   var dequeueList = new List<CombatActionQueueItem>();

                    _actionQueue = new Queue<CombatActionQueueItem>( _actionQueue.Where(q => q.Timeout == 0 || Time.AONormalTime < q.Timeout));

                    var queAction = _actionQueue.Where(q => !q.Used).FirstOrDefault();

                    if (queAction != null)
                    {
                        switch (queAction.CombatAction)
                        {
                            case Item item:
                                {
                                    if (!Item.HasPendingUse && !PerkAction.List.Any(p => p.IsExecuting))
                                    {
                                        item.Use(queAction.Target, queAction.ShouldSetTarget);

                                        var lockValue = item.UseModifiers.FirstOrDefault(m => m.Function == SpellFunction.LockSkill) ?.Properties.TryGetValue(SpellPropertyOperator.Value, out var v) == true ? v : (int?)null;

                                        if (lockValue.HasValue)
                                            GenericProfessionHandler.SkillLock.Add((item, lockValue.Value + (int)Time.AONormalTime));

                                        queAction.Used = true;
                                        dequeueList.Add(queAction);
                                    }
                                }
                                break;
                            case PerkAction perk:
                                {
                                    if (!_actionQueue.Any(x => x.CombatAction is Item))
                                    {
                                        if (PerkAction.List.Count(p => p.IsExecuting || p.IsPending) < GenericProfessionHandler._settings["MAX_CONCURRENT_PERKS"].AsInt32())
                                        {
                                            if (perk.IsAvailable && !perk.IsExecuting && !perk.IsPending && !Item.HasPendingUse)
                                            {
                                                perk.Use(queAction.Target, queAction.ShouldSetTarget);

                                                queAction.Used = true;
                                                dequeueList.Add(queAction);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    _actionQueue = new Queue<CombatActionQueueItem>(_actionQueue.Where(x => !dequeueList.Contains(x)));
                }

                Delay = Time.AONormalTime + 0.15;
            }
        }

        protected void RegisterItemProcessor(int id, ItemConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            RegisterItemProcessor(new[] { id }, conditionProcessor, priority, context);
        }

        protected void RegisterItemProcessor(IEnumerable<int> ids, ItemConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            var idsArr = ids as int[] ?? ids.ToArray();
            if (_itemRules.Any(r => r.Ids.Length == idsArr.Length && r.Ids.SequenceEqual(idsArr))) return;

            var rule = new ItemRule(idsArr, conditionProcessor, priority, context);

            int idx = -1;
            for (int i = 0; i < _itemRules.Count; i++)
            {
                var r = _itemRules[i];
                if ((int)r.Context > (int)rule.Context || ((int)r.Context == (int)rule.Context && (int)r.Priority > (int)rule.Priority))
                {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0) _itemRules.Insert(idx, rule);
            else _itemRules.Add(rule);
        }

        protected void RegisterPerkProcessor(PerkHash perkHash, PerkConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            if (_perkRules.Any(r => r.PerkHash == perkHash)) return;
            var rule = new PerkRule(perkHash, conditionProcessor, priority, context);
            int idx = -1;
            for (int i = 0; i < _perkRules.Count; i++)
            {
                var r = _perkRules[i];
                if ((int)r.Context > (int)rule.Context || ((int)r.Context == (int)rule.Context && (int)r.Priority > (int)rule.Priority)) { idx = i; break; }
            }
            if (idx >= 0) _perkRules.Insert(idx, rule); else _perkRules.Add(rule);
        }

        protected void RegisterSpellProcessor(Spell spell, SpellConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            RegisterSpellProcessor(new[] { spell.Id }, conditionProcessor, priority, context);
        }

        protected void RegisterSpellProcessor(IEnumerable<Spell> spellGroup, SpellConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            RegisterSpellProcessor(spellGroup.GetIds(), conditionProcessor, priority, context);
        }

        protected void RegisterSpellProcessor(int spellId, SpellConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            RegisterSpellProcessor(new[] { spellId }, conditionProcessor, priority, context);
        }

        protected void RegisterSpellProcessor(int[] spellGroup, SpellConditionProcessor conditionProcessor, CombatActionPriority priority = CombatActionPriority.NonCombat, RuleContext context = RuleContext.Both)
        {
            if (spellGroup.Length == 0) return;
            if (_spellRules.Any(r => r.SpellGroup.Length == spellGroup.Length && r.SpellGroup.SequenceEqual(spellGroup))) return;

            var rule = new SpellRule(spellGroup, conditionProcessor, priority, context);

            int idx = -1;
            for (int i = 0; i < _spellRules.Count; i++)
            {
                var r = _spellRules[i];
                if ((int)r.Context > (int)rule.Context || ((int)r.Context == (int)rule.Context && (int)r.Priority > (int)rule.Priority))
                {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0) _spellRules.Insert(idx, rule);
            else _spellRules.Add(rule);
        }

        protected class CombatActionQueueItem : IEquatable<CombatActionQueueItem>
        {
            public AOSharp.Core.Combat.ICombatAction CombatAction;
            public SimpleChar Target;
            public bool Used = false;
            public bool ShouldSetTarget = false;
            public double Timeout = 0;

            public CombatActionQueueItem(AOSharp.Core.Combat.ICombatAction action, SimpleChar target, bool shouldSetTarget, double timeoutOffset = 0)
            {
                CombatAction = action;
                Target = target;
                ShouldSetTarget = shouldSetTarget;
                Timeout = Time.AONormalTime + timeoutOffset;
            }

            public bool Equals(CombatActionQueueItem other)
            {
                if (other == null)
                    return false;

                if (CombatAction.GetType() != other.CombatAction.GetType())
                    return false;

                switch (CombatAction)
                {
                    case PerkAction perk:
                        return perk == ((PerkAction)other.CombatAction);
                    case Item item:
                        Item item1 = item;
                        Item item2 = (Item)other.CombatAction;
                        return item.Id == ((Item)other.CombatAction).Id && item.HighId == ((Item)other.CombatAction).HighId && item.QualityLevel == ((Item)other.CombatAction).QualityLevel;
                    case Spell spell:
                        return spell == ((Spell)other.CombatAction);
                    default:
                        return false;
                }
            }
        }

        protected enum CombatActionPriority { Ultra = 0, VeryHigh = 10, High = 20, Medium = 30, Low = 40, VeryLow = 50, NonCombat = 60 }

        protected enum RuleContext
        {
            Both = 0,
            InCombat = 1,
            OutOfCombat = 2
        }

        protected readonly struct ItemRule
        {
            public int[] Ids { get; }
            public ItemConditionProcessor ItemConditionProcessor { get; }
            public CombatActionPriority Priority { get; }
            public RuleContext Context { get; }

            public ItemRule(IEnumerable<int> ids, ItemConditionProcessor itemConditionProcessor, CombatActionPriority priority, RuleContext context)
            {
                Ids = ids.ToArray();
                ItemConditionProcessor = itemConditionProcessor;
                Priority = priority;
                Context = context;
            }
        }

        protected readonly struct PerkRule
        {
            public PerkHash PerkHash { get; }
            public PerkConditionProcessor PerkConditionProcessor { get; }
            public CombatActionPriority Priority { get; }
            public RuleContext Context { get; }

            public PerkRule(PerkHash perkHash, PerkConditionProcessor perkConditionProcessor, CombatActionPriority combatActionPriority, RuleContext context)
            {
                PerkHash = perkHash;
                PerkConditionProcessor = perkConditionProcessor;
                Priority = combatActionPriority;
                Context = context;
            }
        }

        protected readonly struct SpellRule
        {
            public int[] SpellGroup { get; }
            public SpellConditionProcessor SpellConditionProcessor { get; }
            public CombatActionPriority Priority { get; }
            public RuleContext Context { get; }

            public SpellRule(int[] spellGroup, SpellConditionProcessor spellConditionProcessor, CombatActionPriority priority, RuleContext context)
            {
                SpellGroup = spellGroup;
                SpellConditionProcessor = spellConditionProcessor;
                Priority = priority;
                Context = context;
            }
        }
    }
}
