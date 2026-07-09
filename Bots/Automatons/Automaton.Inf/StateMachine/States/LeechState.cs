using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;

namespace AutomatonInf
{
    public class LeechState : IState
    {
        public void OnStateEnter()
        {
            AutomatonInf.currerntState = AutomatonInf._stateMachine.CurrentState.ToString();

            if (AutomatonInf._mainWindow?.IsValid == true)
            {
                if (AutomatonInf._mainWindow.FindView("State", out TextView state))
                    state.Text = AutomatonInf.currerntState;
            }

            AutomatonInf.missionTimer = Time.AONormalTime;

            Chat.WriteLine("Leecher");

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();

            DynelManager.LocalPlayer.Position = Constants.LeechSpot;
            MovementController.Instance.SetMovement(MovementAction.Update);
            MovementController.Instance.SetMovement(MovementAction.JumpStart);
            MovementController.Instance.SetMovement(MovementAction.Update);
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
                    if (!AutomatonInf.MissionExist())
                         return AutomatonInf.Exit;

                    if (!AutomatonInf._settings["Leech"].AsBool())
                         return AutomatonInf.Idle;

                    break;

                case Constants.Inferno:
                     return AutomatonInf.Idle;
            }

            return null;
        }
        public void Tick()
        {
        }
        public void OnStateExit()
        {
            DynelManager.LocalPlayer.Position = new Vector3(160.4f, 2.6f, 103.0f);
        }
    }
}
