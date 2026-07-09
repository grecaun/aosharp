using System;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using AOSharp.Common.GameData;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System.Collections.Generic;

namespace ICE
{
    public class Main : AOPluginEntry
    {
        public static bool Enable = false;
        public static double _timer;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("ICE loaded!");
                Chat.WriteLine("/ICE to enable.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("ICE", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Enable = !Enable;
                    Chat.WriteLine($"ICE : {Enable}");
                });

                foreach (var bag in Inventory.Items.Where(b => b.UniqueIdentity.Type == IdentityType.Container))
                {
                    bag.Use();
                    bag.Use();
                }
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if (Enable)
            {
                var NanoProgrammingInterface = Inventory.Items.Where(c => c.Name == "Nano Programming Interface").FirstOrDefault();

                if (Time.AONormalTime > _timer )
                {
                    List<Item> UpgradedControllerRecompilerUnit = Inventory.Items.FindAll((Item x) => x.Name == "Upgraded Controller Recompiler Unit");

                    if (UpgradedControllerRecompilerUnit?.Count > 1)
                    {
                        CharacterActionMessage characterActionMessage = new CharacterActionMessage();
                        characterActionMessage.Action = (CharacterActionType)53;
                        characterActionMessage.Target = UpgradedControllerRecompilerUnit[1].Slot;
                        Identity slot = UpgradedControllerRecompilerUnit[0].Slot;
                        characterActionMessage.Parameter1 = (int)slot.Type;
                        slot = UpgradedControllerRecompilerUnit[0].Slot;
                        characterActionMessage.Parameter2 = slot.Instance;
                        Network.Send(characterActionMessage);
                    }
                    else
                    {
                        var ICEBreaker = Inventory.Items.Where(c => c.Name == "Hacker ICE-Breaker Source");

                        if (ICEBreaker != null)
                        {
                            foreach (var ice in Inventory.Items.Where(c => c.Name == "Hacker ICE-Breaker Source"))
                            {
                                NanoProgrammingInterface?.UseOn(ice.Slot);
                            }
                        }
                        else
                        {
                            foreach (var bag in Inventory.Backpacks)
                            {
                                foreach (var item in bag.Items)
                                {
                                    if (item.Name == "Hacker ICE-Breaker Source")
                                    {
                                        item.MoveToInventory();
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    _timer = Time.AONormalTime +0.3;
                }
            }
        }

        public override void Teardown()
        {
        }
    }
}
