using AOSharp.Core;
using System;
using AOSharp.Pathfinding;
using Serilog.Core;
using AOSharp.Core.UI;
using System.IO;
using System.Linq;
using AOSharp.Common.GameData;

namespace AutomatonRoamba
{
    public class AutomatonRoamba : AOPluginEntry
    {
        public static AutomatonRoambaSettings _settings;
        public static Logger Log;
        public static RoamStateMachine StateMachine;
        public static IPC Ipc;
        public static MainWindow MainWindow;
        public static PathEditorWindow PathEditorWindow;
        public static ConfigEditorWindow ConfigEditorWindow;

        public override void Run()
        {
            try
            {
                Logger.Information("Loaded!");

                SMovementController.Set();

                Log = Logger;

                CommonParameters.Init("AutomatonRoamba");   // Init plugin name to create all relative directories

                Directory.CreateDirectory(FilePath.PathFolderPath);
                Directory.CreateDirectory(FilePath.ConfigFolderPath);
                FilePath.Set(PluginDirectory);   // Set our file paths for various files

                _settings = AutomatonRoambaSettings.LoadConfig($"{CommonParameters.PlayerSettingsPath}"); // Per character config containing all UI related settings

                OpenMainWindow();

                Chat.RegisterCommand("AutomatonRoamba", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    OpenMainWindow();
                });

                var autoStart = _settings.CoreConfig.OnInjectEnable && _settings.PathEditorConfig?.SPath?.PlayfieldId == Playfield.ModelIdentity.Instance;
                StateMachine = new RoamStateMachine(autoStart);
                Ipc = new IPC(_settings.CoreConfig.ChannelId, _settings);
            }
            catch (Exception e)
            {
                Logger.Warning(e.ToString());
            }
        }

        public override void Teardown()
        {
            _settings.Save();
        }

        private void OpenMainWindow()
        {
            MainWindow = new MainWindow("AutomatonRoamba",
                $"{FilePath.WindowsRootDir}\\MainWindow.xml",
                $"{FilePath.WindowsRootDir}\\InfoWindow.xml",
                $"{FilePath.ViewsRootDir}\\BuddyCoreView.xml", 
                _settings);

            MainWindow.Show();
        }

        public static void SetPath(SPath path)
        {
            if (path.Waypoints.Count > 1 || (path.Waypoints.Count == 1 && Vector3.Distance(DynelManager.LocalPlayer.Position, path.Waypoints[0]) > 1))
            {
                SMovementController.SetPath(path, true);
            }
        }

        public static void SetPath(SPath path, ConfigEditorConfig config)
        {
            if (config.FollowTarget)
            {
                var charNames = config.FollowTargetName.Split('\n').Select(x => x.Trim());
                var followTarget = DynelManager.Players.FirstOrDefault(x => charNames.Any(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
                if (followTarget != null) { SMovementController.SetPath(path, followTarget.Position, false); }
            }
            else
            {
                SetPath(path);
            }
        }
    }
}
