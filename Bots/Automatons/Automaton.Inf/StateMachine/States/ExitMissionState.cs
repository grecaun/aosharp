using AOSharp.Core;
using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Linq;
using AOSharp.Core.Movement;

namespace AutomatonInf
{
    public class ExitMissionState : IState
    {
        double delayForCorpse;
        double TimeOut;
        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            Chat.WriteLine("Exit Mission");
            AutomatonInf.missionTimer = 0.0;
            delayForCorpse = Time.AONormalTime + 10;
            TimeOut = Time.AONormalTime + 120;

            if (AutomatonInf._settings["Leech"].AsBool())
                DynelManager.LocalPlayer.Position = Constants.LeechMissionExit;

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

                    if (DynelManager.NPCs.FirstOrDefault(m => m.Name == "Umbral Spectre") != null)
                    {
                        if (AutomatonInf.state == AutomatonInf.Mission.done)
                        {
                            AutomatonInf.state = AutomatonInf.Mission.delete;
                        }

                        AutomatonInf.DeleteMission();
                    }

                    var mob = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && !c.IsPet && !AutomatonInf.NamesToIgnores.Contains(c.Name) && !c.Buffs.Contains(AutomatonInf.BuffsToIgnore));

                    var corpse = DynelManager.Corpses.FirstOrDefault();

                    if (AutomatonInf._settings["Looting"].AsBool() && corpse != null && mob == null)
                        return AutomatonInf.Loot;

                    break;
                case Constants.Inferno:
                    if (DynelManager.LocalPlayer.Velocity > 0)
                        SMovementController.Halt();
                    else if (AutomatonInf._settings["DoubleReward"].AsBool())
                    {
                        if (!AutomatonInf.DoubleReward)
                        {
                            AutomatonInf.DoubleReward = true;
                            return AutomatonInf.Grab;
                        }
                        else
                        {
                            AutomatonInf.DoubleReward = false;
                            return AutomatonInf.Reform;
                        }
                    }
                    else
                        return AutomatonInf.Reform;

                    break;
            }

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning || !Team.IsInTeam) return;
            if (DynelManager.LocalPlayer.Buffs.Contains(new[] { NanoLine.Root, NanoLine.Stun, NanoLine.Snare })) return;

            if (Time.AONormalTime > TimeOut) { Team.Leave(); }

            if (AutomatonInf._settings["DoubleReward"].AsBool() && !AutomatonInf.DoubleReward)
            {
                if (DynelManager.LocalPlayer.Velocity > 0) return;

                if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.MissionExitToInferno) > 2f)
                    SMovementController.SetNavDestination(Constants.MissionExitToInferno);
                else
                    MovementController.Instance.SetMovement(MovementAction.ForwardStart);
            }

            if (AutomatonInf._settings["Stop"].AsBool()) return;

            if (Time.AONormalTime > delayForCorpse)
            {
                if (DynelManager.LocalPlayer.Identity == AutomatonInf.Leader)
                {
                    if (DynelManager.LocalPlayer.Velocity > 0) return;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.MissionExitToInferno) > 2f)
                        SMovementController.SetNavDestination(Constants.MissionExitToInferno);
                    else
                        MovementController.Instance.SetMovement(MovementAction.ForwardStart);
                }
                else
                {
                    var leader = DynelManager.Players.FirstOrDefault(c => c.Health > 0 && c.Identity == AutomatonInf.Leader && c.Identity != DynelManager.LocalPlayer.Identity);

                    if (leader != null)
                    {
                        if (Time.AONormalTime < AutomatonInf.pathDelay) return;
                        if (DynelManager.LocalPlayer.Position.DistanceFrom(leader.Position) < 3f) return;

                        SMovementController.SetNavDestination(leader.Position);
                        AutomatonInf.pathDelay = Time.AONormalTime + .1;
                    }
                    else
                    {
                        if (DynelManager.LocalPlayer.Velocity > 0) return;

                        if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.MissionExitToInferno) > 2f)
                            SMovementController.SetNavDestination(Constants.MissionExitToInferno);
                        else
                            MovementController.Instance.SetMovement(MovementAction.ForwardStart);
                    }
                }
            }
        }
        public void OnStateExit()
        {
            Chat.WriteLine("Exit Mission");
            delayForCorpse = 0.0;
            TimeOut = 0.0;

            AutomatonInf._settings["LastRunTime"] = (float)(Time.AONormalTime - AutomatonInf.StartStamp);

            AutomatonInf.StartStamp = 0.0;
        }
    }
}
