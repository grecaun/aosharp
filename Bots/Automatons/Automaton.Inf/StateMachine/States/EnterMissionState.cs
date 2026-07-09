using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Linq;

namespace AutomatonInf
{
    public class EnterMissionState : IState
    {
        double waitforlead;
        double KickPlayer;
        private double TeamDelay = 0.0;
        EnterState currentState = new EnterState();

        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            Chat.WriteLine("Entering");
            waitforlead = 0.0;
            KickPlayer = 0.0;
            TeamDelay = 0.0;
            currentState = EnterState.Enter;

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
                case Constants.Inferno:
                    if (!Mission.List.Exists(x => x.DisplayName.Contains("The Purification Ri")))
                        return AutomatonInf.Grab;

                    break;
                case Constants.Mission:

                    if (AutomatonInf._settings["Leech"].AsBool())
                        return AutomatonInf.Leech;

                    if (currentState != EnterState.Done) { return null; }

                    if (Time.AONormalTime < AutomatonInf.teamDelay) { return null; }

                    if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader)
                    {
                        var leader = DynelManager.Players.FirstOrDefault(c => c != null && c.Health > 0 && c.Identity == AutomatonInf.Leader);

                        if (leader == null)
                        {
                            if (AutomatonInf._settings["DoubleReward"].AsBool() && AutomatonInf.DoubleReward)
                            {
                                if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                                    return AutomatonInf.Defend;

                                else
                                    return AutomatonInf.Roam;
                            }
                            else if (waitforlead != 0.0 || AutomatonInf.HasDied)
                            {
                                var Guardian = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Guardian Spirit of Purification"));

                                if (Time.AONormalTime > waitforlead || Guardian != null || AutomatonInf.HasDied)
                                {
                                    if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                                        return AutomatonInf.Defend;
                                    else
                                        return AutomatonInf.Roam;
                                }
                            }
                            else if (waitforlead == 0.0)
                                waitforlead = Time.AONormalTime + 60;
                        }
                        else
                        {
                            if (AutomatonInf._settings["Clear"].AsBool())
                                return AutomatonInf.Clear;
                            else if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                                return AutomatonInf.Defend;
                            else
                                return AutomatonInf.Roam;
                        }
                    }
                    else
                    {
                        if (AutomatonInf._settings["Clear"].AsBool())
                            return AutomatonInf.Clear;

                        else
                            return AutomatonInf.StartMission;
                    }

                    break;
            }

            return null;
        }

        enum EnterState { Enter, Waiting, Removing, Done }

        public void Tick()
        {
            if (Game.IsZoning || !Team.IsInTeam) { return; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.Inferno:
                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.EntrancetoXantemple) > 2)
                    {
                        if (DynelManager.LocalPlayer.Velocity == 0)
                            SMovementController.SetNavDestination(Constants.EntrancetoXantemple);
                    }
                    else if (DynelManager.LocalPlayer.Velocity == 0)
                        MovementController.Instance.SetMovement(MovementAction.ForwardStart);
                    break;
                case Constants.Mission:

                    if (DynelManager.NPCs.FirstOrDefault(m => m.Name == "Umbral Spectre") != null)
                    {
                        if (AutomatonInf.state == AutomatonInf.Mission.done)
                            AutomatonInf.state = AutomatonInf.Mission.delete;

                        AutomatonInf.DeleteMission();
                    }
                    else
                    {
                        switch (currentState)
                        {
                            case EnterState.Enter:
                                MovementController.Instance.Halt();
                                currentState = EnterState.Waiting;
                                break;
                            case EnterState.Waiting:

                                if (AutomatonInf.HasDied ||  (AutomatonInf._settings["DoubleReward"].AsBool() && AutomatonInf.DoubleReward))
                                {
                                    currentState = EnterState.Done;
                                    break;
                                }

                                if (!Team.Members.Any(t => t.Character == null))
                                {
                                    if (TeamDelay == 0.0)
                                        TeamDelay = Time.AONormalTime + 10;
                                    else if (Time.AONormalTime >= TeamDelay)
                                    {
                                        currentState = EnterState.Done;
                                        break;
                                    }
                                }
                                else if (KickPlayer == 0.0)
                                    KickPlayer = Time.AONormalTime + 600;
                                else if (Time.AONormalTime > KickPlayer)
                                    currentState = EnterState.Removing;
                                break;
                            case EnterState.Removing:
                                Chat.WriteLine($"EnterMissionState: Removing ids from TeamList", ChatColor.Green);
                                Team.Kick(Team.Members.FirstOrDefault(p => p.Character == null).Identity);
                                currentState = EnterState.Done;
                                break;
                        }
                    }

                    break;
            }
        }

        public void OnStateExit()
        {
            waitforlead = 0.0;
            KickPlayer = 0.0;
            TeamDelay = 0.0;
            currentState = EnterState.Enter;
            AutomatonInf.StartStamp = Time.AONormalTime;
        }
    }
}
