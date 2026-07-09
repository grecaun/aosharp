using AOSharp.Core;
using System.Linq;
using AOSharp.Pathfinding;
using AOSharp.Core.UI;

namespace AutomatonInf
{
    public class DiedState : IState
    {
        State Currentstate = new State();

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

            Currentstate = State.Rezzing;

            Chat.WriteLine("Died");

            if (DynelManager.LocalPlayer.Identity != AutomatonInf.Leader)
                AutomatonInf.HasDied = true;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (Playfield.ModelIdentity.Instance != Constants.Inferno) { return null; }

            return AutomatonInf.Idle;
        }

        enum State
        {
            Rezzing, Moving, Using, Zoning, Done,
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            var playerPosition = DynelManager.LocalPlayer.Position;
            var statue = DynelManager.AllDynels.FirstOrDefault(s => s.Name.Contains("Garden Exit"));
            var gardens = new int[] { 4696, 4697 };

            switch (Currentstate)
            {
                case State.Rezzing:
                    if (!Extensions.CanProceed()) { return; }
                    Currentstate = State.Moving;
                    break;
                case State.Moving:
                    if (!gardens.Contains(Playfield.ModelIdentity.Instance)) { return; }
                    if (statue == null) { return; }
                    if (playerPosition.DistanceFrom(statue.Position) > 5)
                    {
                        if (DynelManager.LocalPlayer.Velocity > 0) { return; }
                        SMovementController.SetNavDestination(statue.Position);
                    }
                    else
                    {
                        if (DynelManager.LocalPlayer.Velocity > 0) { SMovementController.Halt(); }
                        else
                            Currentstate = State.Using;
                    }
                    break;
                case State.Using:
                    statue?.Use();
                    Currentstate = State.Zoning;
                    break;
                case State.Zoning:
                    if (Playfield.ModelIdentity.Instance != Constants.Pande) { return; }
                    if (DynelManager.LocalPlayer.Velocity > 0) { return; }
                    SMovementController.SetNavDestination(Constants.PandeExitToInferno);
                    Currentstate = State.Done;
                    break;
            }
        }

        public void OnStateExit()
        {
            Currentstate = State.Rezzing;
        }
    }
}
