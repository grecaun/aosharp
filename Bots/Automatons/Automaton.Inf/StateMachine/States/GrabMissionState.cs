using AOSharp.Core;
using System.Linq;
using AOSharp.Pathfinding;
using AOSharp.Core.UI;

namespace AutomatonInf
{
    public class GrabMissionState : IState
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

            _init = false;

            Chat.WriteLine("Grab mission");

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();

            if (AutomatonInf._settings["Clear"].AsBool()) { AutomatonInf.Mobs.Clear(); }
        }
        public IState GetNextState()
        {
            if (Game.IsZoning || !Team.IsInTeam) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.ClanPandeGarden:
                case Constants.OmniPandeGarden:
                     return AutomatonInf.Died;
                case Constants.Inferno:
                    if (Mission.List.Exists(x => x.DisplayName.Contains("The Purification Ri")))
                    {
                        if (Team.IsInTeam)
                             return AutomatonInf.Idle;
                    }
                    break;
            }

            return null;
        }
        public void Tick()
        {
            if (Game.IsZoning || !Team.IsInTeam) { return; }
            if (DynelManager.LocalPlayer.Velocity > 0) { return; }

            var TheRetainerOfErgo = DynelManager.NPCs.FirstOrDefault(c => c.Name == "The Retainer Of Ergo");
            var randoPos = Constants.TheRetainerOfErgo;
            randoPos.AddRandomness((int)1.34f);

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants.TheRetainerOfErgo) > 4f)
            {
                if (DynelManager.LocalPlayer.Velocity == 0)
                    SMovementController.SetNavDestination(randoPos);
            }

            else if (TheRetainerOfErgo != null)
            {
                if (!_init)
                {
                    _init = true;
                    _timeToOpenDialog = Time.AONormalTime + 1;
                }
                else if (_timeToOpenDialog > 0 && Time.AONormalTime >= _timeToOpenDialog)
                {
                    NpcDialog.Open(TheRetainerOfErgo);
                    _timeToOpenDialog = 0;
                    _init = false;
                }
            }
        }
        public void OnStateExit()
        {
            _init = false;
            _timeToOpenDialog = 0;
        }
    }
}