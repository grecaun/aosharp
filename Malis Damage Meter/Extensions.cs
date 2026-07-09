using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Common.Unmanaged.DataTypes;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Common.Unmanaged.Interfaces;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MalisDamageMeter
{
    public static class Extensions
    {
        public static WeaponInfo GetWeaponInfo(this AttackInfoMessage attckInfoMsg)
        {
            if (!DynelManager.Find(attckInfoMsg.Identity, out SimpleChar infoChar))
                return null;

            bool isEquippableWeapon = ((WeaponSlots)attckInfoMsg.WeaponSlot).IsEquippableWeapon();

            WeaponSlots slot = attckInfoMsg.WeaponSlot > 0 && attckInfoMsg.WeaponSlot < 5 ? WeaponSlots.FistOrPet : (WeaponSlots)attckInfoMsg.WeaponSlot;

            WeaponInfo weaponInfo = new WeaponInfo
            {
                Slot = slot,
                DummyItem = new WeaponStat { Name = "", LowId = 0, HighId = 0, Ql = 0 },
            };

            if (infoChar.Weapons.Count() != 0 && isEquippableWeapon)
            {
                weaponInfo.DummyItem.Name = infoChar.Weapons[(EquipSlot)attckInfoMsg.WeaponSlot].Name;
                weaponInfo.DummyItem.LowId = infoChar.Weapons[(EquipSlot)attckInfoMsg.WeaponSlot].GetStat(Stat.ACGItemTemplateID);
                weaponInfo.DummyItem.HighId = infoChar.Weapons[(EquipSlot)attckInfoMsg.WeaponSlot].GetStat(Stat.ACGItemTemplateID2);
                weaponInfo.DummyItem.Ql = infoChar.Weapons[(EquipSlot)attckInfoMsg.WeaponSlot].GetStat(Stat.ACGItemLevel);
            }

            if (infoChar.GetStat(Stat.DamageType1) != 0)
            {
                weaponInfo.DamageType = (Stat)infoChar.GetStat(Stat.DamageType1);
            }
            else if (isEquippableWeapon)
            {
                weaponInfo.DamageType = (Stat)infoChar.Weapons[(EquipSlot)attckInfoMsg.WeaponSlot].GetStat(Stat.DamageType2);
            }
            else
            {
                weaponInfo.DamageType = Stat.MeleeAC;
            }

            return weaponInfo;
        }

        public static bool IsEquippableWeapon(this WeaponSlots weaponSlot) => weaponSlot == WeaponSlots.MainHand || weaponSlot == WeaponSlots.Offhand;

        public static bool IsDamage(this HealthDamageMessage healthMsg) => healthMsg.Amount < 0 ? true : false;

        public static void Redraw(this List<MeterView> meterViews, View meterRoot, int count)
        {
            foreach (var meterView in meterViews)
            {
                meterRoot.RemoveChild(meterView.Root);
                meterView.Root.Dispose();
            }

            meterViews.Clear();

            for (int i = 0; i < count; i++)
            {
                MeterView meterView = new MeterView();
                meterRoot.AddChild(meterView.Root, true);
                meterViews.Add(meterView);
            }

            meterRoot.FitToContents();
        }

        public static void SetAllGfx(this Button button, int gfxId)
        {
            button.SetGfx(ButtonState.Raised, gfxId);
            button.SetGfx(ButtonState.Hover, gfxId);
            button.SetGfx(ButtonState.Pressed, gfxId);
        }

        public static int GetOwnerId(this SimpleChar simpleChar) => simpleChar.IsPet ? simpleChar.GetStat((Stat)196) : 0;
    }
}