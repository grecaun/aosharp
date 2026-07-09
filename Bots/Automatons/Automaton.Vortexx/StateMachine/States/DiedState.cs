using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;

namespace AutomatonVortexx
{
    public class DiedState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("Died");

            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Playfield.ModelIdentity.Instance != Constants.XanHubId) return null;
            if (!Extensions.CanProceed()) return null;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._startPos) < 10.0f)
                return AutomatonVortexx.Idle;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance != Constants.XanHubId) return;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) > 20.0f)
            {
                if (DynelManager.LocalPlayer.HealthPercent > AutomatonVortexx._settings["KitHealthPercentageBox"].AsInt32() && DynelManager.LocalPlayer.NanoPercent > AutomatonVortexx._settings["KitNanoPercentageBox"].AsInt32())
                {
                    if (DynelManager.LocalPlayer.MovementState == MovementState.Sit)
                        MovementController.Instance.SetMovement(MovementAction.LeaveSit);
                    else
                    {
                        if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1)
                        {
                            if (!AutomatonVortexx.NavMeshMovementController.IsNavigating)
                                AutomatonVortexx.NavMeshMovementController.SetNavMeshDestination(Constants._startPos);
                        }
                    }
                }
            }
        }

        public void OnStateExit()
        {
            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();
        }
    }
}