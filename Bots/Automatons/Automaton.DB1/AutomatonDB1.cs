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
using AutomatonDB1.IPCMessages;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace AutomatonDB1
{
    public class AutomatonDB1 : AOPluginEntry
    {
        public const string PluginName = "AutomatonDB1";
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

        public static Window _mainWindow;
        Window _kittingWindow;
        Window _infoWindow;

        public static string EnableString = "Enable";
        public static string TeamButtonState = "Team";

        double SendDelay;

        public static readonly Shared.Kits kitsInstance = new Shared.Kits();

        public static readonly EnterState Enter = new EnterState();
        public static readonly FarmingState Loot = new FarmingState();
        public static readonly FightState Fight = new FightState();
        public static readonly GetBuffState BuffState = new GetBuffState();
        public static readonly IdleState Idle = new IdleState();
        public static readonly ReformState Reform = new ReformState();
        public static readonly StartState Start_State = new StartState();
        public static readonly ClearBeforeEntering Clear = new ClearBeforeEntering();

        private static readonly Random _rand = new Random();

        public static List<Identity> _teamCache = new List<Identity>();

        public static double StartStamp;

        public static List<string> ErrorMessages = new List<string>();
        public static string PluginDir;

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                _settings = new Settings(PluginName);

                PluginDir = PluginDirectory;

                NavMeshMovementController = new NavMeshMovementController($"{PluginDirectory}\\NavMeshes", true);
                MovementController.Set(NavMeshMovementController);

                _settings.AddVariable("Enable", false);
                _settings["Enable"] = false;

                _settings.AddVariable("Tank", false);
                _settings.AddVariable("Farming", false);

                _settings.AddVariable("IPCChannel", 48);

                _settings.AddVariable("KitNanoPercentageBox", 90);
                _settings.AddVariable("KitHealthPercentageBox", 90);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                _settings.AddVariable("LastRunTime", 0f);

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
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
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
            try
            {
                if (Game.IsZoning) { return; }

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

                kitsInstance.SitAndUseKit(_settings["KitNanoPercentageBox"].AsInt32(), _settings["KitHealthPercentageBox"].AsInt32());

                if (_settings["Enable"].AsBool())
                    _stateMachine.Tick();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
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
            if (charAction.Action != CharacterActionType.Logout) return;

            _settings["Enable"] = false;
            EnableString = "Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Stop();
            Save();

            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            Team.TeamRequest -= OnTeamRequest;

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
            else
            {
                _mainWindow = Window.CreateFromXml(PluginName, PluginDirectory + $"\\UI\\{PluginName}MainWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                _mainWindow.Show(true);

                if (_mainWindow.FindView("InfoButton", out Button infoButton))
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

                if (_mainWindow.FindView("Errors", out View errorView))
                    PopulateErrorView(errorView);

                if (_mainWindow.FindView("Counter", out TextView counter))
                    counter.Text = $"Run Count = {Counter}";

                if (_mainWindow.FindView("LastRunTime", out TextView lastRunTime))
                {
                    int minutes = (int)(_settings["LastRunTime"].AsFloat() / 60);
                    int seconds = (int)(_settings["LastRunTime"].AsFloat() % 60);
                    lastRunTime.Text = $"Last run time = {minutes}m {seconds}s";
                }

                if (_mainWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";
            }
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
            }
            else
            {
                _infoWindow = Window.CreateFromXml("Info", PluginDirectory + $"\\UI\\{PluginName}InfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade );
                _infoWindow.Show(true);

                if (_infoWindow.FindView("PluginName", out TextView setText))
                    setText.Text = PluginName;
            }
        }

        internal void Enable_Disable_Button_Clicked(object s, ButtonBase button)
        {
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
            }
            else
            {
                _kittingWindow = Window.CreateFromXml("Kitting", PluginDirectory + "\\UI\\KittingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

                if (_kittingWindow.FindView("KitHealthPercentageBox", out TextInputView healthBox))
                    healthBox.Text = $"{_settings["KitHealthPercentageBox"].AsInt32()}";

                if (_kittingWindow.FindView("KitNanoPercentageBox", out TextInputView nanoBox))
                    nanoBox.Text = $"{_settings["KitNanoPercentageBox"].AsInt32()}";

                if (_kittingWindow.FindView("Save_Button", out Button saveButton))
                    saveButton.Clicked = Save_Button_Clicked;

                _kittingWindow.Show(true);
            }
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

        public static class Nanos
        {
            public const int ThriceBlessedbytheAncients = 269711;
            public const int BlessingoftheAncientMachinist = 269543;//Yellow get buff
            public const int BlessingoftheEternalCleric = 269543;//Red get buff
            public const int BlessingoftheAncientForm = 269534;//Green get buff
            public const int BlessingoftheEternalCraftsman = 269540;//Blue get buff

            public const int CallofRust = 270011; //blue
            public const int CrawlingSkin = 270010; //green
            public const int HealingBlight = 270013; //red
            public const int GreedoftheSource = 270012; //yellow
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

        public static void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (_mainWindow != null && _mainWindow.IsValid && _mainWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);
        }

        public static void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { "   at " }, StringSplitOptions.None);

                    View xmlRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                    xmlRoot.FindChild("TextLabel", out TextView labelView);
                    labelView.Text = parts[0];
                    labelView.SetColor(Color.Red);
                    errorView.AddChild(xmlRoot, true);

                    if (parts.Length > 1)
                    {
                        View methodRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        methodRoot.FindChild("TextLabel", out TextView methodLabel);
                        methodLabel.Text = "at " + parts[1];
                        errorView.AddChild(methodRoot, true);
                    }
                }
            }
        }

        #endregion
    }
}