
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;

namespace AutomatonDB2
{
    public class ClearBeforeEntering : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("Kill the attacking mob!");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Playfield.ModelIdentity.Instance != Constants.PWId)
                return AutomatonDB2.Idle;

            if (!DynelManager.NPCs.Any(m => m.IsAttacking && Team.Members.Any(t => t.Identity == m.FightingTarget.Identity))) return AutomatonDB2.Idle;

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;

            var localPlayer = DynelManager.LocalPlayer;
            var attackingMob = DynelManager.NPCs.FirstOrDefault(m => m.IsAttacking && Team.Members.Any(t => t.Identity == m.FightingTarget.Identity));

            if (attackingMob == null) return;

            if (localPlayer.Identity == AutomatonDB2.Leader)
            {
                if (localPlayer.IsAttacking) return;
                if (localPlayer.FightingTarget == null)
                    localPlayer.Attack(attackingMob.Identity);
            }
            else
            {
                var leader = DynelManager.Players.FirstOrDefault(l => l.Identity == AutomatonDB2.Leader && l.Health > 0);

                if (leader != null)
                {
                    if (leader.IsAttacking)
                    {
                        var leaderTarget = leader.FightingTarget;

                        if (localPlayer.IsAttacking) return;
                        if (localPlayer.FightingTarget == null)
                            localPlayer.Attack(leaderTarget.Identity);
                    }
                }
                else
                {
                    if (localPlayer.IsAttacking) return;
                    if (localPlayer.FightingTarget == null)
                        localPlayer.Attack(attackingMob.Identity);
                }
            }
        }

        public void OnStateExit()
        {

        }
    }
}