using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using ManagerHelp.IPCMessages;

namespace ManagerHelp
{
    public partial class ManagerHelp
    {
        List<(string Name, int Id)> rkList = new List<(string, int)>();
        List<(string Name, int Id)> slList = new List<(string, int)>();

        private readonly int[] YalmSpells = {  290473, 281569, 301672, 270984, 270991, 273468, 288795, 270993, 270995, 270986, 270982, 296034, 296669, 304437, 270884, 270941, 270836, 287285, 288816, 270943, 270939,
            270945, 270711, 270731, 270645, 284061, 288802, 270764, 277426, 288799, 270738, 270779, 293619, 294781, 301669, 301700, 301670, 120499,};

        private readonly int[] Hoverboards = { 270634, 270632, 270636, 270327, 277712, 288804, 270643, 270641, 270431, 270540, 270542, 274272, 288808, 281684, 288814, 270538, 281668, 288812, 270544, 270546 };

        private readonly int[] GraftSparrowFlight = { 128345, 128346, 128344, 128343, 94577, 94799, 94049, 93648, 94798, 94048 };

        private readonly int[] SLVehicles = { 295704 };

        private readonly int[] Sparrow = { 82835 };

        private void FlyMessageReceived(int arg1, IPCMessage message)
        {
            if (!(message is FlyMessage flyMsg)) return;

            var lp = DynelManager.LocalPlayer;

            if (flyMsg.Action == 0)
            {
                if (lp.Buffs.Contains(YalmSpells) || lp.Buffs.Contains(Hoverboards) || lp.Buffs.Contains(SLVehicles) || lp.Buffs.Contains(Sparrow))
                {
                    foreach (var yalm in lp.Buffs.Where(buff =>
                        YalmSpells.Contains(buff.Id) ||
                        Hoverboards.Contains(buff.Id) ||
                        SLVehicles.Contains(buff.Id) ||
                        Sparrow.Contains(buff.Id)))
                        yalm.Remove();
                }
                else
                {
                    Inventory.Items.Where(x =>
                        (x.Name.Contains("Yalm") || x.Name.Contains("Ganimedes")) &&
                        x.Slot.Type == IdentityType.WeaponPage).FirstOrDefault()?.MoveToInventory();
                }
            }

            if (flyMsg.Action == 1)
            {
                if (Playfield.IsShadowlands)
                {
                    if (Spell.Find(_settings["SLYalm"].AsInt32(), out Spell spell))
                        spell?.Cast();
                }
                else if (!Spell.Find(_settings["RKYalm"].AsInt32(), out Spell spell))
                {
                    if (Inventory.Find(_settings["RKYalm"].AsInt32(), out Item item))
                        item?.Equip(EquipSlot.Weap_Hud1);
                }
                else
                    spell?.Cast();
            }
        }

        private void FlyCommand(string arg1, string[] arg2, ChatWindow window)
        {
            var lp = DynelManager.LocalPlayer;

            if (lp.Buffs.Contains(YalmSpells) || lp.Buffs.Contains(Hoverboards) || lp.Buffs.Contains(SLVehicles) || lp.Buffs.Contains(Sparrow))
            {
                foreach (var yalm in lp.Buffs.Where(buff =>
                    YalmSpells.Contains(buff.Id) ||
                    Hoverboards.Contains(buff.Id) ||
                    SLVehicles.Contains(buff.Id) ||
                    Sparrow.Contains(buff.Id)))
                    yalm.Remove();

                IPCChannel.Broadcast(new FlyMessage { Action = 0 });
            }
            else if (Inventory.Items.Where(x =>
                        (x.Name.Contains("Yalm") || x.Name.Contains("Ganimedes")) &&
                        x.Slot.Type == IdentityType.WeaponPage).FirstOrDefault() != null)
            {
                Inventory.Items.Where(x =>
                    (x.Name.Contains("Yalm") || x.Name.Contains("Ganimedes")) &&
                    x.Slot.Type == IdentityType.WeaponPage).FirstOrDefault().MoveToInventory();

                IPCChannel.Broadcast(new FlyMessage { Action = 0 });
            }
            else if (Playfield.IsShadowlands)
            {
                if (Spell.Find(_settings["SLYalm"].AsInt32(), out Spell spell))
                    spell?.Cast();

                IPCChannel.Broadcast(new FlyMessage { Action = 1 });
            }
            else if (!Spell.Find(_settings["RKYalm"].AsInt32(), out Spell spell))
            {
                if (Inventory.Find(_settings["RKYalm"].AsInt32(), out Item item))
                    item?.Equip(EquipSlot.Weap_Hud1);

                IPCChannel.Broadcast(new FlyMessage { Action = 1 });
            }
            else
            {
                IPCChannel.Broadcast(new FlyMessage { Action = 1 });
                spell?.Cast();
            }
        }

        private void Yalm_Selection_Message_Received(int arg1, IPCMessage message)
        {
            if (!(message is BroadcastYalms BCY)) return;

            _settings["RKYalm"] = BCY.RKSelection;
            _settings["SLYalm"] = BCY.SLSelection;
            Save();
        }
    }
}
