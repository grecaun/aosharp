using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using AutomatonMitaar.IPCMessages;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace AutomatonMitaar
{
    public class AutomatonMitaar : AOPluginEntry
    {
        public const string PluginName = "AutomatonMitaar";
        private const string Version_Number = "2.0.0";
        public static int Counter = 0;

        public static StateMachine _stateMachine;
        public static NavMeshMovementController NavMeshMovementController { get; private set; }
        public static IPCChannel IPCChannel { get; private set; }

        public static Identity Leader = Identity.None;
        private bool _leader = false;

        public static double _stateTimeOut;

        public static Settings _settings;

        private List<Settings> settingsToSave = new List<Settings>();

        Window _mainWindow;
        Window _kittingWindow;
        Window _infoWindow;

        public static string EnableString = "Enable";
        public static string TeamButtonState = "Team";

        double SendDelay;

        public static readonly Shared.Kits kitsInstance = new Shared.Kits();

        public static readonly IdleState Idle = new IdleState();
        public static readonly FightState Fight = new FightState();
        public static readonly DiedState Died = new DiedState();
        public static readonly EnterState Enter = new EnterState();
        public static readonly SoloState Solo = new SoloState();
        public static readonly FarmingState Loot = new FarmingState();
        public static readonly ReformState Reform = new ReformState();

        private static readonly Random _rand = new Random();

        public override void Run()
        {
            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            _settings = new Settings(PluginName);

            NavMeshMovementController = new NavMeshMovementController($"{PluginDirectory}\\NavMeshes", true);
            MovementController.Set(NavMeshMovementController);

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;

            _settings.AddVariable("Tank", false);
            _settings.AddVariable("Solo", false);

            _settings.AddVariable("Farming", false);
            _settings.AddVariable("StopAttack", false);
            _settings.AddVariable("Red", false);
            _settings.AddVariable("Blue", false);
            _settings.AddVariable("Yellow", false);
            _settings.AddVariable("Green", false);

            _settings.AddVariable("IPCChannel", 48);

            _settings.AddVariable("KitNanoPercentageBox", 90);
            _settings.AddVariable("KitHealthPercentageBox", 90);

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            settingsToSave.Add(_settings);

            IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

            IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, OnStartStopMessage);
            IPCChannel.RegisterCallback((int)IPCOpcode.SettingsUpdate, OnSettingsUpdateMessage);
            IPCChannel.RegisterCallback((int)IPCOpcode.LeaderInfo, OnLeaderInfoMessage);
            IPCChannel.RegisterCallback((int)IPCOpcode.Team, LocalTeamMessageReceived);

            Chat.RegisterCommand("enable", EnableCommand);
            Chat.RegisterCommand("buddy", BuddyCommand);
            Chat.RegisterCommand("buddychannel", ChannelCommand);
            Chat.RegisterCommand("form", FormCommand);

            _stateMachine = new StateMachine(Idle);

            Game.OnUpdate += OnUpdate;
            Network.N3MessageSent += N3MessageSent;
            UIController.WindowDeleted += Windowclosed;
            Team.TeamRequest += OnTeamRequest;

            Chat.WriteLine($"{PluginName} Loaded!");
            Chat.WriteLine("/buddy to open or close the ui. /enable to start or stop.");

        }

        #region Received IPC Msgs

        private void OnStartStopMessage(int sender, IPCMessage msg)
        {
            if (!(msg is StartStopIPCMessage startStopMessage)) return;
            if (startStopMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            _settings["Enable"] = startStopMessage.IsStarting;

            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            if (_settings["Enable"].AsBool())
                Start();
            else
                Stop();
        }

        private void OnSettingsUpdateMessage(int sender, IPCMessage msg)
        {
            if (!(msg is SettingsUpdateMessage settingsUpdateMessage)) return;

            _settings["StopAttack"] = settingsUpdateMessage.StopAttack;
            _settings["Red"] = settingsUpdateMessage.Red;
            _settings["Blue"] = settingsUpdateMessage.Blue;
            _settings["Yellow"] = settingsUpdateMessage.Yellow;
            _settings["Green"] = settingsUpdateMessage.Green;
            _settings["Farming"] = settingsUpdateMessage.Farming;

            Save();
        }

        private void OnLeaderInfoMessage(int sender, IPCMessage msg)
        {
            if (!(msg is LeaderInfoIPCMessage leaderInfoMessage)) return;
            if (leaderInfoMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            if (leaderInfoMessage.LeaderChanger && _settings["Tank"].AsBool())
            {
                _settings["Tank"] = false;
                _leader = false;
                Leader = leaderInfoMessage.LeaderIdentity;
            }
            else
            {
                if (_settings["Tank"].AsBool())
                    IPCChannel.Broadcast(new LeaderInfoIPCMessage { LeaderIdentity = DynelManager.LocalPlayer.Identity, Sender = DynelManager.LocalPlayer.Identity });
                else
                    Leader = leaderInfoMessage.LeaderIdentity;
            }
        }

        public static void LocalTeamMessageReceived(int sender, IPCMessage msg)
        {
            if (!(msg is LocalTeamMessage teamMsg)) return;
            var LPID = DynelManager.LocalPlayer.Identity;
            if (teamMsg.Sender == LPID) return;
            switch (teamMsg.TeamAction)
            {
                case 0:
                    if (Team.IsInTeam) return;
                    IPCChannel.Broadcast(new LocalTeamMessage { Sender = LPID, Receiver = LPID, TeamAction = 3 });
                    break;
                case 2:
                    if (Team.IsInTeam)
                    {
                        Team.Leave();
                    }
                    break;
                case 3:
                    if (Leader != LPID) return;
                    Team.Invite(teamMsg.Receiver);
                    break;
            }
        }

        #endregion

        #region Subscriptions

        private void OnUpdate(object s, float deltaTime)
        {
            if (Game.IsZoning) { return; }

            var localPlayer = DynelManager.LocalPlayer;

            if (_mainWindow?.IsValid == true)
            {
                TeamButtonState = Team.IsInTeam ? "Disband" : "Team";

                if (_mainWindow.FindView("TeamButton", out Button teamButton))
                {
                    if (teamButton.Label != TeamButtonState)
                        teamButton.SetLabel(TeamButtonState);
                }

                if (_mainWindow.FindView("Counter", out TextView counter))
                {
                    if (counter.Text != Counter.ToString())
                        counter.Text = $"Run Count = {Counter}";
                }
            }

            Leader_Changed("Tank");

            if (Leader == Identity.None)
            {
                if (_settings["Tank"].AsBool())
                    Leader = DynelManager.LocalPlayer.Identity;
                else if (Time.AONormalTime > SendDelay)
                {
                    IPCChannel.Broadcast(new LeaderInfoIPCMessage { Sender = DynelManager.LocalPlayer.Identity });
                    SendDelay = Time.AONormalTime + 5;
                }
            }

            var _sinuh = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Technomaster Sinuh");

            if (_sinuh == null)
            {

                kitsInstance.SitAndUseKit(_settings["KitNanoPercentageBox"].AsInt32(), _settings["KitHealthPercentageBox"].AsInt32());
            }

            if (_settings["Enable"].AsBool())
                _stateMachine.Tick();
        }

        private void Windowclosed(object sender, Window e)
        {
            switch (e.Name)
            {
                case PluginName:
                    Window_Closed_helper();
                    break;
            }
        }

        private void OnTeamRequest(object sender, TeamRequestEventArgs e)
        {
            if (Game.IsZoning) return;
            if (Leader == Identity.None)
                Leader = e.Requester;

            e.Accept();
        }

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Identity != DynelManager.LocalPlayer.Identity) return;
            if (charAction.Action != CharacterActionType.Logout) return;

            Chat.WriteLine("N3MessageSent");

            _settings["Enable"] = false;
            EnableString = "Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Stop();
            Save();

            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            Team.TeamRequest -= OnTeamRequest;
            UIController.WindowDeleted -= Windowclosed;

            return;
        }

        #endregion

        #region Chat Commands

        private void EnableCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length < 1)
            {
                if (!Team.IsInTeam)
                    ToggleTeam();

                Handle_Enable();
                return;
            }
        }

        private void BuddyCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (_mainWindow?.IsValid == true)
            {
                Window_Closed_helper();

                _mainWindow.Close();
                _mainWindow = null;
                return;
            }

            _mainWindow = Window.CreateFromXml(PluginName, PluginDirectory + $"\\UI\\AutomatonMitaarMainWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

            if (_mainWindow.FindView("InfoWindow", out Button infoButton))
                infoButton.Clicked = Info_Button_Clicked;

            if (_mainWindow.FindView("Enable_Disable_Button", out Button enableBtn))
            {
                enableBtn.SetLabel(EnableString);
                enableBtn.Clicked = Enable_Disable_Button_Clicked;
            }

            if (_mainWindow.FindView("TeamButton", out Button teamButton))
            {
                teamButton.SetLabel(TeamButtonState);
                teamButton.Clicked = Team_Button_Clicked;
            }

            if (_mainWindow.FindView("KittingButton", out Button kittingButton))
                kittingButton.Clicked = Kitting_Button_Clicked;

            if (_mainWindow.FindView("BroadcastSettings", out Button settingsButton))
                settingsButton.Clicked = Broadcast_Settings_Button_Clicked;

            if (_mainWindow.FindView("Counter", out TextView counter))
                counter.Text = $"Run Count = {Counter}";

            if (_mainWindow.FindView("VersionNumber", out TextView version))
                version.Text = $"Version {Version_Number}";

            _mainWindow.Show(true);
        }

        private void ChannelCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (arg2 == null || arg2.Length == 0)
            {
                Chat.WriteLine($"Current IPC Channel: {_settings["IPCChannel"].AsInt32()}");
                return;
            }

            if (int.TryParse(arg2[0], out int newChannel))
            {
                if (newChannel < 1 || newChannel > 255)
                {
                    Chat.WriteLine("Invalid channel. Please enter a number between 1 and 255.");
                    return;
                }

                _settings["IPCChannel"] = newChannel;
                IPCChannel.SetChannelId(Convert.ToByte(_settings["IPCChannel"].AsInt32()));
                Chat.WriteLine($"IPC Channel set to: {_settings["IPCChannel"].AsInt32()}");
                Save();
            }
            else
            {
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
            }
        }

        public static void FormCommand(string command, string[] param, ChatWindow chatWindow)
        {
            ToggleTeam();
        }

        #endregion

        #region Buttons

        internal void Info_Button_Clicked(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
                return;
            }

            _infoWindow = Window.CreateFromXml("Info", PluginDirectory + $"\\UI\\AutomatonMitaarInfoWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            if (_infoWindow.FindView("PluginName", out TextView setText))
                setText.Text = PluginName;

            _infoWindow.Show(true);

        }

        internal void Enable_Disable_Button_Clicked(object s, ButtonBase button)
        {
            if (!Team.IsInTeam)
                ToggleTeam();

            Handle_Enable();
        }

        public static void Team_Button_Clicked(object sender, ButtonBase e)
        {
            ToggleTeam();
        }

        private void Kitting_Button_Clicked(object sender, ButtonBase e)
        {
            if (_kittingWindow?.IsValid == true)
            {
                _kittingWindow.Close();
                _kittingWindow = null;
                return;
            }

            _kittingWindow = Window.CreateFromXml("Kitting", PluginDirectory + "\\UI\\KittingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            if (_kittingWindow.FindView("KitHealthPercentageBox", out TextInputView healthBox))
                healthBox.Text = $"{_settings["KitHealthPercentageBox"].AsInt32()}";

            if (_kittingWindow.FindView("KitNanoPercentageBox", out TextInputView nanoBox))
                nanoBox.Text = $"{_settings["KitNanoPercentageBox"].AsInt32()}";

            if (_kittingWindow.FindView("Save_Button", out Button saveButton))
                saveButton.Clicked = Save_Button_Clicked;

            _kittingWindow.Show(true);
        }

        private void Save_Button_Clicked(object sender, ButtonBase e)
        {
            if (_kittingWindow?.IsValid == true)
            {
                if (_kittingWindow.FindView("KitHealthPercentageBox", out TextInputView kitHealthInput) &&
                int.TryParse(kitHealthInput.Text, out int kitHealthValue))
                {
                    if (_settings["KitHealthPercentageBox"].AsInt32() != kitHealthValue)
                        _settings["KitHealthPercentageBox"] = kitHealthValue;
                }

                if (_kittingWindow.FindView("KitNanoPercentageBox", out TextInputView kitNanoInput) &&
                    int.TryParse(kitNanoInput.Text, out int kitNanoValue))
                {
                    if (_settings["KitNanoPercentageBox"].AsInt32() != kitNanoValue)
                        _settings["KitNanoPercentageBox"] = kitNanoValue;
                }

                Save();
            }
        }
        private void Broadcast_Settings_Button_Clicked(object sender, ButtonBase e)
        {
            IPCChannel.Broadcast(new SettingsUpdateMessage()
            {
                StopAttack = _settings["StopAttack"].AsBool(),
                Red = _settings["Red"].AsBool(),
                Blue = _settings["Blue"].AsBool(),
                Yellow = _settings["Yellow"].AsBool(),
                Green = _settings["Green"].AsBool(),
                Farming = _settings["Farming"].AsBool(),
            });

            Save();
        }

        #endregion

        #region Misc.

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            Team.TeamRequest -= OnTeamRequest;
            _mainWindow?.Close();
            _infoWindow?.Close();
        }

        public static void Start()
        {
            Chat.WriteLine($"{PluginName} Enabled.");

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(Idle);
        }

        private void Stop()
        {
            Chat.WriteLine($"{PluginName} Disabled.");

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(Idle);

            if (NavMeshMovementController.IsNavigating)
                NavMeshMovementController.Halt();
        }

        private void Window_Closed_helper()
        {
            if (_mainWindow?.IsValid == true)
            {
                Rect frame = _mainWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }


        void Handle_Enable()
        {
            _settings["Enable"] = !_settings["Enable"].AsBool();
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = _settings["Enable"].AsBool(), Sender = DynelManager.LocalPlayer.Identity });

            if (_settings["Enable"].AsBool())
                Start();
            else
                Stop();

            return;
        }

        public void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        public static void ToggleTeam()
        {
            int action = Team.IsInTeam ? 2 : 0;

            IPCChannel.Broadcast(new LocalTeamMessage
            {
                Sender = DynelManager.LocalPlayer.Identity,
                TeamAction = action,
                Receiver = Identity.None,
            });
        }

        public static float Rand(float min, float max)
        {
            return (float)(_rand.NextDouble() * (max - min) + min);
        }

        public static class SpiritNanos
        {
            public const int BlessingofTheBlood = 280472; //Red
            public const int BlessingofTheSource = 280521; //Blue
            public const int BlessingofTheOutsider = 280493; //Green
            public const int BlessingofTheLight = 280496;  //Yellow
        }

        private void Leader_Changed(string settingName)
        {
            if (_settings[settingName].AsBool() != _leader)
            {
                IPCChannel.Broadcast(new LeaderInfoIPCMessage { Sender = DynelManager.LocalPlayer.Identity, LeaderIdentity = DynelManager.LocalPlayer.Identity, LeaderChanger = true });
                Leader = DynelManager.LocalPlayer.Identity;
                _leader = true;
            }
        }

        #endregion
    }
}
