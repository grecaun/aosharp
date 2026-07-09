using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        private double _mongo;
        private double _singleTaunt;

        public static HashSet<int> LoadedTaunts = new HashSet<int>();

        private void RegisterTauntSpells(int spellID)
        {
            try
            {
                switch ((Profession)DynelManager.LocalPlayer.GetStat(Stat.VisualProfession))
                {
                    case Profession.Adventurer:
                        if (SpellID.SingleTargetTaunts.Contains(spellID))
                        {
                            LoadedTaunts.Add(spellID);
                            
                            if (!TauntWindowController.Taunts.Any(c => c.SettingKey == "TauntSpellSingleTarget"))
                                TauntWindowController.Taunts.Add(new TauntUiConfig { TauntIDs = SpellID.SingleTargetTaunts, SettingKey = "TauntSpellSingleTarget", Label = "Single Target", UiType = UiType.DropDownInputBoxDropDown, Options = TauntWindowController.TauntDefualt });

                            RegisterSpellProcessor(_settings["TauntSpellSingleTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetTaunt(spell, fightingTarget, ref actionTarget, "TauntSpellSingleTarget"), CombatActionPriority.High, RuleContext.InCombat);
                        }

                        if (Spell.GetSpellsForNanoline(NanoLine.AOETauntDOT).Any(s => s.Id == spellID))
                        {
                            LoadedTaunts.Add(spellID);
                            RegisterSpellProcessor(_settings["TauntSpellAOETaunt"].AsInt32(), AOETaunt, CombatActionPriority.VeryHigh, RuleContext.Both);
                        }

                        break;

                    case Profession.Enforcer:
                        if (SpellID.SingleTargetTaunts.Contains(spellID))
                        {
                            LoadedTaunts.Add(spellID);

                            if (!TauntWindowController.Taunts.Any(c => c.SettingKey == "TauntSpellSingleTarget"))
                                TauntWindowController.Taunts.Add(new TauntUiConfig { TauntIDs = SpellID.SingleTargetTaunts, SettingKey = "TauntSpellSingleTarget", Label = "Single Target", UiType = UiType.DropDownInputBoxDropDown, Options = TauntWindowController.TauntDefualt });

                            RegisterSpellProcessor(_settings["TauntSpellSingleTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetTaunt(spell, fightingTarget, ref actionTarget, "TauntSpellSingleTarget"), CombatActionPriority.High, RuleContext.InCombat);
                        }
                        if (Spell.GetSpellsForNanoline(NanoLine.MongoBuff).Any(s => s.Id == spellID))
                        {
                            LoadedTaunts.Add(spellID);
                            RegisterSpellProcessor(_settings["TauntSpellAOETaunt"].AsInt32(), AOETaunt, CombatActionPriority.VeryHigh, RuleContext.Both);
                        }

                        break;

                    case Profession.Soldier:
                        if (SpellID.SingleTargetTaunts.Contains(spellID))
                        {
                            LoadedTaunts.Add(spellID);

                            if (!TauntWindowController.Taunts.Any(c => c.SettingKey == "TauntSpellSingleTarget"))
                                TauntWindowController.Taunts.Add(new TauntUiConfig { TauntIDs = SpellID.SingleTargetTaunts, SettingKey = "TauntSpellSingleTarget", Label = "Single Target", UiType = UiType.DropDownInputBoxDropDown, Options = TauntWindowController.TauntWithAMS });

                            RegisterSpellProcessor(_settings["TauntSpellSingleTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetTaunt(spell, fightingTarget, ref actionTarget, "TauntSpellSingleTarget"), CombatActionPriority.High, RuleContext.InCombat);
                        }
                        if (SpellID.SOLDTimedTauntBuffs.Contains(spellID))
                        {
                            RegisterSpellProcessor(_settings["TauntSpellTimedSingleTarget"].AsInt32(), (Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget) =>
                            SingleTargetTaunt(spell, fightingTarget, ref actionTarget, "TauntSpellTimedSingleTarget"), CombatActionPriority.High, RuleContext.InCombat);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Spells

        private bool SingleTargetTaunt(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[$"{setting}Option"].AsInt32() == 0) return false;
            if (!CanCast(spell)) return false;
            if (Time.AONormalTime < _singleTaunt + _settings[$"{setting}Value"].AsInt32()) return false;
            if (DynelManager.LocalPlayer.HealthPercent <= 30) return false;
            if (debuffAreaTargetsToIgnore.Contains(fightingTarget?.Name)) return false;

            if (SpellID.DragonTargetTaunt.Contains(spell.Id) && !DynelManager.LocalPlayer.Buffs.Any(b => SpellID.DragonMorph.Contains(b.Id))) return false;

            switch (_settings[$"{setting}Option"].AsInt32())
            {
                case 1:
                    if (fightingTarget == null) return false;
                    
                    _singleTaunt = Time.AONormalTime;
                    return true;
                case 2:

                    var mob = DynelManager.NPCs.Where(c => c != null && c.IsAttacking && c.FightingTarget?.Identity != DynelManager.LocalPlayer.Identity
                    && c.IsInLineOfSight && InCastRange(spell, c) && AttackingTeam(c)).OrderBy(c => c.MaxHealth).FirstOrDefault();

                    if (mob == null) return false;
                    actionTarget = (mob, true);
                    _singleTaunt = Time.AONormalTime;
                    return true;
                case 3:
                    if (!DynelManager.LocalPlayer.Buffs.Contains(SpellID.UserReflectShieldSoldier)) return false;
                    if (fightingTarget == null) return false;
                    actionTarget = (fightingTarget, true);
                    _singleTaunt = Time.AONormalTime;
                    return true;
                default:
                    return false;
            }
        }

        private bool AOETaunt(Spell spell, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["AOE"].AsBool()) return false;

            if (!CanCast(spell)) return false;
            if (PVPDistance(21)) return false;

            if (SpellID.DragonAOE.Contains(spell.Id) && !DynelManager.LocalPlayer.Buffs.Any(b => SpellID.DragonMorph.Contains(b.Id))) return false;

            actionTarget.ShouldSetTarget = false;

            if (_settings["OSTMongo"] != null && _settings["OSTMongo"].AsBool())
            {
                Chat.WriteLine("OSTMongo");
                if (!spell.IsReady) return false;
                if (Spell.HasPendingCast) return false;
                return true;
            }

            if (DynelManager.LocalPlayer.HealthPercent <= 30) return false;

            switch (_settings["TauntSpellAOETauntOption"].AsInt32())
            {
                case 0://off
                    return false;
                case 1://target
                    if (Time.AONormalTime < _mongo + _settings["TauntSpellAOETauntValue"].AsInt32()) return false;
                    if (fightingTarget == null) return false;
                    if (fightingTarget.IsPlayer) return false;
                    if (fightingTarget?.Position.DistanceFrom(DynelManager.LocalPlayer.Position) >= 20f || debuffAreaTargetsToIgnore.Contains(fightingTarget?.Name)) return false;

                    _mongo = Time.AONormalTime;
                    return true;
                case 2: // adds
                    if (Time.AONormalTime < _mongo + _settings["TauntSpellAOETauntValue"].AsInt32()) return false;
                    bool hasAdd = DynelManager.NPCs.Any(mob => mob.IsAttacking && mob.Position.DistanceFrom(DynelManager.LocalPlayer.Position) <= 20f && !debuffAreaTargetsToIgnore.Contains(mob.Name) && mob.FightingTarget != null
                    && Team.IsInTeam && Team.Members.Any(tm => tm.Character != null && tm.Identity == mob.FightingTarget.Identity) && mob.FightingTarget.Identity != DynelManager.LocalPlayer.Identity);

                    if (!hasAdd) return false;
                    _mongo = Time.AONormalTime;
                    return true;
                case 3://buff
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.MongoBuff)) return false;
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Perks

        private bool TauntPerk(PerkAction perkAction, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget, string setting)
        {
            if (_settings[$"{setting}Option"].AsInt32() == 0) return false;
            if (!CanUsePerk(perkAction)) return false;
            if (fightingTarget == null) return false;
            if (!TargetInPerkRange(perkAction, fightingTarget)) return false;

            switch (_settings[$"{setting}Option"].AsInt32())
            {
                case 1://Target
                    if (perkAction.Hash == PerkHash.CauseOfAnger)
                    {
                        if (!fightingTarget.Buffs.Contains(251183)) return false;
                    }

                    if (fightingTarget.IsAttacking && fightingTarget.FightingTarget.Identity == DynelManager.LocalPlayer.Identity) return false;

                    actionTarget = (fightingTarget, true);
                    return true;
                case 2://Adds
                    var mob = DynelManager.NPCs.Where(c => c != null && c.IsAttacking && c.FightingTarget?.Identity != DynelManager.LocalPlayer.Identity && c.IsInLineOfSight && AttackingTeam(c)).OrderBy(c => c.MaxHealth).FirstOrDefault();

                    if (mob == null) return false;

                    if (perkAction.Hash == PerkHash.CauseOfAnger)
                    {
                        if (!mob.Buffs.Contains(251183)) return false;
                    }

                    actionTarget = (mob, true);
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Items

        private bool TauntItem(Item item, SimpleChar fightingTarget, ref (SimpleChar Target, bool ShouldSetTarget) actionTarget)
        {
            if (!_settings["TauntItemTauntToolsCheckBox"].AsBool()) return false;
            if (!CanUseItem(item)) return false;
            if (fightingTarget == null) return false;
            if (!TargetInItemRange(item, fightingTarget)) return false;

            actionTarget = (fightingTarget, true);
            return true;
        }

        #endregion
    }
}
