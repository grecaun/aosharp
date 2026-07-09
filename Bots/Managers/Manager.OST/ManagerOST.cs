using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;

namespace ManagerOST
{
    public class ManagerOST : AOPluginEntry
    {
        public static StateMachine _stateMachine;
        public static IPCChannel IPCChannel { get; private set; }
        public static Config Config { get; private set; }

        public static Identity Leader = Identity.None;
        public static bool IsLeader = false;

        //public static string PluginDir;

        public static Window infoWindow;

        public static bool Toggle = false;

        public static bool SwitchForLastPos = false;
        public static bool MobsAllDead = false;
        public static bool Slam = false;
        public static bool Demo = true;

        public static int RespawnDelay;

        public double _refreshAbsorbTimer;

        public static Vector3 currentPos;
        public static Vector3 lastPos;
        public static List<Vector3> _waypoints = new List<Vector3>();

        public static Settings _settings;

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                Chat.WriteLine("OST Manager Loaded!");
                Chat.WriteLine("/buddy for settings. /enable to start and stop.");

                _settings = new Settings("ManagerOST");
                //PluginDir = pluginDir;

                Config = Config.Load($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\AOSharp\\ManagerOST\\{DynelManager.LocalPlayer.Name}\\Config.json");

                Config.CharSettings[DynelManager.LocalPlayer.Name].RespawnDelayChangedEvent += RespawnDelay_Changed;

                Chat.RegisterCommand("enable", BuddyCommand);

                SettingsController.RegisterSettingsWindow("ManagerOST", PluginDirectory + "\\UI\\ManagerOSTSettingWindow.xml", _settings);

                _stateMachine = new StateMachine(new IdleState());

                Game.OnUpdate += OnUpdate;

                _settings.AddVariable("Toggle", false);

                _settings.AddVariable("MongoSelection", (int)MongoSelection.Demolish);

                _settings["Toggle"] = false;

                RespawnDelay = Config.CharSettings[DynelManager.LocalPlayer.Name].RespawnDelay;
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        public override void Teardown()
        {
            SettingsController.CleanUp();
        }

        public static void RespawnDelay_Changed(object s, int e)
        {
            Config.CharSettings[DynelManager.LocalPlayer.Name].RespawnDelay = e;
            Config.Save();
        }

        private void Start()
        {
            Toggle = true;

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(new IdleState());
        }

        private void Stop()
        {
            Toggle = false;

            _settings["Toggle"] = false;

            _stateMachine.SetState(new IdleState());
        }
        private void AddPos()
        {
            Chat.WriteLine("Waypoint Added.");

            _waypoints.Add(DynelManager.LocalPlayer.Position);
        }

        private void InfoView(object s, ButtonBase button)
        {
            infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerOSTInfoView.xml",
                windowSize: new Rect(0, 0, 455, 345),
                windowStyle: WindowStyle.Default,
                windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            infoWindow.Show(true);
        }

        private void AddWaypoint(object s, ButtonBase button)
        {
            _waypoints.Add(DynelManager.LocalPlayer.Position);
        }

        private void ClearWaypoints(object s, ButtonBase button)
        {
            _waypoints.Clear();
        }


        private void OnUpdate(object s, float deltaTime)
        {
            if (Game.IsZoning)
                return;

            if (SettingsController.settingsWindow != null && SettingsController.settingsWindow.IsValid)
            {
                SettingsController.settingsWindow.FindView("RespawnDelayBox", out TextInputView _respawnDelayInput);

                // Use this as reference for all others good template for the event handlers
                if (int.TryParse(_respawnDelayInput.Text, out int _respawnDelayValue)
                    && Config.CharSettings[DynelManager.LocalPlayer.Name].RespawnDelay != _respawnDelayValue)
                {
                    Config.CharSettings[DynelManager.LocalPlayer.Name].RespawnDelay = _respawnDelayValue;
                    RespawnDelay = _respawnDelayValue;
                }

                if (SettingsController.settingsWindow.FindView("ManagerOSTInfoView", out Button infoView))
                {
                    infoView.Tag = SettingsController.settingsWindow;
                    infoView.Clicked = InfoView;
                }

                if (SettingsController.settingsWindow.FindView("AddWaypoint", out Button addBox))
                {
                    addBox.Tag = SettingsController.settingsWindow;
                    addBox.Clicked = AddWaypoint;
                }

                if (SettingsController.settingsWindow.FindView("ClearWaypoints", out Button clearBox))
                {
                    clearBox.Tag = SettingsController.settingsWindow;
                    clearBox.Clicked = ClearWaypoints;
                }

                if (!_settings["Toggle"].AsBool() && Toggle == true)
                {
                    Stop();
                    return;
                }
                if (_settings["Toggle"].AsBool() && Toggle == false)
                {
                    IsLeader = true;
                    Leader = DynelManager.LocalPlayer.Identity;

                    Start();
                    return;
                }
            }

            if (_waypoints.Count >= 1)
            {
                foreach (Vector3 pos in _waypoints)
                {
                    Debug.DrawSphere(pos, 0.2f, DebuggingColor.White);
                }
            }

            _stateMachine.Tick();

        }

        private void BuddyCommand(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {
                if (param.Length < 1)
                {
                    if (!_settings["Toggle"].AsBool() && !Toggle)
                    {
                        IsLeader = true;
                        Leader = DynelManager.LocalPlayer.Identity;

                        Start();
                        Chat.WriteLine("Bot enabled.");
                        return;
                    }
                    else if (_settings["Toggle"].AsBool() && Toggle)
                    {
                        Stop();
                        Chat.WriteLine("Bot disabled.");
                        return;
                    }
                }

                switch (param[0].ToLower())
                {
                    case "addpos":
                        _waypoints.Add(DynelManager.LocalPlayer.Position);
                        break;

                    default:
                        return;
                }
                Config.Save();
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        public enum MongoSelection
        {
            Slam, Demolish
        }
    }
}
