using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;
using AOSharp.Pathfinding;

namespace AutomatonInf
{
    public class RoamState : IState
    {
        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();

            AutomatonInf.missionTimer = Time.AONormalTime;
            AutomatonInf.missionTimeOut = Time.AONormalTime;

            Chat.WriteLine("Roaming");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            var mob = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && !c.IsPet && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore));

            var corpse = DynelManager.Corpses.FirstOrDefault();

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                    return AutomatonInf.Died;
                case Constants.Mission:
                    var spectre = DynelManager.NPCs.FirstOrDefault(m => m.Name == "Umbral Spectre");
                    if (spectre != null)
                    {
                        if (AutomatonInf.state == AutomatonInf.Mission.done)
                            AutomatonInf.state = AutomatonInf.Mission.delete;

                        AutomatonInf.DeleteMission();
                    }
                    if (AutomatonInf._settings["Looting"].AsBool() && corpse != null && mob == null)
                        return AutomatonInf.Loot;

                    else if (!AutomatonInf.MissionExist())
                    {
                        Chat.WriteLine("Mission does not exist", ChatColor.Green);
                        return AutomatonInf.Exit;
                    }
                    else if (Time.AONormalTime > AutomatonInf.missionTimeOut + 600)
                        AutomatonInf.DeleteMission();

                    else if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                    {
                        var mission = Mission.List.FirstOrDefault(x => x.DisplayName.Contains("The Purification Ritual"));
                        if (mission == null) { return null; }
                        Chat.WriteLine("Mission timed out, deleting.");
                        mission?.Delete();
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
                        var target = DynelManager.Characters.Where(c => c.Health > 0 && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore) && c.Identity == leader?.FightingTarget.Identity).FirstOrDefault();
                        HandlePathAndAttcking(target);
                    }

                    else
                    {
                        if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }

                        if (DynelManager.LocalPlayer.Position.DistanceFrom((Vector3)leader?.Position) > 2f)
                        {
                            if (Time.AONormalTime < AutomatonInf.pathDelay) return;
                            SMovementController.SetNavDestination((Vector3)leader?.Position);
                            AutomatonInf.pathDelay = Time.AONormalTime + .2;
                        }
                        else if (DynelManager.LocalPlayer.Velocity > 0)
                            SMovementController.Halt();
                    }
                }
                else
                    ReturnToPos();
            }
            else
            {
                var mob = DynelManager.NPCs.Where(c => c.Health > 0 && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.IsPet && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore))
                    .OrderBy(c => c.Position.DistanceFrom(DynelManager.LocalPlayer.Position)).ThenBy(c => c.HealthPercent).FirstOrDefault();

                if (mob != null)
                    HandlePathAndAttcking(mob);

                else if (AutomatonInf.Mobs.Count > 0)
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }
                    if (DynelManager.LocalPlayer.Position.DistanceFrom(AutomatonInf.Mobs.Values.First()) > 10)
                    {
                        if (DynelManager.LocalPlayer.Velocity > 0) { return; }
                        SMovementController.SetNavDestination(AutomatonInf.Mobs.Values.First());
                    }
                    else if (mob == null)
                        AutomatonInf.Mobs.Remove(AutomatonInf.Mobs.Keys.First());
                }
                else
                    ReturnToPos();
            }
        }

        void HandlePathAndAttcking(SimpleChar mob)
        {
            if (mob == null) return;

            if (!Extensions.InAttackRange(mob) || !mob.IsInLineOfSight)
            {
                if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) return;
                if (Time.AONormalTime < AutomatonInf.pathDelay) return;
                SMovementController.SetNavDestination(mob.Position);
                AutomatonInf.pathDelay = Time.AONormalTime + .1;
                return;
            }

            if (DynelManager.LocalPlayer.Velocity > 0)
            {
                SMovementController.Halt();
                return;
            }

            if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttacking && !DynelManager.LocalPlayer.IsAttackPending)
            {
                DynelManager.LocalPlayer.Attack(mob, false);
                AutomatonInf.missionTimeOut = Time.AONormalTime;
                return;
            }
        }

        void ReturnToPos()
        {
            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) { return; }
            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.RoamPos) < 5) { return; }
            if (DynelManager.LocalPlayer.Velocity > 0) return;
            SMovementController.SetNavDestination(Constants.RoamPos);
            return;
        }

        public void OnStateExit()
        {
        }
    }
}
