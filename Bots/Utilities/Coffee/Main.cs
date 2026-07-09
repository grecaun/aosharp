using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using AOSharp.Common.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.GameData;

namespace Coffee
{
    public class Main : AOPluginEntry
    {
        public static bool Toggle = false;

        public static double _timer = 0f;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("Coffee loaded!");
                Chat.WriteLine("/coffee for toggle.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("coffee", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Toggle = !Toggle;
                    Chat.WriteLine($"Coffee : {Toggle}");
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
                Item coffee = Inventory.Items.Where(c => c.Name == "Miyashiro Superior Enhanced Coffee Machine").FirstOrDefault();

                List<Item> list = Inventory.Items.FindAll((Item x) => x.Name == "Steaming Hot Cup of Enhanced Coffee");

                if (list?.Count > 1)
                {
                    CharacterActionMessage characterActionMessage = new CharacterActionMessage();
                    characterActionMessage.Action = (CharacterActionType)53;
                    characterActionMessage.Target = list[1].Slot;
                    Identity slot = list[0].Slot;
                    characterActionMessage.Parameter1 = (int)slot.Type;
                    slot = list[0].Slot;
                    characterActionMessage.Parameter2 = slot.Instance;
                    Network.Send(characterActionMessage);
                }
                else
                {
                    if (Time.AONormalTime > _timer + 2.0 && !DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.ElectricalEngineering))
                    {
                        if (coffee != null)
                        {
                            coffee.Use(null, false);
                            _timer = Time.AONormalTime;
                        }
                    }
                }
            }
        }

        public override void Teardown()
        {
        }
    }
}
