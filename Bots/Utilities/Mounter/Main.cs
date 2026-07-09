using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mounter
{
    public class Main : AOPluginEntry
    {

        public override void Run()
        {

            Chat.RegisterCommand("mount", (string command, string[] param, ChatWindow chatWindow) =>
            {
                try
                {
                    // Check if mounted.
                    bool notMounted = true;
                    foreach (Buff buff in DynelManager.LocalPlayer.Buffs)
                    {
                        if (buff.Nanoline == NanoLine.Vehicles || buff.Name.Contains("Phasefront"))
                        {
                            buff.Remove();
                            notMounted = false;
                        }
                    }
                    // Mount if not mounted.
                    if (notMounted)
                    {
                        foreach (var spell in Spell.List.OrderByStackingOrder())
                        {
                            if (spell.Nanoline == NanoLine.Vehicles || spell.Name.Contains("Phasefront"))
                            {
                                spell.Cast();
                                return;
                            }
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
    }
}