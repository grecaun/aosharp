using AOSharp.Core;
using System.Linq;
using AOSharp.Common.GameData;
using System.Collections.Generic;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.Messages;
using AOSharp.Core.Inventory;
using System;

namespace AutomatonRoamba
{
    public class MobTargeting
    {
        private List<Identity> _ignoredIdentities;
        private List<int> _tauntItemIds;
        private List<CorpseTarget> _corpses;
        private ConfigEditorConfig _configEditorConfig;
        private PathEditorConfig _pathEditorConfig;

        public MobTargeting()
        {
            _tauntItemIds = new List<int> { 83919, 83920, 253187, 244655, 152029, 151692, 151693, 158045, 158046 };
            _ignoredIdentities = new List<Identity>();
            _corpses = new List<CorpseTarget>();

            Network.N3MessageReceived += OnN3MessageReceived;
            Inventory.ContainerOpened += OnCorpseOpened;
        }

        public void UpdateConfig(ConfigEditorConfig config)
        {
            _configEditorConfig = config;
        }

        public void UpdateConfig(PathEditorConfig config)
        {
            _pathEditorConfig = config;
        }

        public bool TryGetNextTarget(out SimpleChar target, out bool shouldAttack, out Item tauntItem)
        {
            shouldAttack = _configEditorConfig.AttackMobs;
            tauntItem = GetTauntItem();

            var filteredTargets = FilterTargets(GetPossibleTargets(), tauntItem);
            filteredTargets = OrderTargets(filteredTargets);
            target = filteredTargets.FirstOrDefault();

            return target != null;
        }

        public void RemoveCorpse(Identity corpseIdentity)
        {
            var corpseTarget = _corpses.FirstOrDefault(x => x.CorpseIdentity == corpseIdentity);

            if (corpseIdentity != null)
                _corpses.Remove(corpseTarget);
        }

        public bool TryGetNextCorpse(out Corpse corpse)
        {
            corpse = null;

            if (!_configEditorConfig.PathToCorpses)
                return false;

            foreach (var cachedCorpse in _corpses.ToList())
            {
                if (!cachedCorpse.HasExpired())
                    continue;

                _corpses.Remove(cachedCorpse);
            }

            var range = _configEditorConfig.PathToMobs ? _configEditorConfig.PathRange : 5;

            corpse = DynelManager.Corpses
                .Where(x => !x.IsOpen && _corpses.Any(y => y.CorpseIdentity == x.Identity && !y.Looted))
                .Where(x => Vector3.Distance(x.Position, DynelManager.LocalPlayer.Position) <= range)
                .OrderBy(x => Vector3.Distance(x.Position, DynelManager.LocalPlayer.Position))
                .FirstOrDefault();

            return corpse != null;
        }

        private IEnumerable<SimpleChar> GetPossibleTargets()
        {
            var sharedQuery = DynelManager.NPCs
                .Where(x => !x.IsPet && !x.IsPlayer && x.IsAlive && IsInLineOfSight(x) && !_ignoredIdentities.Contains(x.Identity));

            if (_configEditorConfig.AttackPriorityOnly)
                return sharedQuery.Where(x => _pathEditorConfig.Rules.PriorityNames.Contains(x.Name));
            else 
                return sharedQuery.Where(x=> !_pathEditorConfig.Rules.IgnoredNames.Contains(x.Name));
        }

        private List<SimpleChar> FilterTargets(IEnumerable<SimpleChar> possibleTargets, Item tauntItem)
        {
            var finalTargets = new List<SimpleChar>();

            if (!_configEditorConfig.AttackMobs && !_configEditorConfig.UseTauntItem && !_configEditorConfig.PathToMobs)
                return finalTargets;

            if (_configEditorConfig.UseTauntItem && tauntItem != null)
                finalTargets.AddRange(possibleTargets.Where(x => Vector3.Distance(x.Position, DynelManager.LocalPlayer.Position) <= _configEditorConfig.TauntRange));

            if (_configEditorConfig.PathToMobs)
                finalTargets.AddRange(possibleTargets.Where(x => Vector3.Distance(x.Position, DynelManager.LocalPlayer.Position) <= _configEditorConfig.PathRange));

            if (_configEditorConfig.AttackMobs)
                finalTargets.AddRange(possibleTargets.Where(x => IsInWeaponsRange(x)));

            if (_configEditorConfig.DisableIf && _configEditorConfig.DisableIfAttacked)
                finalTargets.AddRange(possibleTargets.Where(x => x.FightingTarget?.Identity == DynelManager.LocalPlayer.Identity));

            finalTargets = finalTargets
              .Distinct(new SimpleCharComparer())
              .Where(x => _pathEditorConfig.SPath.Waypoints.Any(c => Vector3.Distance(c, x.Position) < _configEditorConfig.WanderLimit))
              .ToList();

            if (Team.IsInTeam && !Team.IsLeader)
            {
                var teamMemberTarget = Team.Members?
                    .Where(x => x.Character?.Identity != DynelManager.LocalPlayer.Identity && x.Character?.FightingTarget != null)
                    .OrderByDescending(x => x.Name == _configEditorConfig.FollowTargetName)
                    .ThenByDescending(x => x.Level)
                    .FirstOrDefault()?.Character?.FightingTarget;

                if (teamMemberTarget != null && finalTargets.Any(x => x.Identity == teamMemberTarget.Identity))
                {
                    return new List<SimpleChar> { teamMemberTarget };
                }

            }

            return finalTargets;
        }

