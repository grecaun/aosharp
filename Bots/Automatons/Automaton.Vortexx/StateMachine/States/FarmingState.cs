using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonVortexx
{
    public class FarmingState : IState
    {
        private bool _atCorpse = false;
        private double _timeToLeave = 0;

        public void OnStateEnter()
        {
            Chat.WriteLine("Looting");
            _timeToLeave = 0;
            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Playfield.ModelIdentity.Instance != Constants.XanHubId) return null;

            if (Team.Members.Any(c => c.Character == null)) return null;

            return AutomatonVortexx.Reform;
        }

        public void Tick()
        {

            var Beacon = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Dust Brigade Beacon");

            var _vortexxCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Ground Chief Vortexx");

            if (!Extensions.CanProceed()) return;

            if (_vortexxCorpse != null)
            {
                if (!_atCorpse)
                {
                    if (AtPosition(_vortexxCorpse.Position, 2))
                    {
                        Chat.WriteLine("Pause for looting, 30 sec");
                        
                        _timeToLeave = Time.AONormalTime + 30;
                        _atCorpse = true;
                    }
                }
                else
                {
                    if (Time.AONormalTime < _timeToLeave) return;
                    HandleDeviceUse(Beacon);
                }
            }
            else
                HandleDeviceUse(Beacon);
        }

        void HandleDeviceUse(Dynel Device)
        {
            if (Device == null) return;

            if (AtPosition(Device.Position, 3))
            {
                if (DynelManager.LocalPlayer.Identity == AutomatonVortexx.Leader)
                    Device.Use();
                else
                {
                    var _leader = DynelManager.Players.FirstOrDefault(c => c.Health > 0 && c.Identity == AutomatonVortexx.Leader);

                    if (_leader == null)
                        Device.Use();
                }
            }
        }

        bool AtPosition(Vector3 position, int distance)
        {
            HandlePathing(position, distance);
            return AutomatonVortexx.NavMeshMovementController.IsNavigating == false;
        }

        void HandlePathing(Vector3 position, int distance)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(position) > distance)
            {
                if (!AutomatonVortexx.NavMeshMovementController.IsNavigating)
                    AutomatonVortexx.NavMeshMovementController.SetNavMeshDestination(position);
            }
            else
            {
                if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                    AutomatonVortexx.NavMeshMovementController.Halt();
            }
        }

        public void OnStateExit()
        {
            _atCorpse = false;
            _timeToLeave = 0;
            if (AutomatonVortexx.NavMeshMovementController.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.Halt();
        }
    }
}
