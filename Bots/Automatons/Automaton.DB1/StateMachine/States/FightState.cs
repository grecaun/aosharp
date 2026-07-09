using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonDB1
{
    public class FightState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("FightState");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Playfield.ModelIdentity.Instance != Constants.DB1Id)
                return AutomatonDB1.Idle;

            if (HasDebuff()) return null;

            if (Playfield.ModelIdentity.Instance == Constants.DB1Id && !DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.ThriceBlessedbytheAncients))
                return AutomatonDB1.BuffState;

            var mikkelsenCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("Remains of Ground Chief Mikkelsen"));

            if (mikkelsenCorpse == null) return null;

            if (Extensions.CanProceed() && AutomatonDB1._settings["Farming"].AsBool())
                return AutomatonDB1.Loot;

            return null;
        }

        public void Tick()
        {
            if (!Team.IsInTeam || Game.IsZoning || Playfield.ModelIdentity.Instance != Constants.DB1Id)
                return;

            var mikkelsen = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Ground Chief Mikkelsen"));
            var mikkelsenCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("Remains of Ground Chief Mikkelsen"));
            var maskedCommando = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Masked Commando"));

            if (mikkelsenCorpse != null) return;

            if (!MovementController.Instance.IsNavigating)
            {
                var debuffDestinations = new List<(Vector3 position, bool active)>
                {
                    (Constants._redPodium, DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.HealingBlight)),
                    (Constants._bluePodium, DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.CallofRust)),
                    (Constants._greenPodium, DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.CrawlingSkin)),
                    (Constants._yellowPodium, DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.GreedoftheSource))
                };

                var closest = debuffDestinations.Where(d => d.active).OrderBy(d => DynelManager.LocalPlayer.Position.DistanceFrom(d.position)).FirstOrDefault();

                if (closest.active && DynelManager.LocalPlayer.Position.DistanceFrom(closest.position) > 2f)
                {
                    MovementController.Instance.SetDestination(closest.position);
                    return;
                }
               
            }

            if (!HasDebuff() && mikkelsen != null && DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                DynelManager.LocalPlayer.Attack(mikkelsen, false);

            if (DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.CallofRust) && DynelManager.LocalPlayer.FightingTarget?.Name == mikkelsen?.Name)
                DynelManager.LocalPlayer.StopAttack(false);

            if (!HasDebuff() && maskedCommando != null && DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                DynelManager.LocalPlayer.Attack(maskedCommando, false);

            if (!HasDebuff())
            {
                 if (DynelManager.LocalPlayer.Position.Distance2DFrom(Constants._centerOfPodiums) > 5)
                    MovementController.Instance.SetDestination(Constants._centerOfPodiums);
                return;
                //if (mikkelsen != null && DynelManager.LocalPlayer.Position.DistanceFrom(mikkelsen.Position) > 7f)
                //    AutomatonDB1.NavMeshMovementController.SetNavMeshDestination(mikkelsen.Position);
                //else if (mikkelsen == null && DynelManager.LocalPlayer.Position.DistanceFrom(Constants._returnPosition) > 10f)
                //    AutomatonDB1.NavMeshMovementController.SetNavMeshDestination(Constants._returnPosition);
            }
        }

        private bool HasDebuff()
        {
            return DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.HealingBlight) ||
                   DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.CallofRust) ||
                   DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.CrawlingSkin) ||
                   DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.GreedoftheSource);
        }

        public void OnStateExit()
        {
            AutomatonDB1._settings["LastRunTime"] = (float)(Time.AONormalTime - AutomatonDB1.StartStamp);
            Chat.WriteLine("Exit FightState");
        }
    }
}
