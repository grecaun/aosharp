using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;
using AOSharp.Pathfinding;

namespace AutomatonInf
{
    public class DefendSpiritState : PositionHolder, IState
    {
        public DefendSpiritState() : base(Constants.DefendPos, 3f, 1) { }

        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            Chat.WriteLine("Defending");

            var randoPos = Constants.DefendPos;
            randoPos.AddRandomness((int)1.34f);

            AutomatonInf.missionTimer = Time.AONormalTime;

            if (DynelManager.LocalPlayer.Velocity == 0)
                SMovementController.SetNavDestination(randoPos);

            AutomatonInf.missionTimeOut = Time.AONormalTime;
        }
        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            var corpse = DynelManager.Corpses.FirstOrDefault();

            var mob = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && !c.IsPet && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore));

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                    return AutomatonInf.Died;
                case Constants.Mission:

                    if (DynelManager.NPCs.FirstOrDefault(m => m.Name == "Umbral Spectre") != null)
                    {
                        if (AutomatonInf.state == AutomatonInf.Mission.done)
                            AutomatonInf.state = AutomatonInf.Mission.delete;

                        AutomatonInf.DeleteMission();
                    }

                    if (AutomatonInf._settings["Looting"].AsBool() && corpse != null && mob == null)
                        return AutomatonInf.Loot;

                    if (!AutomatonInf.MissionExist())
                        return AutomatonInf.Exit;

                    if (AutomatonInf._settings["ModeSelection"].AsInt32() == 1)
                        return AutomatonInf.Roam;

                    if (Time.AONormalTime > AutomatonInf.missionTimeOut + 1200)
                    {
                        foreach (Mission mission in Mission.List)
                        {
                            if (mission.DisplayName.Contains("The Purification Ritual"))
                            {
                                Chat.WriteLine("Mission timed out, deleting.");
                                mission.Delete();
                            }
                        }
                    }
                    break;
                case Constants.Inferno:
                    return AutomatonInf.Idle;
            }
            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader)
            {
                var leader = DynelManager.Players.FirstOrDefault(c => c.Health > 0 && c.Identity == AutomatonInf.Leader);

                if (leader != null)
                {
                    if (AutomatonInf.HasDied)
                        AutomatonInf.HasDied = false;

                    if (leader?.FightingTarget != null)
                    {
                        var targetMob = DynelManager.Characters.Where(c => c.Health > 0 && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore) && c.Identity == leader?.FightingTarget.Identity).FirstOrDefault();

                        if (targetMob != null)
                        {
                            if (targetMob.IsInLineOfSight && Extensions.InAttackRange(targetMob))
                            {
                                if (DynelManager.LocalPlayer.Velocity > 0)
                                    SMovementController.Halt();

                                else if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttacking && !DynelManager.LocalPlayer.IsAttackPending)
                                {
                                    AutomatonInf.missionTimeOut = Time.AONormalTime;
                                    DynelManager.LocalPlayer.Attack(targetMob, false);
                                }
                            }
                            else if (!DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare }))
                            {
                                if (DynelManager.LocalPlayer.Velocity == 0)
                                    SMovementController.SetNavDestination(targetMob.Position);
                            }
                        }
                    }
                    else if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.DefendPos) > 5)
                    {
                        if (!DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare }))
                        {
                            if (DynelManager.LocalPlayer.Velocity == 0)
                                SMovementController.SetNavDestination(Constants.DefendPos, true);
                        }
                    }
                    else
                        HoldPosition();
                }
                else if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.DefendPos) > 5)
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }
                    SMovementController.SetNavDestination(Constants.DefendPos, true);
                }
                else
                    HoldPosition();
            }
            else
            {
                var mob = DynelManager.NPCs.Where(c => c.Health > 0 && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.IsPet && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore))
                    .OrderBy(c => c.Position.DistanceFrom(DynelManager.LocalPlayer.Position)).ThenBy(c => c.HealthPercent).FirstOrDefault();

                if (mob != null)
                {
                    var distanceToTarget = mob.Position.DistanceFrom(Constants.DefendPos);

                    if (Extensions.InAttackRange(mob) && mob.IsInLineOfSight)
                    {
                        if (DynelManager.LocalPlayer.Velocity > 0)
                            SMovementController.Halt();

                        else if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttacking && !DynelManager.LocalPlayer.IsAttackPending)
                        {
                            DynelManager.LocalPlayer.Attack(mob, false);
                            AutomatonInf.missionTimeOut = Time.AONormalTime;
                        }
                    }
                    else if (distanceToTarget <= 60)
                    {
                        if (mob.IsInLineOfSight)
                        {
                            if (distanceToTarget > 20)
                                Shared.TauntingTools.HandleTaunting(mob);

                            else if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }
                            else if (mob.Velocity == 0)
                                SMovementController.SetNavDestination(mob.Position);
                        }
                        else if (distanceToTarget < 20 && mob.Velocity == 0)
                        {
                            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }
                            SMovementController.SetNavDestination(mob.Position);
                        }
                    }
                }
                else if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.DefendPos) > 5)
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }
                    if (DynelManager.LocalPlayer.Velocity == 0)
                        SMovementController.SetNavDestination(Constants.DefendPos, true);
                }
                else
                    HoldPosition();
            }
        }

        public void OnStateExit()
        {

        }
    }

    public class PositionHolder
    {
        private readonly Vector3 _holdPos;
        private readonly float _HoldDist;
        private readonly int _entropy;

        public PositionHolder(Vector3 holdPos, float holdDist, int entropy)
        {
            _holdPos = holdPos;
            _HoldDist = holdDist;
            _entropy = entropy;
        }

        public void HoldPosition()
        {
            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }

            if (!SMovementController.IsNavigating() && !IsNearDefenseSpot())
            {
                var randomHoldPos = _holdPos;
                randomHoldPos.AddRandomness(_entropy);

                SMovementController.SetNavDestination(randomHoldPos);
            }
        }

        private bool IsNearDefenseSpot()
        {
            return DynelManager.LocalPlayer.Position.DistanceFrom(Constants.DefendPos) < _HoldDist;
        }
    }
}
