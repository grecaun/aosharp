using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonDB2
{
    public class EnterState : IState
    {
        private static double _time;

        public void OnStateEnter()
        {
            Chat.WriteLine("Entering");
            _time = Time.AONormalTime;

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            var _leader = Team.Members.Where(c => c.Character?.Health > 0 && c.Identity == AutomatonDB2.Leader).FirstOrDefault()?.Character;

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.PWId:
                    if (!Extensions.CanProceed()) return null;
                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._atDoor) < 10)
                    {
                        if (!SMovementController.IsNavigating())
                            SMovementController.SetNavDestination(Constants._warpPos);
                    }

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._warpPos) < 5f)
                    {
                        if (!Team.Members.Any(c => c.Character == null))
                            return AutomatonDB2.PathToBoss;
                    }

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPosition) < 30f)
                        return AutomatonDB2.Fight;
                    break;
                case Constants.DB2Id:
                    return AutomatonDB2.Idle;
            }

            return null;
        }

        public void Tick()
        {
            try
            {
                var entrance = DynelManager.AllDynels.FirstOrDefault(c => c.Name.Contains("Dust Brigade 1 Outpost"));

                if (Game.IsZoning) { return; }

                if (Playfield.ModelIdentity.Instance != Constants.PWId) return;

                if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) < 5)
                {
                    DynelManager.LocalPlayer.Position = Constants._centerofentrance;
                    MovementController.Instance.SetMovement(MovementAction.Update);
                }

                if (Time.AONormalTime > _time + 2f)
                {
                    _time = Time.AONormalTime;

                    MovementController.Instance.SetDestination(Constants._entrance);
                    MovementController.Instance.AppendDestination(Constants._append);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred on line " + AutomatonDB2.GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != AutomatonDB2.previousErrorMessage)
                {
                    Chat.WriteLine(errorMessage);
                    Chat.WriteLine("Stack Trace: " + ex.StackTrace);
                    AutomatonDB2.previousErrorMessage = errorMessage;
                }
            }
        }

        public void OnStateExit()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}