        private Item GetTauntItem()
        {
            if (!_configEditorConfig.UseTauntItem)
                return null;

            return Inventory.Items.Where(x => _tauntItemIds.Contains(x.Id)).FirstOrDefault(x => x.MeetsSelfUseReqs());
        }

        private List<SimpleChar> OrderTargets(List<SimpleChar> finalTargets)
        {
            if (finalTargets.Count == 1)
                return finalTargets;

            return finalTargets
                .OrderBy(x =>
                {
                    int index = _pathEditorConfig.Rules.PriorityNames.IndexOf(x.Name);
                    return index != -1 ? index : int.MaxValue;
                })
                .ThenBy(x => x.FightingTarget?.Identity != DynelManager.LocalPlayer.Identity)
                .ThenBy(x => x.HealthPercent)
                .ThenBy(x => Vector3.Distance(x.Position, DynelManager.LocalPlayer.Position))
                .ToList();
        }

        public bool IsInTauntRange(SimpleChar target)
        {
            return target != null && _configEditorConfig.UseTauntItem && IsInLineOfSight(target) && Vector3.Distance(target.Position, DynelManager.LocalPlayer.Position) <= _configEditorConfig.TauntRange;
        }


        public bool IsInPathRange(SimpleChar target)
        {
            return _configEditorConfig.PathToMobs && Vector3.Distance(target.Position, DynelManager.LocalPlayer.Position) <= _configEditorConfig.PathRange;
        }

        private bool IsInLineOfSight(SimpleChar target)
        {
            if (_configEditorConfig.IgnoreLos)
                return true;

            return target.IsInLineOfSight;
        }
        public bool IsInWeaponsRange(SimpleChar target)
        {
            if (target == null)
                return false;

            if (!IsInLineOfSight(target))
                return false;

            switch (_configEditorConfig.AttackRangeMode)
            {
                case AttackRangeMode.Manual:
                    return target.DistanceFrom(DynelManager.LocalPlayer) < _configEditorConfig.ManualModeAttackRange;
                case AttackRangeMode.Automatic:
                    return target.IsInWeaponHitRange(_configEditorConfig.AutomaticModeAttackPadding);
                default:
                    return false;
            }
        }

        public void AddToIgnoreList(Identity identity)
        {
            if (!_ignoredIdentities.Contains(identity))
                _ignoredIdentities.Add(identity);
        }

        private void OnCorpseOpened(object sender, Container container)
        {
            if (container.Identity.Type != IdentityType.Corpse)
                return;

            var corpse = _corpses.FirstOrDefault(x => x.CorpseIdentity == container.Identity);

            if (corpse != null)
                corpse.Looted = true;
        }

        private void OnN3MessageReceived(object sender, N3Message n3Msg)
        {
            if (n3Msg is AttackMessage attackMsg)
            {
                _ignoredIdentities.Remove(attackMsg.Identity);
            }
            else if (n3Msg is CorpseFullUpdateMessage corpseMsg)
            {
                if (_corpses.Any(x => x.CorpseIdentity == corpseMsg.Identity))
                    return;

                var expiretyTimer = corpseMsg.Stats.FirstOrDefault(x => x.Value1 == Stat.TimeExist).Value2 / 100f;

                _corpses.Add(new CorpseTarget(corpseMsg.Identity, expiretyTimer));
            }
            else if (n3Msg is DespawnMessage despawnMsg)
            {
                RemoveCorpse(despawnMsg.Identity);
            }
        }
    }

    public class CorpseTarget
    {
        private DateTime _expiretyTimer;
        public Identity CorpseIdentity;
        public bool Looted;

        public CorpseTarget(Identity identity, float secondsToExpire)
        {
            CorpseIdentity = identity;
            _expiretyTimer = DateTime.Now.AddSeconds(secondsToExpire);
            Looted = false;
        }

        public bool HasExpired() => DateTime.Now > _expiretyTimer;
    }

    public class SimpleCharComparer : IEqualityComparer<SimpleChar>
    {
        public bool Equals(SimpleChar x, SimpleChar y)
        {
            return x.Identity == y.Identity;
        }

        public int GetHashCode(SimpleChar simpleChar)
        {
            return simpleChar.Identity.GetHashCode();
        }
    }
}
