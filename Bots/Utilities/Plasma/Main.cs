using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using AOSharp.Common.GameData;

namespace Plasma
{
    public class Main : AOPluginEntry
    {
        public static bool Toggle = false;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("Plasma loaded!");
                Chat.WriteLine("/plasma for toggle.");
                Chat.WriteLine("Name bags 'Parts' and 'Plasma' even if multiple.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("plasma", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Toggle = !Toggle;
                    Chat.WriteLine($"Plasma : {Toggle}");
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
                if (!Inventory.Items.Any(c => c.Name == "Monster Parts")
                    && !Inventory.Items.Any(c => c.Name == "Blood Plasma"))
                {
                    Container _bag = Inventory.Backpacks.FirstOrDefault(c => c.IsOpen && c.Items.Count() > 0 && c.Name.Contains("Parts"));

                    if (_bag != null)
                        foreach (Item item in _bag?.Items.Take(Inventory.NumFreeSlots))
                            item?.MoveToInventory();
                }

                if (Inventory.Find("Monster Parts", out Item parts))
                    if (Inventory.Find("Advanced Bio-Comminutor", out Item bio))
                        bio.CombineWith(parts);

                if (Inventory.Find("Blood Plasma", out Item plasma))
                    plasma.MoveToContainer(Inventory.Backpacks.FirstOrDefault(c => c.IsOpen && c.Items.Count() < 21 && c.Name.Contains("Plasma")));
            }
        }

        public override void Teardown()
        {
        }
    }
}
