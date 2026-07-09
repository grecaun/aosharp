using System;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Common.GameData;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using AOSharp.Core.Inventory;
using System.Linq;
using AOSharp.Common.GameData.UI;

namespace CrystalFilledbytheSource
{
    public class UtilityCrystalFilledbytheSource : AOPluginEntry
    {
        public Dictionary<string, CharacterSettings> CharSettings { get; set; }

        public static Config Config { get; private set; }

        public static Window _infoWindow;
        public static View _infoView;

        protected Settings _settings;

        public static string PluginDir;

        double delay = 0;
        bool OpenBags = false;

        [Obsolete]
        public override void Run(string pluginDir)
        {
            Config = Config.Load($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\AOSP\\CrystalFilledbytheSource\\{DynelManager.LocalPlayer.Name}\\Config.json");

            PluginDir = pluginDir;

            _settings = new Settings("CrystalFilledbytheSource");

            RegisterSettingsWindow("Crystal Filled by the Source", "CrystalFilledbytheSourceSettingWindow.xml");

            Game.OnUpdate += OnUpdate;

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;

            _settings.AddVariable("BoonofErgoSelection", (int)BoonofErgoSelection.AbanShere);

            Chat.WriteLine("Crystal Filled by the Source Loaded!");
            Chat.WriteLine("/source for settings.");
            Chat.WriteLine("Guide");
            Chat.WriteLine("https://www.ao-universe.com/guides/shadowlands/tradeskill-guides-5/general-crafting-4/pocket-boss-guide");
        }

        public override void Teardown()
        {
            SettingsController.CleanUp();
        }

        public Window[] _windows => new Window[] { };

        private void InfoView(object s, ButtonBase button)
        {
            _infoWindow = Window.CreateFromXml("INSTRUCTIONS", PluginDir + "\\UI\\InfoView.xml",
                windowSize: new Rect(0, 0, 440, 510),
                windowStyle: WindowStyle.Default,
                windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            _infoWindow.Show(true);
        }

        protected void RegisterSettingsWindow(string settingsName, string xmlName)
        {
            SettingsController.RegisterSettingsWindow(settingsName, PluginDir + "\\UI\\" + xmlName, _settings);
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if (_settings["Enable"].AsBool())
            {
                if (!OpenBags)
                {
                    HandleBags();
                    OpenBags = true;
                }

                var BagPack = Inventory.Backpacks.Where(bag => bag.Name.Contains("Crystals") && bag.Items.Count < 21).ToList();
                var tower = DynelManager.AllDynels.FirstOrDefault(t => t.Name.Contains("Control Tower"));
                var CrystalFilledbytheSource = Inventory.Items.Find(crystal => crystal.Name == "Crystal Filled by the Source");
                var UnpollutedCrystal = Inventory.Items.Find(crystal => crystal.Name == "Unpolluted Crystal");
                var BoonofErgo = Spell.List.FirstOrDefault(spell => spell.Id == _settings["BoonofErgoSelection"].AsInt32());

                if (Time.AONormalTime > delay)
                {
                    if (CrystalFilledbytheSource != null)
                    {
                        if (BagPack.Any())
                        {
                            foreach (Item FilledCrystal in Inventory.Items.Where(c => c.Slot.Type == IdentityType.Inventory))
                            {
                                if (FilledCrystal.Name == "Crystal Filled by the Source")
                                {
                                    Chat.WriteLine($"Moving {FilledCrystal.Name} to {BagPack.FirstOrDefault().Name}");
                                    FilledCrystal.MoveToContainer(BagPack.FirstOrDefault());
                                }
                            }
                        }
                        else
                        {
                            _settings["Enable"] = false;
                        }
                    }
                    else
                    {
                        if (UnpollutedCrystal != null)
                        {
                            foreach (Item EmptyCrystal in Inventory.Items)
                            {
                                if (EmptyCrystal.Name == "Unpolluted Crystal")
                                {
                                    if (tower != null && DynelManager.LocalPlayer.Position.DistanceFrom(tower.Position) < 10)
                                    {
                                        if (Targeting.Target != tower)
                                        {
                                            Targeting.SetTarget(tower.Identity);
                                        }
                                        else
                                        {
                                            if (EmptyCrystal.QualityLevel <= 250)
                                            {
                                                if (EmptyCrystal.QualityLevel <= tower.GetStat(Stat.Level))
                                                {
                                                    EmptyCrystal.Use();
                                                }
                                            }
                                            else
                                            {
                                                if (tower.GetStat(Stat.Level) >= 255)
                                                {
                                                    EmptyCrystal.Use();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (BoonofErgo != null)
                            {
                                if (!Spell.HasPendingCast && BoonofErgo.IsReady && DynelManager.LocalPlayer.Nano >= BoonofErgo.Cost
                                    && DynelManager.LocalPlayer.MovementState != MovementState.Sit)
                                {
                                    BoonofErgo?.Cast();
                                }
                            }
                        }
                    }
                    delay = Time.AONormalTime + 1;
                }
            }
            else
            { 
                if (OpenBags)
                {
                    OpenBags = false;
                }
            }

            if (SettingsController.settingsWindow != null && SettingsController.settingsWindow.IsValid)
            {
                if (SettingsController.settingsWindow.FindView("InfoView", out Button infoView))
                {
                    infoView.Tag = SettingsController.settingsWindow;
                    infoView.Clicked = InfoView;
                }
            }
        }

        enum BoonofErgoSelection
        {
            AbanShere = 229922,
            OcraBhotaar = 229926,
            OcraThar = 229928,
            OcraRoch = 229930,
            OcraXum = 229932,
            OcraShere = 229934,
            EnelBhotaar = 229936,
            EnelThar = 229938,
            EnelRoch = 229940,
            EnelXum = 229942,
        }

        void HandleBags()
        {
            foreach (var Bag in Inventory.Backpacks.Where(b => b.Name.Contains("Crystals")))
            {
                foreach (var BagtoOpen in Inventory.Items.Where(c => c.Slot == Bag.Slot))
                {
                    BagtoOpen.Use();
                }
            }
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
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\AOSP\\CrystalFilledbytheSource\\{DynelManager.LocalPlayer.Name}"))
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\AOSP\\CrystalFilledbytheSource\\{DynelManager.LocalPlayer.Name}");

            File.WriteAllText(_path, JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
        }
    }

    public class CharacterSettings
    {

    }
}

