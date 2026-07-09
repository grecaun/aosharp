using System;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Common.GameData;
using System.Collections.Generic;
using AOSharp.Common.GameData.UI;
using System.IO;
using Newtonsoft.Json;
using AOSharp.Core.Inventory;
using System.Linq;

namespace AutoItemLevel
{
    public class UtilityAutoItemLevel : AOPluginEntry
    {
        public Dictionary<string, CharacterSettings> CharSettings { get; set; }

        static double Delay;
        static double morphingMemoryDelay;
        static double pistolDelay;

        //bool leftArmEquipped = false;
        //bool rightArmEquipped = false;
        //bool nextArmIsLeft = true;

        public static Config Config { get; private set; }

        public static Window _infoWindow;
        public static View _infoView;

        protected Settings _settings;


        public override void Run()
        {
            Config = Config.Load($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\AOSP\\AutoItemLevel\\{DynelManager.LocalPlayer.Name}\\Config.json");

            _settings = new Settings("AutoItemLevel");

            RegisterSettingsWindow("Auto Item", "AutoItemLevelSettingWindow.xml");

            Game.OnUpdate += OnUpdate;

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;

            _settings.AddVariable("Newcomer", false);
            _settings.AddVariable("MorphingMemory", false);
            _settings.AddVariable("EngiePistol", false);

            Chat.WriteLine("AutoItemLevel Loaded!");
            Chat.WriteLine("/autoitem for settings.");
        }

        public override void Teardown()
        {
            SettingsController.CleanUp();
        }

        public Window[] _windows => new Window[] { };

        private void InfoView(object s, ButtonBase button)
        {
            _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\AutoItemLevelInfoView.xml",
                windowSize: new Rect(0, 0, 440, 510),
                windowStyle: WindowStyle.Default,
                windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            _infoWindow.Show(true);
        }

        protected void RegisterSettingsWindow(string settingsName, string xmlName)
        {
            SettingsController.RegisterSettingsWindow(settingsName, PluginDirectory + "\\UI\\" + xmlName, _settings);
        }

        private void OnUpdate(object s, float deltaTime)
        {
            var player = DynelManager.LocalPlayer;

            if (Time.AONormalTime > Delay + 0.5)
            {
                if (_settings["Newcomer"].AsBool())
                {

                    var NewcomerArmor = Inventory.Items.FirstOrDefault(i => i.Name.Contains("Newcomer"));
                    var inventoryNewcomerArmor = Inventory.Items.FirstOrDefault(i => i.Name.Contains("Newcomer") && i.Slot.Type == IdentityType.Inventory);
                    var equippedNewcomerArmor = Inventory.Items.FirstOrDefault(i => i.Name.Contains("Newcomer") && i.Slot.Type == IdentityType.ArmorPage);
                    var outOfLevelNewcomerArmor = Inventory.Items.FirstOrDefault(i => i.Name.Contains("Newcomer") && i.QualityLevel < player.Level);

                    if (NewcomerArmor == null) { return; }

                    if (inventoryNewcomerArmor != null)
                    {
                        if (inventoryNewcomerArmor.QualityLevel == player.Level)
                        {
                            EquipSlot targetSlot = GetTargetSlot(inventoryNewcomerArmor);

                            if (targetSlot != EquipSlot.Cloth_Back)
                            {
                                inventoryNewcomerArmor.Equip(targetSlot);
                            }
                        }
                        else if (inventoryNewcomerArmor.QualityLevel < player.Level)
                        {
                            inventoryNewcomerArmor.Use();
                        }
                    }
                    else
                    {
                        outOfLevelNewcomerArmor?.MoveToInventory();
                    }

                    //int playerLevel = DynelManager.LocalPlayer.Level;

                    //foreach (Item item in Inventory.Items)
                    //{
                    //    if (item.Name.Contains("Newcomer"))
                    //    {
                            
                    //        if (item.QualityLevel < playerLevel)
                    //        {
                                
                    //            if (item.Slot.Type != IdentityType.Inventory)
                    //            {
                    //                item.MoveToInventory();
                    //            }

                                
                    //            if (item.Slot.Type == IdentityType.Inventory)
                    //            {
                    //                item.Use(); 
                    //            }
                    //        }

                    //        Identity leftArmIdentity = new Identity(IdentityType.ArmorPage, (int)EquipSlot.Cloth_LeftArm);
                    //        List<EquipSlot> equipSlots = item.EquipSlots;

                            
                    //        if (item.QualityLevel == playerLevel && item.Slot.Type != IdentityType.ArmorPage)
                    //        {
                                
                    //            if (!Inventory.Find(leftArmIdentity, out Item item1) && item.Name.Contains("Sleeve"))
                    //            {
                    //                item.Equip(EquipSlot.Cloth_LeftArm);
                    //            }
                    //            else
                    //            {
                    //                foreach (EquipSlot equipSlot in item.EquipSlots)
                    //                {
                    //                    item.Equip(equipSlot);
                    //                    break;  
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

                if (_settings["MorphingMemory"].AsBool())
                {
                    int playerPsychic = DynelManager.LocalPlayer.GetStat(Stat.Psychic) - 2;
                    int playerComputerLiteracy = DynelManager.LocalPlayer.GetStat(Stat.ComputerLiteracy) - 2;

                    foreach (Item morphingMemory in Inventory.Items)
                    {
                        if (morphingMemory.Name == "Morphing Memory")
                        {
                            //for (int i = 0; i < 20; i++)
                            //    Chat.WriteLine($"{item.GetReqs((ItemActionInfo)i).Count} {i}");

                            var memoryReq = morphingMemory.GetReqs((ItemActionInfo)8);
                            int psychic = memoryReq[1].Param2 + 1;
                            int compLit = memoryReq[0].Param2 + 1;

                            //Chat.WriteLine($"{(Stat)memoryReq[0].Param1}, {memoryReq[0].Param2}, {memoryReq[0].Operator}");
                            //Chat.WriteLine($"{(Stat)memoryReq[1].Param1}, {memoryReq[1].Param2}, {memoryReq[1].Operator}");
                            //Chat.WriteLine($"Psychic: {psychic}, Computer Literacy {compLit}");
                            //Chat.WriteLine($"playerComputerLiteracy: {playerComputerLiteracy}, playerPsychic: {playerPsychic}");

                            if (psychic < playerPsychic && compLit < playerComputerLiteracy)
                            {
                                if (morphingMemory.Slot.Type != IdentityType.Inventory)
                                {
                                    if (Time.AONormalTime > morphingMemoryDelay + 12)
                                    {
                                        morphingMemory.MoveToInventory();
                                        Chat.WriteLine($"Moving {morphingMemory.Name} to inventory");
                                        morphingMemoryDelay = Time.AONormalTime;
                                    }
                                }
                                else
                                {
                                    morphingMemory.Use();
                                }
                            }

                            List<EquipSlot> equipSlots = morphingMemory.EquipSlots;

                            if (psychic == playerPsychic || compLit == playerComputerLiteracy)
                            {
                                if (morphingMemory.Slot.Type == IdentityType.Inventory)
                                {
                                    if (Time.AONormalTime > morphingMemoryDelay + 12)
                                    {
                                        foreach (EquipSlot equipSlot in morphingMemory.EquipSlots)
                                        {
                                            Chat.WriteLine($"Equiping {morphingMemory.Name} to slot {equipSlot}");
                                            morphingMemory.Equip(equipSlot);
                                            break;
                                        }

                                        morphingMemoryDelay = Time.AONormalTime;
                                    }
                                }
                            }
                        }
                    }
                }

                #region UI

                if (SettingsController.settingsWindow != null && SettingsController.settingsWindow.IsValid)
                {
                    // SettingsController.settingsWindow.FindView("ChannelBox", out TextInputView channelInput);

                    if (SettingsController.settingsWindow.FindView("AutoItemLevelInfoView", out Button infoView))
                    {
                        infoView.Tag = SettingsController.settingsWindow;
                        infoView.Clicked = InfoView;
                    }
                }

                #endregion

                Delay = Time.AONormalTime;
            }

            if (Time.AONormalTime > pistolDelay + 2.3f)
            {
                if (_settings["EngiePistol"].AsBool())
                {
                    int playerWeaponSmithing = DynelManager.LocalPlayer.GetStat(Stat.WeaponSmithing) - 3;
                    int playerPistolSkill = DynelManager.LocalPlayer.GetStat(Stat.Pistol) - 4;

                    foreach (Item pistol in Inventory.Items)
                    {
                        if (pistol.Name.Contains("Solar-Powered"))
                        {
                            var weaponSmithingyReq = pistol.GetReqs((ItemActionInfo)3);
                            int weaponSmithing = weaponSmithingyReq[0].Param2 + 1;
                            var pistolReq = pistol.GetReqs((ItemActionInfo)8);
                            int pistolSkill = pistolReq[0].Param2 + 1;

                            if (weaponSmithing < playerWeaponSmithing && pistolSkill < playerPistolSkill)
                            {
                                if (pistol.Slot.Type != IdentityType.Inventory)
                                {
                                    pistol.MoveToInventory();
                                    Chat.WriteLine($"Moving {pistol.Name} to inventory");
                                }
                                else
                                {
                                    pistol.Use();
                                }
                            }

                            List<EquipSlot> equipSlots = pistol.EquipSlots;

                            if (pistolSkill == playerPistolSkill || weaponSmithing >= playerWeaponSmithing)
                            {
                                if (pistol.Slot.Type == IdentityType.Inventory)
                                {
                                    foreach (EquipSlot equipSlot in pistol.EquipSlots)
                                    {
                                        Chat.WriteLine($"Equiping {pistol.Name} to slot {equipSlot}");
                                        pistol.Equip(equipSlot);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                pistolDelay = Time.AONormalTime;
            }
        }

        private static EquipSlot GetTargetSlot(Item item)
        {
            if (item.Name.Contains("Sleeve"))
            {
                return Inventory.Find(new Identity(IdentityType.ArmorPage, (int)EquipSlot.Cloth_LeftArm), out Item _) ? EquipSlot.Cloth_RightArm : EquipSlot.Cloth_LeftArm;
            }
            else if (item.Name.Contains("Body")) return EquipSlot.Cloth_Body;
            else if (item.Name.Contains("Boots")) return EquipSlot.Cloth_Feet;
            else if (item.Name.Contains("Gloves")) return EquipSlot.Cloth_Hands;
            else if (item.Name.Contains("Pants")) return EquipSlot.Cloth_Legs;
            else return EquipSlot.Cloth_Back;
        }

        private Dictionary<string, (int min, int max)> EngiePistols()
        {
            var pistols = new Dictionary<string, (int min, int max)>()
            {
                {"Solar-Powered Tinker Pistol", (1, 39)},
                {"Solar-Powered Mender Pistol", (40, 99)},
                {"Solar-Powered Mechanic Pistol", (100, 139)},
                {"Solar-Powered Machinist Pistol", (140, 179)},
                {"Solar-Powered Engineer Pistol", (180, 199)},
                {"Solar-Powered Master Engineer Pistol", (200, 200)}
            };

            return pistols;
        }
    }

    public class Config
    {
        public Dictionary<string, CharacterSettings> CharSettings { get; set; }

        protected string _path;

        public static Config Load(string path)
        {
            Config config;

            try
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));

                config._path = path;
            }
            catch
            {
                Chat.WriteLine($"No config file found.");
                Chat.WriteLine($"Using default settings");

                config = new Config
                {
                    CharSettings = new Dictionary<string, CharacterSettings>()
                    {
                        { DynelManager.LocalPlayer.Name, new CharacterSettings() }
                    }
                };

                config._path = path;

                config.Save();
            }

            return config;
        }

        public void Save()
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\AOSP\\AutoItemLevel\\{DynelManager.LocalPlayer.Name}"))
            {
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\AOSP\\AutoItemLevel\\{DynelManager.LocalPlayer.Name}");
            }

            File.WriteAllText(_path, JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
        }
    }

    public class CharacterSettings
    {

    }
}

