using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using System;
using System.Linq;

namespace Mounter
{
    public partial class Main : AOPluginEntry
    {
        public static IPCChannel IPCChannel;

        public override void Run()
        {
            IPCChannel = new IPCChannel(Convert.ToByte(0));

            try
            {
                IPCChannel.RegisterCallback((int)IPCOpcode.Mount, OnMountMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.Dismount, OnDismountMessage);
            }
            catch (Exception e)
            {
                Chat.WriteLine(e);
            }

            Chat.RegisterCommand("mount", (string command, string[] param, ChatWindow chatWindow) =>
            {
                try
                {
                    // Check if mounted.
                    foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
                    {
                        if (buff.Nanoline == NanoLine.Vehicles || buff.Name.Contains("Phasefront"))
                        {
                            buff.Remove();
                            IPCChannel.Broadcast(new DismountMessage
                            {
                                Sender = DynelManager.LocalPlayer.Identity,
                            });
                            return;
                        }
                    }
                    // Mount if not mounted.
                    foreach (var spell in Spell.List.OrderByStackingOrder())
                    {
                        if (spell.Nanoline == NanoLine.Vehicles || spell.Name.Contains("Phasefront"))
                        {
                            spell.Cast();
                            IPCChannel.Broadcast(new MountMessage
                            {
                                Sender = DynelManager.LocalPlayer.Identity,
                            });
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Chat.WriteLine(e);
                }
            });

            Chat.WriteLine("Mounter Loaded!");
            Chat.WriteLine("/mount to enter or exit your vehicle.");
        }

        public void OnMountMessage(int sender, IPCMessage msg)
        {
            if (!(msg is MountMessage mountMsg)) return;
            if (!DynelManager.Players.Any(p => p.Identity == mountMsg.Sender)) { return; }
            foreach (var spell in Spell.List.OrderByStackingOrder())
            {
                if (spell.Nanoline == NanoLine.Vehicles || spell.Name.Contains("Phasefront"))
                {
                    spell.Cast();
                    return;
                }
            }
        }

        public void OnDismountMessage(int sender, IPCMessage msg)
        {
            if (!(msg is DismountMessage mountMsg)) return;
            if (!DynelManager.Players.Any(p => p.Identity == mountMsg.Sender)) { return; }
            foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
            {
                if (buff.Nanoline == NanoLine.Vehicles || buff.Name.Contains("Phasefront"))
                {
                    buff.Remove();
                }
            }
        }
    }
}