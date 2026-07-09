using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonMitaar
{
    public class FarmingState : IState
    {
        private bool _atCorpse = false;
        private double _timeToLeave;

        public void OnStateEnter()
        {
            if (AutomatonMitaar.NavMeshMovementController.IsNavigating)
                AutomatonMitaar.NavMeshMovementController.Halt();
            _atCorpse = false;
            Chat.WriteLine("Looting state");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }
            if (Playfield.ModelIdentity.Instance == Constants.MitaarId) return null;

            if (!Team.Members.Any(c => c.Character == null) || AutomatonMitaar._settings["Solo"].AsBool())
                return AutomatonMitaar.Reform;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance == Constants.MitaarId)
            {
                var Device = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Strange Alien Device");
                var SinuhCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Technomaster Sinuh");

                if (SinuhCorpse != null)
                {
                    if (!_atCorpse)
                    {
                        if (AtPosition(SinuhCorpse.Position, 2))
                        {
                            Chat.WriteLine("Pause for looting, 30 sec");
                            _timeToLeave = Time.AONormalTime + 30;
                            _atCorpse = true;
                        }
                    }
                    else
                    {
                        if (Time.AONormalTime > _timeToLeave)
                        {
                            HandleDeviceUse(Device);
                        }
                    }
                }
                else
                {
                    HandleDeviceUse(Device);
                }
            }
        }

        public void OnStateExit()
        {
            _atCorpse = false;
        }

        void HandleDeviceUse(Dynel Device)
        {
            if (Game.IsZoning) return;
            if (Device == null) return;

            if (!Extensions.CanProceed()) return;

            if (AtPosition(Constants._strangeAlienDevice, 3))
            {
                if (DynelManager.LocalPlayer.Identity == AutomatonMitaar.Leader)
                    Device.Use();
                else
                {
                    var _leader = Team.Members.FirstOrDefault(c => c.Character?.Health > 0 && c.Identity == AutomatonMitaar.Leader)?.Character;

                    if (_leader == null)
                        Device.Use();
                }
            }
        }

        bool AtPosition(Vector3 position, int distance)
        {
            HandlePathing(position, distance);
            return MovementController.Instance.IsNavigating == false;
        }

        void HandlePathing(Vector3 position, int distance)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(position) > distance)
            {
                if (!MovementController.Instance.IsNavigating)
                    MovementController.Instance.SetDestination(position);
            }
            else
            {
                if (MovementController.Instance.IsNavigating)
                    MovementController.Instance.Halt();
            }
        }
    }
}