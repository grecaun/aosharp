using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonMitaar
{
    public class EnterState : IState
    {
        double Delay = 0;
        public void OnStateEnter()
        {
            Chat.WriteLine("Entering Mitaar");

            if (AutomatonMitaar.NavMeshMovementController.IsNavigating)
                AutomatonMitaar.NavMeshMovementController.Halt();

            Delay = 0;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (Playfield.ModelIdentity.Instance != Constants.MitaarId) return null;

            if (!AutomatonMitaar._settings["Solo"].AsBool())
            {
                if (Delay != 0 && Time.AONormalTime> Delay)
                    return AutomatonMitaar.Fight;
            }
            else
               return AutomatonMitaar.Solo;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            switch(Playfield.ModelIdentity.Instance)
            {
                case Constants.MitaarId:
                    if (!Team.Members.Any(t => t.Character == null) && Delay == 0)
                    {
                        if (DynelManager.LocalPlayer.Identity == AutomatonMitaar.Leader)
                            Delay = Time.AONormalTime + 2;
                        else
                            Delay = Time.AONormalTime + 2.5;
                    }
                    break;
                case Constants.XanHubId:
                    var pos = DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance);

                    if (pos < 20 && pos > 2)
                    {
                        if (!AutomatonMitaar.NavMeshMovementController.IsNavigating)
                            AutomatonMitaar.NavMeshMovementController.SetDestination(Constants._entrance);
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