using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonVortexx
{
    public class ImmunityState : IState
    {
        private SimpleChar _vortexx = null;
        private SimpleChar _releasedSpirit = null;

        private Dictionary<string, double> _timeToUseItem = new Dictionary<string, double>();

        public void OnStateEnter()
        {
            Chat.WriteLine("ImmunityState");
            _timeToUseItem.Clear();
        }

        public IState GetNextState()
        {
            if (Playfield.ModelIdentity.Instance != Constants.VortexxId) return null;

            _releasedSpirit = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Released Spirit"));

            if (_releasedSpirit == null)
                return AutomatonVortexx.Fight;

            return null;
        }

        public void Tick()
        {
            if (!Team.IsInTeam || Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance != Constants.VortexxId) return;


            _vortexx = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name.Contains("Ground Chief Vortexx")).FirstOrDefault();

            _releasedSpirit = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name.Contains("Released Spirit")).FirstOrDefault();


            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPodium) > 5f && !MovementController.Instance.IsNavigating)
                AutomatonVortexx.NavMeshMovementController.SetNavMeshDestination(Constants._centerPodium);

            if (AutomatonVortexx._settings["Immunity"].AsBool() && _releasedSpirit != null)
            {
                if (DynelManager.LocalPlayer.FightingTarget != null && DynelManager.LocalPlayer.FightingTarget.Name == _vortexx.Name)
                    DynelManager.LocalPlayer.StopAttack(false);
            }

            var spiritsNearRedPodium = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name.Contains("Released Spirit") && c.Position.DistanceFrom(Constants._redPodium) < 5).ToList();

            var spiritsNearGreenPodium = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name.Contains("Released Spirit") && c.Position.DistanceFrom(Constants._greenPodium) < 5).ToList();

            var spiritsNearYellowPodium = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name.Contains("Released Spirit") && c.Position.DistanceFrom(Constants._yellowPodium) < 5).ToList();

            var spiritsNearBluePodium = DynelManager.NPCs.Where(c => c.Health > 0 && c.Name.Contains("Released Spirit") && c.Position.DistanceFrom(Constants._bluePodium) < 5).ToList();

            if (spiritsNearRedPodium.Any())
                CheckAndUseCrystal("Red", ImmunityCrystals.BloodRedNotumCrystal);

            if (spiritsNearGreenPodium.Any())
                CheckAndUseCrystal("Green", ImmunityCrystals.PulsatingGreenNotumCrystal);

            if (spiritsNearYellowPodium.Any())
                CheckAndUseCrystal("Golden", ImmunityCrystals.GoldenNotumCrystal);

            if (spiritsNearBluePodium.Any())
                CheckAndUseCrystal("Blue", ImmunityCrystals.CobaltBlueNotumCrystal);
        }

        private void CheckAndUseCrystal(string color, int[] crystalIds)
        {
            if (!_timeToUseItem.ContainsKey(color))
                _timeToUseItem[color] = Time.AONormalTime + 10.0;  // 10 seconds delay
            else if (_timeToUseItem[color] <= Time.AONormalTime)
            {
                Item crystal = Inventory.Items.Where(x => crystalIds.Contains(x.Id)).FirstOrDefault();

                if (!Item.HasPendingUse)
                    crystal.Use();

                _timeToUseItem[color] = Time.AONormalTime + 10.0;  // Reset the timer
            }
        }

        public static class ImmunityCrystals
        {
            public static readonly int[] BloodRedNotumCrystal = { 280581 };
            public static readonly int[] PulsatingGreenNotumCrystal = { 280585 };
            public static readonly int[] GoldenNotumCrystal = { 280586 };
            public static readonly int[] CobaltBlueNotumCrystal = { 280584 };

        }
        public void OnStateExit()
        {
            Chat.WriteLine("Exit ImmunityState");
            _timeToUseItem.Clear();
        }
    }
}