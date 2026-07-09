using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;

namespace AutomatonVortexx
{
    public class EnterState : IState
    {
        double Delay = 0;
        public void OnStateEnter()
        {
            Chat.WriteLine("Entering Vortexx");

            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();

            Delay = 0;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (Playfield.ModelIdentity.Instance != Constants.VortexxId) return null;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPodium) < 5)
            {
                if (Delay != 0 && Time.AONormalTime > Delay)
                    return AutomatonVortexx.Fight;
            }

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.VortexxId:

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPodium) > 2f)
                    {
                        if (!AutomatonVortexx.NavMeshMovementController.IsNavigating)
                            AutomatonVortexx.NavMeshMovementController.SetNavMeshDestination(Constants._centerPodium);
                    }

                    if (Delay == 0)
                    {
                        if (DynelManager.LocalPlayer.Identity == AutomatonVortexx.Leader)
                            Delay = Time.AONormalTime + 2;
                        else
                            Delay = Time.AONormalTime + 2.5;
                    }

                    break;
                case Constants.XanHubId:
                    var pos = DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance);

                    if (pos < 20 && pos > 2)
                    {
                        if (!AutomatonVortexx.NavMeshMovementController.IsNavigating)
                            AutomatonVortexx.NavMeshMovementController.SetDestination(Constants._entrance);
                    }
                    break;
            }
        }

        public void OnStateExit()
        {
            Delay = 0;
        }
    }
}