using AOSharp.Core;
using AOSharp.Common.GameData;
using System.Linq;
using AOSharp.Core.Inventory;
using AOSharp.Core.Movement;
using System.Collections.Generic;

namespace Shared
{
    public class Kits
    {
        public void SitAndUseKit(int nanoThreshold, int healthThreshold)
        {
            var localPlayer = DynelManager.LocalPlayer;
            if (Game.IsZoning) return;
            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 1) return;

            if ((localPlayer.NanoPercent < nanoThreshold || localPlayer.HealthPercent < healthThreshold) & CanUseSitKit())
            {
                if (localPlayer.MovementState != MovementState.Sit && !localPlayer.IsMoving)
                    MovementController.Instance.SetMovement(MovementAction.SwitchToSit);
                else
                    UseKit();
            }

            if ((localPlayer.NanoPercent >= nanoThreshold && localPlayer.HealthPercent >= healthThreshold) || !CanUseSitKit())
            {
                if (localPlayer.MovementState == MovementState.Sit && !localPlayer.IsMoving)
                    MovementController.Instance.SetMovement(MovementAction.LeaveSit);
            }
        }

        public bool CanUseSitKit()
        {
            var localPlayer = DynelManager.LocalPlayer;
            
            if (InCombat() || Casting() || localPlayer.Cooldowns.ContainsKey(Stat.Treatment) || localPlayer.IsFalling || localPlayer.IsMoving) return false;

            if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(b => Inventory.GetContainerItems(b.Identity))).Any(i => i.Id == 297274)) return true;

            var sitKits = Inventory.Items.Concat(Inventory.Backpacks.SelectMany(b => Inventory.GetContainerItems(b.Identity)))
                .Where(i => i.Name == "Health and Nano Recharger" && i.Id != 297274).ToList();

            if (sitKits == null) return false;

            if (sitKits.Any())
                return sitKits.OrderBy(x => x.QualityLevel).Any(sitKit => MeetsSkillRequirement(sitKit));

            return false;
        }

        public void UseKit()
        {
            var kit = Inventory.Items
                .Concat(Inventory.Backpacks.SelectMany(b => Inventory.GetContainerItems(b.Identity)))
                .FirstOrDefault(x => RelevantItems.Kits.Contains(x.Id));

            if (kit == null) return;

            if (!Item.HasPendingUse)
                kit.Use(DynelManager.LocalPlayer, true);
        }

        public bool MeetsSkillRequirement(Item sitKit)
        {
            var localPlayer = DynelManager.LocalPlayer;

            int skillReq = sitKit.QualityLevel > 200 ? (sitKit.QualityLevel % 200 * 3) + 1501 : (int)(sitKit.QualityLevel * 7.5f);

            return localPlayer.GetStat(Stat.FirstAid) >= skillReq || localPlayer.GetStat(Stat.Treatment) >= skillReq;
        }

        public static bool InCombat()
        {
            var localPlayer = DynelManager.LocalPlayer;

            return DynelManager.Characters.Any(c => c != null && (
                localPlayer.IsAttacking || (localPlayer.Pets != null && localPlayer.Pets.Any(p => p?.Character?.IsAttacking == true)) ||
                (Team.IsInTeam && Team.Members.Any(tm => tm.Character != null && (tm.Character.IsAttacking || (c.IsPet && c.PetOwnerId == tm.Identity.Instance && c.IsAttacking)))) ||
                (c.IsNpc && c.IsAttacking && (c.FightingTarget?.Identity == localPlayer.Identity || (localPlayer.Pets != null && localPlayer.Pets.Any(p => p?.Character?.Identity == c.FightingTarget?.Identity)) ||
                (Team.IsInTeam && Team.Members.Any(tm => tm.Character != null && tm.Identity == c.FightingTarget?.Identity)) ||
                (Team.IsInTeam && Team.Members.Any(tm => tm.Character != null && DynelManager.Characters.Any(pet => pet != null && pet.IsPet && pet.PetOwnerId == tm.Identity.Instance && pet.Identity == c.FightingTarget?.Identity)))))));
        }

        public static bool Casting()
        {
            return Spell.HasPendingCast || !Spell.List.Any(spell => spell.IsReady);
        }
    }

    public static class RelevantItems
    {
        public static readonly int[] Kits = { 297274, 293297, 293296, 291084, 291083, 292256, 291082 };

    }
}