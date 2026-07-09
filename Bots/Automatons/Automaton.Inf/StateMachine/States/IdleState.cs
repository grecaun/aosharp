using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;

namespace AutomatonInf
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            if (AutomatonInf._stateMachine?.CurrentState != null)
                AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();
            else
                AutomatonInf.currerntState = "State Null";
            
            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (!AutomatonInf.Enable) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                    return AutomatonInf.Died;
                case Constants.Inferno:
                    if (!Team.IsInTeam)
                        return AutomatonInf.Reform;

                    else if (!Mission.List.Exists(x => x.DisplayName.Contains("The Purification Ri")))
                        return AutomatonInf.Grab;
                    else
                        return AutomatonInf.Enter;

                case Constants.Mission:
                    if (DynelManager.NPCs.FirstOrDefault(m => m.Name == "Umbral Spectre") != null)
                    {
                        if (DynelManager.NPCs.FirstOrDefault(m => m.Name == "Umbral Spectre") != null)
                        {
                            if (AutomatonInf.state == AutomatonInf.Mission.done)
                                AutomatonInf.state = AutomatonInf.Mission.delete;

                            AutomatonInf.DeleteMission();
                        }
                    }

                    if (AutomatonInf.MissionExist())
                    {
                        if (AutomatonInf._settings["Leech"].AsBool())
                            return AutomatonInf.Leech;

                        else if (DynelManager.NPCs.Any(c => c.Name == "One Who Obeys Precepts"))
                        {
                            if (AutomatonInf._settings["Clear"].AsBool())
                                return AutomatonInf.Clear;

                            else if (DynelManager.LocalPlayer.Identity == AutomatonInf.Leader)
                                return AutomatonInf.StartMission;

                        }
                        else if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                            return AutomatonInf.Defend;

                        else
                            return AutomatonInf.Roam;

                    }
                    else
                    {
                        var corpse = DynelManager.Corpses.FirstOrDefault();

                        if (AutomatonInf._settings["Looting"].AsBool() && corpse != null)
                            return AutomatonInf.Loot;
                        else
                            return AutomatonInf.Exit;
                    }
                    break;
            }

            return null;
        }
        public void Tick()
        {
        }
        public void OnStateExit()
        {
        }
    }
}
