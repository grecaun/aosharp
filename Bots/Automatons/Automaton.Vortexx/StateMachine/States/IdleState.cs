using AOSharp.Core;
using System.Linq;

namespace AutomatonVortexx
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            AutomatonVortexx._stateTimeOut = 0;
        }

        public IState GetNextState()
        {
            if (!AutomatonVortexx._settings["Enable"].AsBool()) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.XanHubId:
                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) < 20f)
                    {
                        if (!Team.IsInTeam) return null;

                        if (!Extensions.CanProceed()) return null;

                        if (DynelManager.LocalPlayer.Identity == AutomatonVortexx.Leader)
                        {
                            if (!Team.Members.Any(t => t.Character == null))
                                return AutomatonVortexx.Enter;
                        }
                        else
                        {
                            if (AutomatonVortexx._settings["Clear"].AsBool()) return null;

                            var _leader = Team.Members.FirstOrDefault(c => c.Character?.Health > 0 && c.Identity == AutomatonVortexx.Leader);

                            if (_leader == null)
                            {
                                if (AutomatonVortexx._stateTimeOut == 0)
                                    AutomatonVortexx._stateTimeOut = Time.AONormalTime + AutomatonVortexx.Rand(0.5f, 3.5f);

                                if (Time.AONormalTime > AutomatonVortexx._stateTimeOut)
                                {
                                    AutomatonVortexx._stateTimeOut = 0;
                                    return AutomatonVortexx.Enter;
                                }

                                return null;
                            }
                        }
                    }
                    else if (Extensions.CanProceed())
                        return AutomatonVortexx.Died;
                    break;
                case Constants.VortexxId:
                    return AutomatonVortexx.Fight;
            }

            return null;
        }

        public void Tick()
        {
        }
        public void OnStateExit()
        {
            AutomatonVortexx._stateTimeOut = 0;
        }
    }
}
