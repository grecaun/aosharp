using AOSharp.Core;
using System.Linq;

namespace AutomatonMitaar
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            AutomatonMitaar._stateTimeOut = 0;

            if (AutomatonMitaar.NavMeshMovementController.IsNavigating)
                AutomatonMitaar.NavMeshMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (!AutomatonMitaar._settings["Enable"].AsBool()) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.XanHubId:
                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) < 20.0f)
                    {
                        if (!Team.IsInTeam) return null;

                        if (!Extensions.CanProceed()) return null;

                        if (DynelManager.LocalPlayer.Identity == AutomatonMitaar.Leader)
                        {
                            if (!Team.Members.Any(t => t.Character == null) || AutomatonMitaar._settings["Solo"].AsBool())
                                return AutomatonMitaar.Enter;
                        }
                        else
                        {
                            var _leader = Team.Members.FirstOrDefault(c => c.Character?.Health > 0 && c.Identity == AutomatonMitaar.Leader);

                            if (_leader == null)
                            {
                                if (AutomatonMitaar._stateTimeOut == 0)
                                    AutomatonMitaar._stateTimeOut = Time.AONormalTime + AutomatonMitaar.Rand(0.5f, 3.5f);

                                if (Time.AONormalTime > AutomatonMitaar._stateTimeOut)
                                {
                                    AutomatonMitaar._stateTimeOut = 0;
                                    return AutomatonMitaar.Enter;
                                }

                                return null;
                            }
                        }
                    }
                    else
                        return AutomatonMitaar.Died;

                    break;
                case Constants.MitaarId:

                    if (!AutomatonMitaar._settings["Solo"].AsBool())
                        return AutomatonMitaar.Fight;
                    else
                        return AutomatonMitaar.Solo;
            }

            return null;
        }

        public void Tick()
        {
        }

        public void OnStateExit()
        {
            AutomatonMitaar._stateTimeOut = 0;
        }
    }
}
