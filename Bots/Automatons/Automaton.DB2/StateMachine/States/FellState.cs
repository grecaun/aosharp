using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Linq;

namespace AutomatonDB2
{
    public class FellState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("You fell, Dumbass!");

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (Playfield.ModelIdentity.Instance != Constants.DB2Id)
                return AutomatonDB2.Idle;

            if (DynelManager.LocalPlayer.IsFalling) return null;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._warpPos) < 10f)
                return AutomatonDB2.PathToBoss;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPosition) < 30)
                return AutomatonDB2.Fight;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance != Constants.DB2Id) return;

            if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.PathtoElevation3))
                SMovementController.SetNavDestination(Constants.forth);
            else if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.PathtoElevation2))
                SMovementController.SetNavDestination(Constants.third);
            else if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.PathtoElevation1))
                SMovementController.SetNavDestination(Constants.second);
            else
                SMovementController.SetNavDestination(Constants.first);
        }

        public void OnStateExit()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}