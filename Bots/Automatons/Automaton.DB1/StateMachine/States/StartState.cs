using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonDB1
{
    public class StartState : IState
    {
        double delay = 0;
        
        public void OnStateEnter()
        {
            delay = Time.AONormalTime + 3;
            Chat.WriteLine("Start State");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Playfield.ModelIdentity.Instance != Constants.DB1Id)
                return AutomatonDB1.Idle;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._startPosition) < 10f)
                return AutomatonDB1.BuffState;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;
            if (!Team.IsInTeam) { return; }
            if (Playfield.ModelIdentity.Instance != Constants.DB1Id) return;
            if (Team.Members.Any(c => c.Character == null)) return;
            if(Time.AONormalTime < delay) return;

            var _maskedCommando = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Masked Commando"));

            if (_maskedCommando != null)
            {
                if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                    DynelManager.LocalPlayer.Attack(_maskedCommando, false);
            }
            else if (!MovementController.Instance.IsNavigating && DynelManager.LocalPlayer.Position.DistanceFrom(Constants._startPosition) > 5f)
                AutomatonDB1.NavMeshMovementController.SetNavMeshDestination(Constants._startPosition);
        }

        public void OnStateExit()
        {
            delay = 0;
            Chat.WriteLine("Exit StartState");
        }
    }
}