using AOSharp.Core;
using AOSharp.Core.UI;
using System;
 
namespace MalisDungeonMap2
{
    public class DungeonMap : AOPluginEntry
    {
        public static string PluginDir;
        private Config _config;
        private MainWindow _window;

        public override void Run(string pluginDir)
        {
            Chat.WriteLine("- Mali's Dungeon Map 2.0 -\n" +
                "Note: If map is not rendering, change graphic setting in launcher to 'Direct 3D T&L HAL'\n" +
                "/mapsettings");

            PluginDir = pluginDir;
            DungeonMapRenderer.Start();
            _config = Config.TryLoad($"{pluginDir}\\config.json");

            DungeonMapRenderer.MapConfig = _config.MapConfig;
            if (_config.MapConfig.ShowConfigOnStartup)
                ToggleWindow();

            Chat.RegisterCommand("mapsettings", (string command, string[] param, ChatWindow chatWindow) =>
            {
                ToggleWindow();
            });
        }

        public void ToggleWindow()
        {
            try
            {
                if (_window?.Window != null)
                {
                    _window.Dispose();
                }
                else
                {
                    _window = new MainWindow("Mali's Dungeon Map 2", $"{PluginDir}\\UI\\MainWindow.xml", $"{PluginDir}\\UI\\ColorView.xml", $"{PluginDir}\\UI\\EntryView.xml", _config);
                    _window.Activate();
                }
            }
            catch (Exception ex)
            {
                Chat.WriteLine(ex.Message); 
            }
        }
    }
}