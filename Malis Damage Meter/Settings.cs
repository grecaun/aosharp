using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using System.IO;

namespace MalisDamageMeter
{
    public class Settings
    {
        public Vector2 Frame;
        public bool AutoToggleTimer;
        public bool LogMobs;
        public bool TotalValues;
        public bool ShowTutorial;
        public bool TotalPerMinute;
        public ScopeEnum Scope;

        public void Save()
        {
            AutoToggleTimer = DamageMeter.Window.ViewSettings.AutoToggleTimer;
            LogMobs = DamageMeter.Window.ViewSettings.LogMobs;
            TotalValues = DamageMeter.Window.ViewSettings.TotalValues;
            Frame.X = DamageMeter.Window.Window.GetFrame().MinX;
            Frame.Y = DamageMeter.Window.Window.GetFrame().MinY;
            TotalPerMinute = DamageMeter.Window.ViewSettings.TotalPerMinute;

            if (ShowTutorial)
                ShowTutorial = false;

            File.WriteAllText($"{DamageMeter.PluginDir}\\JSON\\Settings.json", JsonConvert.SerializeObject(this));
        }

        public static Settings Load(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
            }
            catch
            {
                Chat.WriteLine($"Config file can't be loaded.");
                return null;
            }
        }
    }
}