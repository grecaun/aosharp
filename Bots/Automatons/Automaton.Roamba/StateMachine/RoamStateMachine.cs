using AOSharp.Core.Misc;
using AOSharp.Core;
using AOSharp.Pathfinding;
using System;
using AOSharp.Common.GameData;
using System.Linq;
using AOSharp.Core.Inventory;
using System.Collections.Generic;

namespace AutomatonRoamba
{
    public enum State
    {
        Roam,
        Idle,
        Loot,
        PathToMob,
        Fight
    }

    public enum Trigger
    {
        TargetIsHittable,
        TargetOutRange,
        AliveTargetFound,
        LootTargetFound,
        ChangedTarget,
        RetriggerOnEntry,
        TargetNull,
        Recovered,
        TooLowOnStats,
        LootingTarget
    }

    public class RoamContext
    {
        public SimpleChar NextTarget;
        public DateTime LastFightTime;
        public MobTargeting MobTargeting = new MobTargeting();
        public ConfigEditorConfig ConfigEditorConfig;
        public PathEditorConfig PathEditorConfig;

        public void UpdateConfig(ConfigEditorConfig config)
        {
            ConfigEditorConfig = config;
            MobTargeting.UpdateConfig(config);
        }
        public void UpdateConfig(PathEditorConfig config)
        {
            PathEditorConfig = config;
            MobTargeting.UpdateConfig(config);
        }

        public bool IsInCombat()
        {
            if (!Team.IsInTeam)
                return DynelManager.NPCs.Any(x => x.FightingTarget?.Identity == DynelManager.LocalPlayer.Identity);
            else
                return DynelManager.NPCs.Any(x => Team.Members.Where(c => c.Character != null).Select(c => c.Identity).Any(c => c == x.FightingTarget?.Identity));
        }

        public bool HealthOrNanoTooLow()
        {
            if (!ConfigEditorConfig.DisableIf)
                return false;

            if (ConfigEditorConfig.DisableIfNp && DynelManager.LocalPlayer.NanoPercent < ConfigEditorConfig.NanoPercent)
                return true;

            if (ConfigEditorConfig.DisableIfHp && DynelManager.LocalPlayer.HealthPercent < ConfigEditorConfig.HealthPercent)
                return true;

            return false;
        }

        public bool DisableIfAttacked() =>
            ConfigEditorConfig.DisableIf &&
            ConfigEditorConfig.DisableIfAttacked && 
            DynelManager.Characters.Any(x => x.FightingTarget?.Identity == DynelManager.LocalPlayer.Identity);

        public bool DisableIfPlayersNearby()
        {
            if (!ConfigEditorConfig.DisableIf)
                return false;

            if (!ConfigEditorConfig.DisableIfPlayersNearby)
                return false;

            var followTargets = new List<Identity> { DynelManager.LocalPlayer.Identity };

            if (ConfigEditorConfig.FollowTarget)
            {
                var charNames = ConfigEditorConfig.FollowTargetName.Split('\n').Select(x => x.Trim());
                followTargets.AddRange(DynelManager.Players
                    .Where(x => charNames.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                    .Select(x => x.Identity)
                    .ToList());
            }

            return DynelManager.Players.Select(x => x.Identity).Any(player => !followTargets.Contains(player));
        }
    }

    public class RoamStateMachine : FSM<State, Trigger, RoamContext>
    {
        private AutoResetInterval _fsmTickRate;
        public const int UpdateRate = 10;
        public bool IsRunning = false;

        public RoamStateMachine(bool enabled = false) : base(State.Roam, new RoamContext())
        {
            IsRunning = enabled;
            _fsmTickRate = new AutoResetInterval(1000 / UpdateRate);
            Game.OnUpdate += OnUpdate;
            SMovementController.DestinationReached += OnDestinationReached;
            Inventory.ContainerOpened += OnCorpseOpened;
        }

        public void SetStatus(bool enabled)
        {
            IsRunning = enabled;

            if (enabled)
                return;

            if (DynelManager.LocalPlayer.IsAttacking)
                DynelManager.LocalPlayer.StopAttack(false);

            Fire(Trigger.TargetNull);
        }

        private void OnDestinationReached(Vector3 vector)
        {
            if (!IsRunning)
                return;

            var sPath = Context.PathEditorConfig.SPath;

            if (sPath.Waypoints.Count == 0)
                return;

            var startPos = sPath.Waypoints.FirstOrDefault();
            var endPos = sPath.Waypoints.LastOrDefault();

            if (Vector3.Distance(startPos, vector) < 0.05f || Vector3.Distance(endPos, vector) < 0.05f)
            {
                if (!sPath.IsLooping)
                    sPath.Reverse();

                AutomatonRoamba.SetPath(sPath, Context.ConfigEditorConfig);
                return;
            }
        }

        private void OnCorpseOpened(object sender, Container container)
        {
            if (!IsRunning)
                return;

            if (container.Identity.Type != IdentityType.Corpse)
                return;

            Fire(Trigger.TargetNull);
        }

        private void OnUpdate(object sender, float e)
        {
            if (!_fsmTickRate.Elapsed)
                return;

            if (!IsRunning)
                return;

            if (Playfield.ModelIdentity.Instance != Context.PathEditorConfig.SPath?.PlayfieldId)
            {
                IsRunning = false;
                return;
            }
            Tick();
        }

        protected override void ConfigureStateMachine()
        {
            OnTransitioned(OnTransitioned);

            AddState(State.Roam, typeof(RoamState))
                .Permit(Trigger.AliveTargetFound, State.PathToMob)
                .Permit(Trigger.LootTargetFound, State.Loot)
                .Permit(Trigger.TooLowOnStats, State.Idle)
                .PermitReentry(Trigger.TargetNull);

            AddState(State.Idle, typeof(IdleState))
                .Permit(Trigger.Recovered, State.Roam)
                .PermitReentry(Trigger.TargetNull);

            AddState(State.Loot, typeof(LootState))
                .Permit(Trigger.TargetNull, State.Roam)
                .PermitReentry(Trigger.LootingTarget);

            AddState(State.PathToMob, typeof(PathToMobState))
                .Permit(Trigger.TargetNull, State.Roam)
                .Permit(Trigger.TargetIsHittable, State.Fight)
                .PermitReentry(Trigger.AliveTargetFound)
                .PermitReentry(Trigger.ChangedTarget);

            AddState(State.Fight, typeof(FightState))
                .Permit(Trigger.TargetNull, State.Roam)
                .Permit(Trigger.TooLowOnStats, State.Idle)
                .Permit(Trigger.TargetOutRange, State.PathToMob)
                .PermitReentry(Trigger.AliveTargetFound)
                .PermitReentry(Trigger.ChangedTarget);
        }

        private void OnTransitioned(Transition transition)
        {
            AutomatonRoamba.Log.Information($"Transitioned from {transition.Source} to {transition.Destination}. Trigger {transition.Trigger}");
        }
    }
}