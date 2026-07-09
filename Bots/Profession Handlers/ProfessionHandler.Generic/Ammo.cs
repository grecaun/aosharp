using AOSharp.Common.GameData;
using AOSharp.Core.Inventory;
using AOSharp.Core;
using System.Linq;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.GameData;
using System;
using System.Collections.Generic;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        internal class Ammo
        {
            public static Dictionary<EquipSlot, Tuple<string, int>> WeaponAmmo = new Dictionary<EquipSlot, Tuple<string, int>>();
            public static double delay;
            static string name;
            static int id;

            public static void CrateOfAmmo()
            {
                try
                {
                    var RightHand = DynelManager.LocalPlayer.GetWeapon((int)EquipSlot.Weap_RightHand);
                    var LeftHand = DynelManager.LocalPlayer.GetWeapon((int)EquipSlot.Weap_LeftHand);

                    if (DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.WeaponSmithing)) return;
                    if (Item.HasPendingUse) return;

                    foreach (var weaponItem in DynelManager.LocalPlayer.Weapons)
                    {
                        GetAmmoType(weaponItem.Key, weaponItem.Value);
                    }

                    foreach (var ammoType in WeaponAmmo)
                    {
                        if (Inventory.NumFreeSlots >= 2)
                        {
                            var anyCrate = Inventory.Items.Where(c => c.Name.Contains($"Crate of ")).FirstOrDefault();

                            if (anyCrate != null)
                            {
                                if (ammoType.Value.Item2 > 0)
                                {
                                    var boxOfAmmo = Inventory.Items.Where(c => c.Name == ammoType.Value.Item1).FirstOrDefault();

                                    if (boxOfAmmo == null)
                                    {
                                        if (anyCrate.HighId == ammoType.Value.Item2)
                                        {
                                            anyCrate.Use();
                                        }
                                        else
                                        {
                                            var ScrewDriver = Inventory.Items.Where(c => c.HighId == 150922).FirstOrDefault();

                                            if (ScrewDriver != null)
                                            {
                                                if (Time.AONormalTime > delay)
                                                {
                                                    Network.Send(new CharacterActionMessage
                                                    {
                                                        Action = CharacterActionType.UseItemOnItem,
                                                        Target = ScrewDriver.Slot,
                                                        Parameter1 = (int)IdentityType.Inventory,
                                                        Parameter2 = anyCrate.Slot.Instance,
                                                    });

                                                    delay = Time.AONormalTime + 1.0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorCatch(ex);
                }
            }

            private static void GetAmmoType(EquipSlot slot, WeaponItem weapon)
            {
                if (weapon == null) return;

                switch (weapon.GetStat(Stat.AmmoType))
                {
                    case 1:
                        name = "Ammo: Box of Energy Weapon Ammo";
                        id = 303138;
                        break;
                    case 2:
                        name = "Ammo: Box of Bullets";
                        id = 303137;
                        break;
                    case 3:
                        name = "Ammo: Box of Flamethrower Ammunition";
                        id = 303139;
                        break;
                    case 4:
                        name = "Ammo: Box of Shotgun Shells";
                        id = 303141;
                        break;
                    case 5:
                        name = "Ammo: Box of Arrows";
                        id = 303136;
                        break;
                    case 6:
                        name = "Ammo: Box of Launcher Grenades";
                        id = 303140;
                        break;
                }

                if (!WeaponAmmo.ContainsKey(slot))
                {
                    WeaponAmmo[slot] = new Tuple<string, int>(name, id);
                }
            }
        }
    }
}
