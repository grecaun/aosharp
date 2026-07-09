using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;

namespace AutomatonMitaar
{
    public class DiedState : IState
    {
        public void OnStateEnter()
        {
            if (AutomatonMitaar.NavMeshMovementController.IsNavigating)
                AutomatonMitaar.NavMeshMovementController.Halt();

            Chat.WriteLine("Died :(");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (Playfield.ModelIdentity.Instance != Constants.XanHubId) return null;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) < 10.0f)
                return AutomatonMitaar.Idle;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;

            if (!Extensions.CanProceed()) return;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) > 10.0f)
            {
                if (DynelManager.LocalPlayer.HealthPercent > AutomatonMitaar._settings["KitHealthPercentageBox"].AsInt32() && DynelManager.LocalPlayer.NanoPercent > AutomatonMitaar._settings["KitNanoPercentageBox"].AsInt32())
                {
                    if (DynelManager.LocalPlayer.MovementState == MovementState.Sit)
                        MovementController.Instance.SetMovement(MovementAction.LeaveSit);
                    else
                    {
                        if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1)
                        {
                            if (!AutomatonMitaar.NavMeshMovementController.IsNavigating)
                                AutomatonMitaar.NavMeshMovementController.SetNavMeshDestination(Constants._reneterPos);
                        }
                    }
                }
            }
        }

        public void OnStateExit()
        {
        }
    }
}