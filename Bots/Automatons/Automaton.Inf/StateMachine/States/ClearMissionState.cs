using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Linq;

namespace AutomatonInf
{
    public class ClearMissionState : IState
    {
        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            Chat.WriteLine("Clearing mission");

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                    return AutomatonInf.Died;
                case Constants.Mission:
                    if (AutomatonInf.Mobs.Count == 0)
                    {
                        if (DynelManager.LocalPlayer.Identity == AutomatonInf.Leader)
                            return AutomatonInf.StartMission;
                        else
                        {
                            //var Guardian = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Guardian Spirit of Purification"));

                            //if (Guardian != null) { return null; }

                            if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                                return AutomatonInf.Defend;
                            else
                                return AutomatonInf.Roam;
                        }
                    }
                    break;

                case Constants.Inferno:
                    return AutomatonInf.Idle; ;
            }

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning || !Team.IsInTeam) { return; }
            if (Playfield.ModelIdentity.Instance != Constants.Mission) { return; }

            if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader)
            {
                var leader = DynelManager.Players.Where(c => c.Health > 0 && c.IsValid == true && c.Identity == AutomatonInf.Leader).FirstOrDefault();

                if (leader == null) { return; }

                if (leader?.FightingTarget != null)
                {
                    var target = leader.FightingTarget;
                    var targetMob = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && !AutomatonInf.NamesToIgnores.Contains(c.Name) && target.FightingTarget?.Identity == AutomatonInf.Leader);

                    if (targetMob == null) { return; }

                    HandlePathAndAttcking(targetMob);
                }
                else if (DynelManager.LocalPlayer.Position.DistanceFrom((Vector3)leader?.Position) > 2f)
                {
                    if (Time.AONormalTime < AutomatonInf.pathDelay) return;
                    SMovementController.SetNavDestination((Vector3)leader?.Position);
                    AutomatonInf.pathDelay = Time.AONormalTime + .1;
                }
                else if (DynelManager.LocalPlayer.Velocity > 0)
                    SMovementController.Halt();
            }
            else
            {
                var mob = DynelManager.NPCs.Where(c => c.Health > 0 && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.IsPet && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore))
                    .OrderBy(c => c.Position.DistanceFrom(DynelManager.LocalPlayer.Position)).ThenBy(c => c.HealthPercent).FirstOrDefault();

                if (mob != null)
                    HandlePathAndAttcking(mob);

                else if (AutomatonInf.Mobs.Count > 0)
                {
                    if (DynelManager.LocalPlayer.Velocity > 0) { return; }

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(AutomatonInf.Mobs.Values.First()) > 10)
                        SMovementController.SetNavDestination(AutomatonInf.Mobs.Values.First());

                    else if (mob == null)
                        AutomatonInf.Mobs.Remove(AutomatonInf.Mobs.Keys.First());
                }
            }
        }

        void HandlePathAndAttcking(SimpleChar mob)
        {
            if (mob == null) { return; }

            if (!Extensions.InAttackRange(mob) || !mob.IsInLineOfSight)
            {
                if (Time.AONormalTime < AutomatonInf.pathDelay) return;
                SMovementController.SetNavDestination(mob.Position);
                AutomatonInf.pathDelay = Time.AONormalTime + .1;
            }
            else
            {
                if (DynelManager.LocalPlayer.Velocity > 0)
                    SMovementController.Halt();

                else if (DynelManager.LocalPlayer.FightingTarget != null || DynelManager.LocalPlayer.IsAttacking || DynelManager.LocalPlayer.IsAttackPending) { return; }
                DynelManager.LocalPlayer.Attack(mob, false);
            }
        }

        public void OnStateExit()
        {
        }
    }
}
