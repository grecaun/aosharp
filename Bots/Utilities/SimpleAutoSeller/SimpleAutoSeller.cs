using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using AOSharp.Common.GameData;

namespace SAS
{
    public class SimpleAutoSeller : AOPluginEntry
    {
        private static double _pearlDelay;
        private static double _monsterDelay;

        public static bool Toggle = false;
        public static bool SetBags = false;
        protected double LastZonedTime = Time.AONormalTime;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("SimpeAutoSeller loaded!");
                Chat.WriteLine("Use Specialist Commerce in ICC");
                Chat.WriteLine("/sas for toggle.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("sas", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Toggle = !Toggle;
                    SetBags = false;
                    Chat.WriteLine($"SAS Active : {Toggle}");
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
                if (!SetBags)
                {
                    List<Item> bags = Inventory.Items.Where(c => c.UniqueIdentity.Type == IdentityType.Container).ToList();

                    foreach (Item bag in bags)
                    {
                        bag.Use();
                        bag.Use();
                        SetBags = true;
                    }
                }

                if (!Inventory.Items.Any(c => c.Name.Contains("Pearl") || c.Name.Contains("Monster Parts") || c.Name.Contains("Pattern") || c.Name.Contains("Blood Plasma")))
                {
                    Container LootBag = Inventory.Backpacks.FirstOrDefault(c => c.IsOpen && c.Items.Count() > 0 && c.Name.Contains("loot"));

                    if (LootBag != null)
                    {
                        foreach (Item MoveItem in LootBag.Items.Take(Inventory.NumFreeSlots - 1))
                        {
                            if (MoveItem.Name.Contains("Pearl") || MoveItem.Name.Contains("Pattern") || MoveItem.Name.Contains("Monster Parts")
                                   || MoveItem.Name.Contains("Blood Plasma"))
                            {
                                MoveItem.MoveToInventory();
                            }
                        }
                    }
                }

                if (DynelManager.Find("Specialist Commerce", out SimpleItem SpecCom)) { }

                if (Inventory.Items.Any(c => c.Name.Contains("Monster Parts")))
                {
                    ProccessPlasma();
                }

                if (Inventory.Items.Any(c => c.Name.Contains("Pearl") && !c.Name.Contains("Perfectly")))
                {
                    ProccessPearl();
                }

                foreach (Item SellItem in Inventory.Items.Where(c => c.Slot.Type == IdentityType.Inventory))
                {
                    if (SellItem.Name.Contains("Blood Plasma") || SellItem.Name.Contains("Pattern") || SellItem.Name.Contains("Perfectly Cut"))
                        SpecCom.Use();
                    Trade.AddItem(DynelManager.LocalPlayer.Identity, SellItem.Slot);
                    Trade.Accept(Identity.None);
                }
            }
        }
        private void ProccessPearl()
        {
            Item pearl = Inventory.Items.Where(c => c.Name.Contains("Pearl") && !c.Name.Contains("Perfectly")).FirstOrDefault();

            Item cutter = Inventory.Items.Where(c => c.Name.Contains("Jensen Gem Cutter")).FirstOrDefault();

            if (pearl != null && cutter != null && Time.AONormalTime > _pearlDelay + .35)
            {
                cutter.CombineWith(pearl);

                _pearlDelay = Time.AONormalTime;
            }
        }
        private void ProccessPlasma()
        {

            Item parts = Inventory.Items
                       .Where(c => c.Name.Contains("Monster Parts"))
                       .FirstOrDefault();

            Item bio = Inventory.Items
                       .Where(c => c.Name.Contains("Bio-Comminutor"))
                       .FirstOrDefault();

            if (parts != null && bio != null && Time.AONormalTime > _monsterDelay + .35)
            {
                bio.CombineWith(parts);

                _monsterDelay = Time.AONormalTime;
            }
        }
    }
}
