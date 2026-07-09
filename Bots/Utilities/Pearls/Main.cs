using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using AOSharp.Common.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.GameData;

namespace Pearls
{
    public class Main : AOPluginEntry
    {
        public static bool Toggle = false;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("Pearls loaded!");
                Chat.WriteLine("/pearls for toggle.");
                Chat.WriteLine("Name bags 'Pearls' and 'Perfect' even if multiple.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("pearls", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Toggle = !Toggle;
                    Chat.WriteLine($"Pearls : {Toggle}");
                });
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if (Toggle)
            {
                if (!Inventory.Items.Any(c => c.Name == "Pearl")
                    && !Inventory.Items.Any(c => c.Name == "Perfectly Cut Pearl"))
                {
                    Container _bag = Inventory.Backpacks.FirstOrDefault(c => c.IsOpen && c.Items.Count() > 0 && c.Name.Contains("Pearls"));

                    if (_bag != null)
                        foreach (Item item in _bag?.Items.Take(Inventory.NumFreeSlots))
                            item?.MoveToInventory();
                }

                if (Inventory.Find("Pearl", out Item pearl))
                    if (Inventory.Find("Jensen Gem Cutter", out Item cutter))
                        cutter.CombineWith(pearl);

                if (Inventory.Find("Perfectly Cut Pearl", out Item perfectPearl))
                    perfectPearl.MoveToContainer(Inventory.Backpacks.FirstOrDefault(c => c.IsOpen && c.Items.Count() < 21 && c.Name.Contains("Perfect")));
            }
        }

        public override void Teardown()
        {
        }
    }
}
