using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonDB1
{
    public class FarmingState : IState
    {
        private bool _atCorpse = false;
        private double _timeToLeave;

        public void OnStateEnter()
        {
            if (AutomatonDB1.NavMeshMovementController.IsNavigating)
                AutomatonDB1.NavMeshMovementController.Halt();

            _atCorpse = false;
            _timeToLeave = 0;

            Chat.WriteLine("Looting state");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            if (Playfield.ModelIdentity.Instance != Constants.PWId) return null;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) <= 10f)
                return AutomatonDB1.Reform;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;
            if (!Extensions.CanProceed()) return;
            if (Playfield.ModelIdentity.Instance != Constants.DB1Id) return;

            var mikkelsenCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Ground Chief Mikkelsen");

            if (mikkelsenCorpse != null)
            {
                if (!_atCorpse)
                {
                    if (AtPosition(mikkelsenCorpse.Position, 2))
                    {
                        Chat.WriteLine("Pause for looting, 30 sec");
                        _timeToLeave = Time.AONormalTime + 30;
                        _atCorpse = true;
                    }
                }
                else if (Time.AONormalTime < _timeToLeave) return;
                else
                    Disband();
            }
            else
                Disband();
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

        private void Disband()
        {
            if (!Team.IsInTeam) return;

            foreach (var member in Team.Members)
            {
                if (AutomatonDB1._teamCache.Contains(member.Identity)) continue;
                AutomatonDB1._teamCache.Add(member.Identity);

                if (DynelManager.LocalPlayer.Identity != AutomatonDB1.Leader) continue;
                if (member.Identity == AutomatonDB1.Leader) continue;
                Team.Kick(member.Identity);
            }
        }

        public void OnStateExit()
        {
            _atCorpse = false;
            _timeToLeave = 0;
            DynelManager.LocalPlayer.Position = Constants._reformPos;
        }
    }
}