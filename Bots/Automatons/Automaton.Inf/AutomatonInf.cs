using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using AutomatonInf.IPCMessages;
using Shared;
using SharpNav;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace AutomatonInf
{
    public class AutomatonInf : AOPluginEntry
    {
        public const string PluginName = "AutomatonInf";
        private const string Version_Number = "2.0.6";
        public static int Counter = 0;
        public static double StartStamp = 0.0;
        public static double RunTime = 0.0;

        public static StateMachine _stateMachine;

        public static Settings _settings;
        public static SMovementController SMovementController { get; set; }
        public static IPCChannel IPCChannel { get; set; }
        public SNavAgent NavAgentEvent { get; private set; }

        public static bool Enable = false;
        public static string EnableString = "Enable";
        public static string TeamButtonState = "Team";
        public static string currerntState = "State Null";

        public static double missionTimer = 0.0;
        public static double missionTimeOut;

        public static Identity Leader = Identity.None;
        private bool _leader = false;

        public static bool DoubleReward;

        public static double _stateTimeOut;

        public static List<string> ErrorMessages = new List<string>();
        public static string PluginDir;

        public static Window _mainWindow;
        private static Window infoWindow;
        private static Window _kittingWindow;

        public static float distance = -1f;

        public static List<string> NamesToIgnores = new List<string> { "One Who Obeys Precepts", "Guardian Spirit of Purification", "Umbral Spectre" };
        public static Dictionary<int, Vector3> Mobs = new Dictionary<int, Vector3>();
        public static NanoLine[] BuffsToIgnore = { NanoLine.CharmOther, NanoLine.Charm_Short };

        public static Mission state = new Mission();
        double SendDelay;

        public static double pathDelay;
        public static double teamDelay;

        private readonly List<Settings> settingsToSave = new List<Settings>();

        public static readonly Kits kitsInstance = new Kits();

        public static readonly IdleState Idle = new IdleState();
        public static readonly ClearMissionState Clear = new ClearMissionState();
        public static readonly DiedState Died = new DiedState();
        public static readonly EnterMissionState Enter = new EnterMissionState();
        public static readonly ExitMissionState Exit = new ExitMissionState();
        public static readonly GrabMissionState Grab = new GrabMissionState();
        public static readonly LeechState Leech = new LeechState();
        public static readonly LootingState Loot = new LootingState();
        public static readonly ReformState Reform = new ReformState();
        public static readonly RoamState Roam = new RoamState();
        public static readonly StartMissionState StartMission = new StartMissionState();
        public static readonly DefendSpiritState Defend = new DefendSpiritState();

        public static bool HasDied = false;

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

                _settings.AddVariable("DoubleReward", false);
                _settings.AddVariable("Looting", false);
                _settings.AddVariable("Leech", false);
                _settings.AddVariable("Tank", false);
                _settings.AddVariable("Clear", true);

                _settings.AddVariable("Stop", false);
                _settings["Stop"] = false;

                _settings.AddVariable("ModeSelection", 0);
                _settings.AddVariable("FactionSelection", 1);
                _settings.AddVariable("DifficultySelection", 2);

                _settings.AddVariable("IPCChannel", 48);

                _settings.AddVariable("KitNanoPercentageBox", 66);
                _settings.AddVariable("KitHealthPercentageBox", 66);

                _settings.AddVariable("LastRunTime", 0f);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                settingsToSave.Add(_settings);

                SMovementControllerSettings mSettings = new SMovementControllerSettings
                {
                    NavMeshSettings = new SNavMeshSettings { DrawNavMesh = false, DrawDistance = 20 },

                    PathSettings = new SPathSettings
                    {
                        DrawPath = false,
                        MinRotSpeed = 10,
                        MaxRotSpeed = 30,
                        UnstuckUpdate = 1000,
                        UnstuckThreshold = 2f,
                        RotUpdate = 10,
                        MovementUpdate = 200,
                        PathRadius = 0.29f,
                        Extents = new Vector3(1f, 3f, 1f)
                    }
                };

                SMovementController.Set(mSettings);

                SMovementController.AutoLoadNavmeshes($"{PluginDirectory}\\NavMeshes");

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, OnStartStopMessage);
                IPCChannel.RegisterCallback((short)IPCOpcode.ModeSelections, OnModeSelectionsMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.LeaderInfo, OnLeaderInfoMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.DeleteMission, ReceivedDeleteMissionMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.Team, LocalTeamMessageReceived);

                Chat.RegisterCommand("enable", EnableCommand);
                Chat.RegisterCommand("buddy", BuddyCommand);
                Chat.RegisterCommand("buddychannel", ChannelCommand);
                Chat.RegisterCommand("form", FormCommand);

                Chat.RegisterCommand("deletemission", (command, param, chatWindow) =>
                {
                    Chat.WriteLine("deletemission");
                    IPCChannel.Broadcast(new DeleteMissionIPCMessage());
                    if (state == Mission.done)
                        state = Mission.delete;

                    DeleteMission();
                });

                Chat.RegisterCommand("test", (s, p, cw) => { _stateMachine.SetState(Reform); });

                _stateMachine = new StateMachine(Idle);

                Game.OnUpdate += OnUpdate;
                Team.TeamRequest += OnTeamRequest;

                SMovementController.AgentStateChange += HandleAgentStateChange;
                SMovementController.OnRubberband += OnRubberband;
                SMovementController.Stuck += Stuck;

                Network.N3MessageReceived += N3MessageReceived;
                DynelManager.DynelSpawned += DynelSpawned;
                NpcDialog.AnswerListChanged += AnswerListChanged;
                UIController.WindowDeleted += Windowclosed;
                Network.N3MessageSent += N3MessageSent;
                Game.TeleportStarted += TeleportStarted;
                Game.TeleportEnded += TeleportEnded;

                Chat.WriteLine($"{PluginName} Loaded!");
                Chat.WriteLine("/buddy to open or close the ui. /enable to start or stop.");

                if (_settings["Tank"].AsBool())
                    MainWindow();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Team.TeamRequest -= OnTeamRequest;
            SMovementController.AgentStateChange -= HandleAgentStateChange;
            SMovementController.OnRubberband -= OnRubberband;
            SMovementController.Stuck -= Stuck;
            Network.N3MessageReceived -= N3MessageReceived;
            DynelManager.DynelSpawned -= DynelSpawned;
            NpcDialog.AnswerListChanged -= AnswerListChanged;
            UIController.WindowDeleted -= Windowclosed;
            Network.N3MessageSent -= N3MessageSent;
            Game.TeleportStarted -= TeleportStarted;
            Game.TeleportEnded -= TeleportEnded;
        }

        private void Windowclosed(object sender, Window e)
        {
            if (Game.IsZoning) return;
            switch (e.Name)
            {
                case PluginName:
                    Window_Closed_helper();
                    break;
            }
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

        private void N3MessageSent(object sender, N3Message e)
        {
            if (Game.IsZoning) return;
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            Enable = false;
            EnableString = "Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Save();

            Game.OnUpdate -= OnUpdate;
            Team.TeamRequest -= OnTeamRequest;
            SMovementController.AgentStateChange -= HandleAgentStateChange;
            SMovementController.OnRubberband -= OnRubberband;
            SMovementController.Stuck -= Stuck;
            Network.N3MessageReceived -= N3MessageReceived;
            DynelManager.DynelSpawned -= DynelSpawned;
            NpcDialog.AnswerListChanged -= AnswerListChanged;
            UIController.WindowDeleted -= Windowclosed;
            Network.N3MessageSent -= N3MessageSent;
            Game.TeleportStarted -= TeleportStarted;
            Game.TeleportEnded -= TeleportEnded;

            return;
        }

        #region Events

        private void N3MessageReceived(object sender, N3Message e)
        {
            if (Game.IsZoning) return;
            if (Playfield.ModelIdentity.Instance != Constants.Mission) return;

            switch (e.N3MessageType)
            {
                case N3MessageType.CharacterAction:
                    var action = (CharacterActionMessage)e;
                    if (action.Action != CharacterActionType.Death) return;
                    if (action.Identity == DynelManager.LocalPlayer.Identity)
                    {
                        if (_stateMachine.CurrentState is DiedState) return;
                        _stateMachine.SetState(Died);
                    }
                    if (!_settings["Clear"].AsBool()) return;
                    //if (DynelManager.LocalPlayer.Identity != Leader) return;
                    //if (!(_stateMachine.CurrentState is ClearMissionState)) return;
                    if (Mobs.ContainsKey(action.Identity.Instance))
                        Mobs.Remove(action.Identity.Instance);
                    break;
            }
        }

        private void DynelSpawned(object sender, Dynel e)
        {
            if (Game.IsZoning) return;
            if (!_settings["Clear"].AsBool()) return;
            //if (DynelManager.LocalPlayer.Identity != Leader) return;
            //if (!(_stateMachine.CurrentState is ClearMissionState)) return;
            if (Playfield.ModelIdentity.Instance != Constants.Mission) return;
            if (NamesToIgnores.Contains(e.Name)) return;
            if (Team.Members.Contains(e.Identity)) return;
            var dynel = new SimpleChar(e);
            switch (e.Identity.Type)
            {
                case IdentityType.SimpleChar:
                    if (dynel.Name.Contains("Guardian Spirit of Purification"))
                    {
                        Mobs.Clear();
                        return;
                    }
                    if (dynel.IsPet) return;
                    if (dynel.Buffs.Contains(BuffsToIgnore)) return;
                    if (!Mobs.ContainsKey(e.Identity.Instance))
                        Mobs.Add(dynel.Identity.Instance, dynel.Position);
                    break;
            }
        }

        private void AnswerListChanged(object s, Dictionary<int, string> options)
        {
            if (Game.IsZoning) return;
            var dialogNpc = DynelManager.GetDynel((Identity)s).Cast<SimpleChar>();

            switch (dialogNpc.Name)
            {
                case "The Retainer Of Ergo":
                    foreach (KeyValuePair<int, string> option in options)
                    {
                        if (option.Value == "Is there anything I can help you with?" ||
                            (_settings["FactionSelection"].AsInt32() == 1 && option.Value == "I will defend against the Unredeemed!") ||
                            (_settings["FactionSelection"].AsInt32() == 2 && option.Value == "I will defend against the Redeemed!") ||
                            (_settings["FactionSelection"].AsInt32() == 0 && option.Value == "I will defend against the creatures of the brink!") ||
                            (_settings["DifficultySelection"].AsInt32() == 0 && option.Value == "I will deal with only the weakest aversaries") || //Brink missions has a typo
                            (_settings["DifficultySelection"].AsInt32() == 0 && option.Value == "I will deal with only the weakest adversaries") ||
                            (_settings["DifficultySelection"].AsInt32() == 1 && option.Value == "I will challenge these invaders, as long as there aren't too many") ||
                            (_settings["DifficultySelection"].AsInt32() == 2 && !_settings["DoubleReward"].AsBool() && option.Value == "I will purge the temple of any and all assailants") ||
                            (_settings["DifficultySelection"].AsInt32() == 2 && _settings["DoubleReward"].AsBool() && !DoubleReward && option.Value == "I will challenge these invaders, as long as there aren't too many") ||
                            (_settings["DifficultySelection"].AsInt32() == 2 && _settings["DoubleReward"].AsBool() && DoubleReward && option.Value == "I will purge the temple of any and all assailants"))
                        {
                            NpcDialog.SelectAnswer(dialogNpc.Identity, option.Key);
                        }
                    }
                    break;
                case "One Who Obeys Precepts":
                    foreach (KeyValuePair<int, string> option in options)
                    {
                        if (option.Value == "Yes, I am ready.")
                            NpcDialog.SelectAnswer(dialogNpc.Identity, option.Key);
                    }
                    break;
            }
        }

        private void OnRubberband(Vector3 vector)
        {
            if (Game.IsZoning) return;
            Chat.WriteLine($"Rubberband");
            SMovementController.SetMovement(MovementAction.JumpStart);
        }

        private void Stuck(Vector3 vector1, Vector3 vector2)
        {
            if (Game.IsZoning) return;
            Chat.WriteLine("Stuck");
            SMovementController.SetMovement(MovementAction.JumpStart);
        }

        private void HandleAgentStateChange(NavMeshQuery.AgentNavMeshState state)
        {
            if (Game.IsZoning) return;
            if (!Enable) return;

            if (DynelManager.LocalPlayer.Velocity == 0) return;
            Chat.WriteLine($"HandleAgentStateChange");
            switch (state)
            {
                case NavMeshQuery.AgentNavMeshState.OutNavMesh:
                case NavMeshQuery.AgentNavMeshState.OutPolygon:
                    var extents = new Vector3(10, 20, 10);

                    if (SMovementController.GetClosestNavPoint(extents, out Vector3 nearestNavPoint))
                        DynelManager.LocalPlayer.Position = nearestNavPoint;
                    else
                    {
                        SMovementController.SetMovement(MovementAction.JumpStart);
                        SMovementController.SetMovement(MovementAction.ForwardStart);
                    }

                    break;
            }
        }

        private void TeleportStarted(object sender, EventArgs e)
        {
            teamDelay = Time.AONormalTime + 500;
        }

        private void TeleportEnded(object sender, EventArgs e)
        {
            teamDelay = Time.AONormalTime + 2;
        }

        

        private void OnTeamRequest(object sender, TeamRequestEventArgs e)
        {
            if (Game.IsZoning) return;

            if (Leader == Identity.None)
                Leader = e.Requester;

            e.Accept();
        }

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                if (Game.IsZoning) return;

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
                            counter.Text = $"Missions Completed = {Counter}";
                    }

                    if (_mainWindow.FindView("RunTime", out TextView runTime))
                    {
                        if (RunTime != 0.0)
                        {
                            TimeSpan elapsed = TimeSpan.FromSeconds(Time.AONormalTime - RunTime);
                            runTime.Text = $"RunTime = {string.Format("{0:D2}:{1:D2}:{2:D2}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds)}";
                        }
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
                    return;
                }

                kitsInstance.SitAndUseKit(_settings["KitNanoPercentageBox"].AsInt32(), _settings["KitHealthPercentageBox"].AsInt32());

                if (Enable)
                    _stateMachine.Tick();

            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #endregion

        #region Received Messages

        private void OnStartStopMessage(int sender, IPCMessage msg)
        {
            if (msg is StartStopIPCMessage startStopMessage)
            {
                Enable = startStopMessage.IsStarting;

                EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

                if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                    enableButton.SetLabel(EnableString);

                if (Enable)
                    Start();
                else
                    Stop();
            }
        }

        private void OnModeSelectionsMessage(int sender, IPCMessage msg)
        {
            if (msg is ModeSelectionsIPCMessage modeSelectionsMessage)
            {
                if (modeSelectionsMessage.Mode != _settings["ModeSelection"].AsInt32())
                    _settings["ModeSelection"] = modeSelectionsMessage.Mode;

                if (modeSelectionsMessage.Faction != _settings["FactionSelection"].AsInt32())
                    _settings["FactionSelection"] = modeSelectionsMessage.Faction;

                if (modeSelectionsMessage.Difficulty != _settings["DifficultySelection"].AsInt32())
                    _settings["DifficultySelection"] = modeSelectionsMessage.Difficulty;

                if (modeSelectionsMessage.Clear != _settings["Clear"].AsBool())
                    _settings["Clear"] = modeSelectionsMessage.Clear;

                Save();
            }
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
            else if (_settings["Tank"].AsBool())
                IPCChannel.Broadcast(new LeaderInfoIPCMessage { LeaderIdentity = DynelManager.LocalPlayer.Identity, Sender = DynelManager.LocalPlayer.Identity });
            else
                Leader = leaderInfoMessage.LeaderIdentity;
        }

        private void ReceivedDeleteMissionMessage(int arg1, IPCMessage message)
        {
            if (state == Mission.done)
                state = Mission.delete;

            DeleteMission();
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
                        Team.Leave();
                    break;
                case 3:
                    if (Leader != LPID) return;
                    Team.Invite(teamMsg.Receiver);

                    break;
            }
        }

        #endregion

        #region Chat Commands

        private void EnableCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length < 1)
            {
                if (!Team.IsInTeam)
                    ToggleTeam();
                ToggleEnable();
            }
        }

        private void BuddyCommand(string arg1, string[] arg2, ChatWindow window)
        {
            MainWindow();
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
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
        }
        public static void FormCommand(string command, string[] param, ChatWindow chatWindow)
        {
            ToggleTeam();
        }

        #endregion

        #region Button Clicked

        private void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (infoWindow?.IsValid == true)
            {
                infoWindow.Close();
                infoWindow = null;
                return;
            }

            infoWindow = Window.CreateFromXml("Info", PluginDirectory + $"\\UI\\{PluginName}InfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            infoWindow.Show(true);
        }

        private void Enable_Disable_Button_Clicked(object sender, ButtonBase e)
        {
            if (!Team.IsInTeam)
                ToggleTeam();

            ToggleEnable();
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

        public static void TeamButtonClicked(object sender, ButtonBase e)
        {
            ToggleTeam();
        }

        private void Broadcast_Settings_Button_Clicked(object sender, ButtonBase e)
        {
            IPCChannel.Broadcast(new ModeSelectionsIPCMessage()
            {
                Mode = _settings["ModeSelection"].AsInt32(),
                Faction = _settings["FactionSelection"].AsInt32(),
                Difficulty = _settings["DifficultySelection"].AsInt32(),
                Clear = _settings["Clear"].AsBool()
            });

            Save();
        }

        #endregion

        #region Misc.

        private void MainWindow()
        {
            try
            {
                if (_mainWindow?.IsValid == true)
                {
                    Window_Closed_helper();
                    _mainWindow.Close();
                    _mainWindow = null;
                    return;
                }

                _mainWindow = Window.CreateFromXml(PluginName, PluginDirectory + $"\\UI\\{PluginName}SettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

                if (_mainWindow.FindView("InfoView", out Button infoView))
                    infoView.Clicked = HandleInfoViewClick;

                if (_mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                {
                    enableButton.SetLabel(EnableString);
                    enableButton.Clicked = Enable_Disable_Button_Clicked;
                }

                if (_mainWindow.FindView("KittingButton", out Button kittingButton))
                    kittingButton.Clicked = Kitting_Button_Clicked;

                if (_mainWindow.FindView("TeamButton", out Button teamButton))
                {
                    teamButton.SetLabel(TeamButtonState);
                    teamButton.Clicked = TeamButtonClicked;
                }

                if (_mainWindow.FindView("BroadcastToggles", out Button broadcastToggles))
                    broadcastToggles.Clicked = Broadcast_Settings_Button_Clicked;

                if (_mainWindow.FindView("Errors", out View errorView))
                    PopulateErrorView(errorView);

                if (_mainWindow.FindView("State", out TextView state))
                    state.Text = currerntState;

                if (_mainWindow.FindView("Counter", out TextView counter))
                    counter.Text = $"Missions Completed = {Counter}";

                if (_mainWindow.FindView("LastRunTime", out TextView lastRunTime))
                {
                    int minutes = (int)(_settings["LastRunTime"].AsFloat() / 60);
                    int seconds = (int)(_settings["LastRunTime"].AsFloat() % 60);
                    lastRunTime.Text = $"Last run time = {minutes}m {seconds}s";
                }

                if (_mainWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";

                _mainWindow.Show(true);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void ToggleEnable()
        {
            Enable = !Enable;
            EnableString = Enable ? "Disable" : "Enable";

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            if (_settings["Tank"].AsBool())
                IPCChannel.Broadcast(new StartStopIPCMessage { IsStarting = Enable });

            if (Enable)
                Start();
            else
                Stop();
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

        public static bool MissionExist()
        {
            if (missionTimer != 0.0 && Time.AONormalTime > missionTimer + 10)
                return AOSharp.Core.Mission.List.Exists(x => x.DisplayName.Contains("The Purification Ri"));

            return true;
        }

        private void Start()
        {
            Chat.WriteLine($"{PluginName} enabled.");

            RunTime = Time.AONormalTime;

            if (Playfield.ModelIdentity.Instance != Constants.Mission)
            {
                if (state == Mission.done)
                    state = Mission.delete;

                DeleteMission();
            }

            if (_stateMachine == null || _stateMachine.CurrentState == null) return;

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(Idle);
        }

        private void Stop()
        {
            Chat.WriteLine($"{PluginName} disabled.");

            if (_stateMachine == null || _stateMachine.CurrentState == null) return;

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(Idle);

            if (DynelManager.LocalPlayer.Velocity > 0)
                SMovementController.Halt();
        }

        public enum Mission { done, delete, waiting }

        public static void DeleteMission()
        {
            var MissionToDelete = AOSharp.Core.Mission.List.FirstOrDefault(x => x.DisplayName.Contains("The Purification Ritual"));

            switch (state)
            {
                case Mission.delete:
                    if (MissionToDelete == null) return;
                    MissionToDelete.Delete();
                    Chat.WriteLine("Deleting mission.", ChatColor.Red);
                    state = Mission.waiting;
                    break;
                case Mission.waiting:
                    if (MissionToDelete == null) return;
                    state = Mission.done;
                    break;
            }
        }

        private void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
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
