using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        private void PerformSpecialAttacks(SimpleChar target)
        {
            try
            {
                if (!_settings["Specials"].AsBool()) return;
                if (Game.IsZoning) return;
                if (target == null) return;
                if (target.IsPlayer) return;
                if (target.Buffs.Contains(302808)) return;
                if (DynelManager.LocalPlayer.WeaponHolder.WeaponsAreBusy) return;

                var specials = DynelManager.LocalPlayer.SpecialAttacks.Where(sa => sa.IsAvailable() && sa.IsInRange(target));
                
                var special = specials.FirstOrDefault(sa => 
                {
                    if (sa == SpecialAttack.FlingShot && DynelManager.LocalPlayer.Weapons.Values.FirstOrDefault(w => w.SpecialAttacks.Contains(SpecialAttack.FlingShot))?.Ammo != -1
                    && DynelManager.LocalPlayer.Weapons.Values.FirstOrDefault(w => w.SpecialAttacks.Contains(SpecialAttack.FlingShot))?.Ammo < 2)
                        return false;

                    if (sa == SpecialAttack.Burst && DynelManager.LocalPlayer.Weapons.Values.FirstOrDefault(w => w.SpecialAttacks.Contains(SpecialAttack.Burst))?.Ammo != -1
                    && DynelManager.LocalPlayer.Weapons.Values.FirstOrDefault(w => w.SpecialAttacks.Contains(SpecialAttack.Burst))?.Ammo < 3)
                        return false;

                    if (sa == SpecialAttack.FullAuto)
                    {
                        var weapon = DynelManager.LocalPlayer.Weapons.Values.FirstOrDefault(w => w.SpecialAttacks.Contains(SpecialAttack.FullAuto));

                        if (weapon == null) return false;
                        if (weapon.Ammo == -1) return true;

                        if (5 + (DynelManager.LocalPlayer.GetStat(Stat.FullAuto) / 100) >= weapon.MaxAmmo)
                        {
                            if (weapon.Ammo <= 3)
                            {
                                weapon.Reload();
                                ReloadDelay = Time.AONormalTime + 3;
                            }
                            else if (ReloadDelay > Time.AONormalTime)
                                return true;
                        }

                        if (weapon.Ammo < 5 + (DynelManager.LocalPlayer.GetStat(Stat.FullAuto) / 100))
                        {
                            if (ReloadDelay > Time.AONormalTime) return false;
                            weapon.Reload();
                            ReloadDelay = Time.AONormalTime + 3;
                            return false;
                        }
                        return true;
                    }

                    if ((sa == SpecialAttack.SneakAttack || sa == SpecialAttack.AimedShot) && DynelManager.LocalPlayer.MovementState != MovementState.Sneak)
                        return false;

                    if (sa == SpecialAttack.Backstab)
                    {
                        if (target.FightingTarget == null || (target.FightingTarget != null && target.FightingTarget.Identity == DynelManager.LocalPlayer.Identity)) return false;

                        if (_settings["ShouldMoveBehindTarget"].AsInt32() == 1)
                            MoveBehindFightingtarget();
                        else if (_settings["ShouldMoveBehindTarget"].AsInt32() == 2 && (target.MaxHealth > 1000000 || BossNames.Contains(target.Name)))
                            MoveBehindFightingtarget();
                    }

                    if (sa == SpecialAttack.Dimach) return false;

                    return true;
                });

                special?.UseOn(target);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }
    }
}
