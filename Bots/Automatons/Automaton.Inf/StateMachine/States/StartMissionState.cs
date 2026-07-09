using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;
using AOSharp.Pathfinding;

namespace AutomatonInf
{
    public class StartMissionState : IState
    {
        private bool _init = false;
        private double _timeToOpenDialog = 0;

        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            AutomatonInf._stateTimeOut = Time.AONormalTime;
            AutomatonInf.missionTimer = Time.AONormalTime;
            _init = false;

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();

            Chat.WriteLine("Starting Mission");
        }

        public IState GetNextState()
        {
            var corpse = DynelManager.Corpses.FirstOrDefault();

            if (Game.IsZoning) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                     return AutomatonInf.Died;
                case Constants.Mission:
                    if (!AutomatonInf.MissionExist())
                    {
                        Chat.WriteLine($"mission does not exist");
                        if (AutomatonInf._settings["Looting"].AsBool() && corpse != null)
                             return AutomatonInf.Loot;

                        else
                             return AutomatonInf.Exit;
                    }
                    else
                    {
                        if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.OneWhoObeysPrecepts) < 8f)
                        {
                            if (!DynelManager.NPCs.Any(c => c.Name == "One Who Obeys Precepts"))
                            {
                                if (AutomatonInf._settings["ModeSelection"].AsInt32() == 0)
                                    return AutomatonInf.Defend;

                                else
                                    return AutomatonInf.Roam;
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
            if (!AutomatonInf.MissionExist()) { return; }
            var OneWhoObeysPrecepts = DynelManager.NPCs.FirstOrDefault(c => c.Name == "One Who Obeys Precepts");

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.OneWhoObeysPrecepts) > 4f)
            {
                if (DynelManager.LocalPlayer.Velocity == 0)
                    SMovementController.SetNavDestination(Constants.OneWhoObeysPrecepts);
            }
            else if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();

            else if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.OneWhoObeysPrecepts) < 8f)
                if (OneWhoObeysPrecepts != null)
                {
                    if (!_init)
                    {
                        _init = true;
                        _timeToOpenDialog = Time.AONormalTime + 1;
                    }
                    else if (_timeToOpenDialog > 0 && Time.AONormalTime >= _timeToOpenDialog)
                    {
                        NpcDialog.Open(OneWhoObeysPrecepts);
                        _timeToOpenDialog = 0;
                        return;
                    }
                }
        }

        public void OnStateExit()
        {
            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();
            _init = false;
            _timeToOpenDialog = 0;
        }
    }
}