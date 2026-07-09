using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Inventory;
using AOSharp.Common.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.GameData;

namespace Cookies
{
    public class Main : AOPluginEntry
    {
        public static bool Toggle = false;

        public static double _timer = 0f;
        public static int _counter = 0;

        public override void Run()
        {
            try
            {
                Chat.WriteLine("Cookies loaded!");
                Chat.WriteLine("/cookies for toggle.");
                Chat.WriteLine("Name bags 'Cookies' for placing into when inventory is full.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("cookies", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    Toggle = !Toggle;
                    Chat.WriteLine($"Cookies : {Toggle}");

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
                Item extruder = Inventory.Items.Where(c => c.Name == "The Extruder").FirstOrDefault();

                List<Item> list = Inventory.Items.FindAll((Item x) => x.Name == "Extruder Nutrition Bar");

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
                    _counter++;
                    return;
                }
                if (_counter >= 50)
                {
                    Container _bag = Inventory.Backpacks.FirstOrDefault(c => c.IsOpen && c.Items.Count < 21 && c.Name == "Cookies");

                    if (_bag != null)
                    {
                        list[0].MoveToContainer(_bag);
                        _counter = 0;
                    }
                }
                else
                {
                    if (Time.AONormalTime > _timer + 20.0 && !DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.ElectricalEngineering))
                    {
                        if (extruder != null)
                        {
                            extruder.Use(null, false);
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
