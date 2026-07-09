using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AutomatonKiteHill.IPCMessages;
using Newtonsoft.Json;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using Debug = AOSharp.Core.Debug;

namespace AutomatonKiteHill
{
    public class AutomatonKiteHill : AOPluginEntry
    {
        public const string PluginName = "AutomatonKiteHill";
        private const string Version_Number = "2.0.0";
        public static int Counter = 0;

        public static StateMachine _stateMachine;
        public static IPCChannel IPCChannel { get; private set; }

        public static Settings _settings;

        private List<Settings> settingsToSave = new List<Settings>();

        Window _mainWindow;
        Window _infoWindow;
        Window _kittingWindow;
        Window _ntWindow;
        Window _enfoWindow;

        public static string EnableString = "Enable";

        public static readonly Shared.Kits kitsInstance = new Shared.Kits();

        public static readonly IdleState Idle = new IdleState();
        public static readonly NukeState Nuke_State = new NukeState();
        public static readonly PullBeach Pull_Beach = new PullBeach();
        public static readonly PullEast Pull_East = new PullEast();
        public static readonly PullWest Pull_West = new PullWest();

        public static DateTime RespawnTime;
        public static DateTime RespawnTimeEast;
        public static DateTime RespawnTimeWest;
        public static DateTime GameTime;

        public static double _stateTimeOut = 0;

        public static int _counterVec = 0;

        public static double _beachTimer;
        public static double _eastTimer;
        public static double _westTimer;

        public static string previousErrorMessage = string.Empty;

        private List<string> IgnorePlayers = new List<string>();
        private string _ignorePlayersFile => System.IO.Path.Combine(PluginDataDirectory.FullName, "IgnorePlayers.json");

        public static int PullStartIndex;
        public static List<Vector3> vectorList;
        public static Spell mongo;

        public static Identity Tank = Identity.None;
        private string Tank_Name = "Target tank and click save";
        public static Identity Nt = Identity.None;
        private string NT_Name = "Target Nt and click save";

        public static int[] PVPEnabled = { 202732, 214879, 216382, 284620 };

        private double _startDelay = 0;

        public override void Run()
        {
            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            _settings = new Settings(PluginName);

            _startDelay = Time.AONormalTime + 4;

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;

            _settings.AddVariable("AutoTeaming", false);

            _settings.AddVariable("SideSelection", 1);

            _settings.AddVariable("IPCChannel", 48);

            _settings.AddVariable("KitNanoPercentageBox", 90);
            _settings.AddVariable("KitHealthPercentageBox", 90);

            _settings.AddVariable("Time", 4.0f);
            _settings.AddVariable("HighMongo", 80);
            _settings.AddVariable("MaxLevel", 159);

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            settingsToSave.Add(_settings);

            IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

            IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, OnStartStopMessage);
            IPCChannel.RegisterCallback((short)IPCOpcode.SideSelections, OnSideSelectionsMessage);
            IPCChannel.RegisterCallback((int)IPCOpcode.MoveEast, OnMoveEastMessage);
            IPCChannel.RegisterCallback((int)IPCOpcode.MoveWest, OnMoveWestMessage);
            IPCChannel.RegisterCallback((int)IPCOpcode.IdentityInfo, IdentityMessageReceived);

            Chat.RegisterCommand("enable", EnableCommand);
            Chat.RegisterCommand("buddy", BuddyCommand);
            Chat.RegisterCommand("buddychannel", ChannelCommand);

            _stateMachine = new StateMachine(Idle);

            Game.OnUpdate += OnUpdate;
            Network.N3MessageSent += N3MessageSent;
            UIController.WindowDeleted += Windowclosed;
            DynelManager.DynelSpawned += DynelSpawned;

            LoadIgnorePlayers();

            SendTeamInvite();

            Main_Window();

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

        private void OnSideSelectionsMessage(int sender, IPCMessage msg)
        {
            if (!(msg is SideSelectionsIPCMessage sideSelectionsMessage)) return;

            _settings["SideSelection"] = sideSelectionsMessage.Side;

            Save();
        }

        private void OnMoveEastMessage(int sender, IPCMessage msg)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(new Vector3(1091.7f, 26.5f, 1051.4f)) > 1f && !MovementController.Instance.IsNavigating)
            {
                MovementController.Instance.SetDestination(new Vector3(1091.7f, 26.5f, 1051.4f));
            }
        }

        private void OnMoveWestMessage(int sender, IPCMessage msg)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(new Vector3(1064.4f, 25.6f, 1032.6f)) > 1f && !MovementController.Instance.IsNavigating)
            {
                MovementController.Instance.SetDestination(new Vector3(1064.4f, 25.6f, 1032.6f));
            }
        }

        private void IdentityMessageReceived(int arg1, IPCMessage message)
        {
            if (!(message is IdentityMessage idMessage)) return;
            switch (DynelManager.LocalPlayer.Profession)
            {
                case Profession.NanoTechnician:
                    Tank = idMessage.TankIdentity;
                    var tank = DynelManager.Players.FirstOrDefault(t => t.Identity == Tank);
                    if (tank != null)
                        Tank_Name = tank.Name;
                    break;
                case Profession.Enforcer:
                    Nt = idMessage.NTIdentity;
                    var nt = DynelManager.Players.FirstOrDefault(t => t.Identity == Nt);
                    if (nt != null)
                        NT_Name = nt.Name;
                    break;
            }
        }

        #endregion

        #region Subscriptions

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                if (Game.IsZoning) return;
                if (Time.AONormalTime < _startDelay) return;

                if (_mainWindow?.IsValid == true)
                {
                    if (_mainWindow.FindView("Counter", out TextView counter))
                    {
                        if (counter.Text != Counter.ToString())
                            counter.Text = $"Wave Count = {Counter}";
                    }
                }

                kitsInstance.SitAndUseKit(_settings["KitNanoPercentageBox"].AsInt32(), _settings["KitHealthPercentageBox"].AsInt32());

                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.NanoTechnician:

                        if (Nt == Identity.None)
                        {
                            Nt = DynelManager.LocalPlayer.Identity;
                            IPCChannel.Broadcast(new IdentityMessage { NTIdentity = DynelManager.LocalPlayer.Identity });
                        }

                        if (!_settings["Enable"].AsBool()) return;

                        if (Team.IsInTeam)
                        {
                            foreach (var member in Team.Members)
                            {
                                if (member?.Character == null)
                                {
                                    Team.Kick(member.Identity);
                                    continue;
                                }
                                if (member.Character.Buffs.Contains(PVPEnabled))
                                {
                                    Team.Kick(member.Identity);
                                    continue;
                                }
                                if (member?.Character.Level >= _settings["MaxLevel"].AsInt32())
                                {
                                    Team.Kick(member.Identity);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Handle_Enable();
                        }
                            break;
                    case Profession.Enforcer:

                        if (Tank == Identity.None)
                        {
                            Tank = DynelManager.LocalPlayer.Identity;
                            IPCChannel.Broadcast(new IdentityMessage { TankIdentity = DynelManager.LocalPlayer.Identity });
                        }

                        if (!_settings["Enable"].AsBool())
                        {
                            switch (_settings["SideSelection"].AsInt32())
                            {
                                case 3:
                                case 1:
                                    Debug.DrawSphere(new Vector3(1115.9f, 1.6f, 1064.3f), 0.2f, DebuggingColor.White);
                                    Debug.DrawLine(DynelManager.LocalPlayer.Position, new Vector3(1115.9f, 1.6f, 1064.3f), DebuggingColor.White); // East
                                    break;
                                case 2:
                                    Debug.DrawSphere(new Vector3(1043.2f, 1.6f, 1021.1f), 0.2f, DebuggingColor.White);
                                    Debug.DrawLine(DynelManager.LocalPlayer.Position, new Vector3(1043.2f, 1.6f, 1021.1f), DebuggingColor.White); // West

                                    break;
                                case 0:
                                    Debug.DrawSphere(new Vector3(898.1f, 4.4f, 289.9f), 0.2f, DebuggingColor.White);
                                    Debug.DrawLine(DynelManager.LocalPlayer.Position, new Vector3(898.1f, 4.4f, 289.9f), DebuggingColor.White); // beach
                                    break;
                            }

                            return;
                        }

                        break;
                }

                _stateMachine.Tick();
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred on line " + GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != previousErrorMessage)
                {
                    Chat.WriteLine(errorMessage);
                    Chat.WriteLine("Stack Trace: " + ex.StackTrace);
                    previousErrorMessage = errorMessage;
                }
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
            DynelManager.DynelSpawned -= DynelSpawned;

            return;
        }

        #endregion

        #region Chat Commands

        private void EnableCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length < 1)
            {
                Handle_Enable();
                return;
            }
        }

        private void BuddyCommand(string arg1, string[] arg2, ChatWindow window)
        {
            Main_Window();
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
                _infoWindow = Window.CreateFromXml("Info", PluginDirectory + $"\\UI\\{PluginName}InfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _infoWindow.Show(true);

                if (_infoWindow.FindView("PluginName", out TextView setText))
                    setText.Text = PluginName;
            }
        }

        internal void Enable_Disable_Button_Clicked(object s, ButtonBase button)
        {
            Handle_Enable();
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

        private void NT_Button_Clicked(object sender, ButtonBase e)
        {
            if (_ntWindow?.IsValid == true)
            {
                _ntWindow.Close();
                _ntWindow = null;
                Save();
                return;
            }

            _ntWindow = Window.CreateFromXml("Options", PluginDirectory + $"\\UI\\{PluginName}NTOptionsWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            if (_ntWindow.FindView("TankName", out TextView tankName))
                tankName.Text = Tank_Name;

            if (_ntWindow.FindView("SaveTank", out Button saveTankButton))
                saveTankButton.Clicked = Save_Tank_Button_Clicked;

            if (_ntWindow.FindView("LevelLimitBox", out TextInputView lvl))
                lvl.Text = _settings["MaxLevel"].AsInt32().ToString();

            if (_ntWindow.FindView("Save_Level_Button", out Button saveLvlButton))
                saveLvlButton.Clicked = Save_Level_Limit_button_Clicked;

            if (_ntWindow.FindView("Ignored_players", out DropdownMenu Ignored_Players_Dropdown))
            {
                for (int i = (int)IgnorePlayers.Count - 1; i >= 0; i--)
                    Ignored_Players_Dropdown.DeleteItem((uint)i);

                LoadIgnorePlayers();

                foreach (var item in IgnorePlayers)
                    Ignored_Players_Dropdown.AppendItem(item);
            }

            if (_ntWindow.FindView("SaveIgnored", out Button saveButton))
                saveButton.Clicked = Ignore_Save_Button_Clicked;

            if (_ntWindow.FindView("DeleteIgnored", out Button delButton))
                delButton.Clicked = Ignore_Delete_Button_Clicked;

            _ntWindow.Show(true);
        }

        private void Save_Tank_Button_Clicked(object sender, ButtonBase e)
        {
            if (Targeting.Target != null)
            {
                var tank = Targeting.Target;
                Tank_Name = tank.Name;
                Tank = tank.Identity;

                if (_ntWindow.FindView("TankName", out TextView Name))
                    Name.Text = Tank_Name;

            }
            else if (_ntWindow.FindView("TankName", out TextView Name))
                Name.Text = "Target the tank first";
        }

        private void Save_Level_Limit_button_Clicked(object sender, ButtonBase e)
        {
            if (_ntWindow.FindView("LevelLimitBox", out TextInputView levelLimitBox) &&
                int.TryParse(levelLimitBox.Text, out int levelLimitValue))
                _settings["MaxLevel"] = levelLimitValue;

            Save();
        }

        private void Ignore_Save_Button_Clicked(object sender, ButtonBase e)
        {
            _ntWindow.FindView("Ignored_players", out DropdownMenu menu);

            if (Targeting.Target != null)
            {
                var name = Targeting.Target.Name;
                if (!IgnorePlayers.Contains(name))
                {
                    IgnorePlayers.Add(name);
                    menu.AppendItem(name);
                    SaveIgnorePlayers();
                }
            }
        }
        private void Ignore_Delete_Button_Clicked(object sender, ButtonBase e)
        {
            _ntWindow.FindView("Ignored_players", out DropdownMenu menu);

            uint selectedIndex = menu.GetSelection();

            if (selectedIndex < IgnorePlayers.Count)
            {
                IgnorePlayers.RemoveAt((int)selectedIndex);
                menu.DeleteItem(selectedIndex);
                SaveIgnorePlayers();
            }
        }

        private void Enfo_Button_Clicked(object sender, ButtonBase e)
        {
            if (_enfoWindow?.IsValid == true)
            {
                _enfoWindow.Close();
                _enfoWindow = null;
                return;
            }

            _enfoWindow = Window.CreateFromXml("Options", PluginDirectory + $"\\UI\\{PluginName}EnfoOptionsWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            if (_enfoWindow.FindView("NTName", out TextView ntName))
                ntName.Text = NT_Name;

            if (_enfoWindow.FindView("SaveNT", out Button saveNTButton))
                saveNTButton.Clicked = Save_NT_Button_Clicked;

            if (_enfoWindow.FindView("RecastTime", out TextInputView recastTime))
                recastTime.Text = _settings["Time"].AsFloat().ToString("0.0");

            if (_enfoWindow.FindView("SwitchToHighMongo", out TextInputView switchToHigh))
                switchToHigh.Text = _settings["HighMongo"].AsInt32().ToString();

            if (_enfoWindow.FindView("SaveTankSettings", out Button saveTankSettings))
                saveTankSettings.Clicked = Save_Tank_Settings_Button_Clicked;

            _enfoWindow.Show(true);
        }

        private void Save_NT_Button_Clicked(object sender, ButtonBase e)
        {
            if (Targeting.Target != null)
            {
                var nt = Targeting.Target;
                NT_Name = nt.Name;
                Nt = nt.Identity;

                if (_enfoWindow.FindView("NTName", out TextView ntName))
                    ntName.Text = NT_Name;

            }
            else if (_enfoWindow.FindView("NTName", out TextView ntName))
                ntName.Text = "Target the NT first";
        }

        private void Save_Tank_Settings_Button_Clicked(object sender, ButtonBase e)
        {
            _enfoWindow.FindView("RecastTime", out TextInputView recastTime);
            _enfoWindow.FindView("SwitchToHighMongo", out TextInputView switchToHigh);

            if (float.TryParse(recastTime.Text, out float recast))
                _settings["Time"] = recast;

            if (int.TryParse(switchToHigh.Text, out int switchValue))
                _settings["HighMongo"] = switchValue;

            Save();
        }

        private void Broadcast_Settings_Button_Clicked(object sender, ButtonBase e)
        {
            IPCChannel.Broadcast(new SideSelectionsIPCMessage()
            {
                Side = _settings["SideSelection"].AsInt32(),
            });

            Save();
        }

        #endregion

        #region Misc.


        private void Main_Window()
        {
            if (_mainWindow?.IsValid == true)
            {
                Window_Closed_helper();

                _mainWindow.Close();
                _mainWindow = null;
                return;
            }

            _mainWindow = Window.CreateFromXml(PluginName, PluginDirectory + $"\\UI\\{PluginName}MainWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

            if (_mainWindow.FindView("InfoButton", out Button infoButton))
                infoButton.Clicked = Info_Button_Clicked;

            if (_mainWindow.FindView("Enable_Disable_Button", out Button enableBtn))
            {
                enableBtn.SetLabel(EnableString);
                enableBtn.Clicked = Enable_Disable_Button_Clicked;
            }

            if (_mainWindow.FindView("KittingButton", out Button kittingButton))
                kittingButton.Clicked = Kitting_Button_Clicked;

            if (_mainWindow.FindView("ProfessionOptions", out Button optionsButton))
            {
                switch (DynelManager.LocalPlayer.Profession)
                {
                    case Profession.NanoTechnician:
                        optionsButton.SetLabel("NT Options");
                        optionsButton.Clicked = NT_Button_Clicked;
                        break;
                    case Profession.Enforcer:
                        optionsButton.SetLabel("Enfo Options");
                        optionsButton.Clicked = Enfo_Button_Clicked;
                        break;
                }
            }

            if (_mainWindow.FindView("BroadcastSettings", out Button settingsButton))
                settingsButton.Clicked = Broadcast_Settings_Button_Clicked;

            if (_mainWindow.FindView("Counter", out TextView counter))
                counter.Text = $"Wave Count = {Counter}";

            if (_mainWindow.FindView("VersionNumber", out TextView version))
                version.Text = $"Version {Version_Number}";

            _mainWindow.Show(true);
        }


        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            DynelManager.DynelSpawned -= DynelSpawned;
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

            MovementController.Instance.Halt();
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

        public static class RelevantItems
        {
            public static readonly int[] Kits = {
                297274, 293296, 291084, 291083, 291082
            };
        }

        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;

            var lineMatch = Regex.Match(ex.StackTrace ?? "", @":line (\d+)$", RegexOptions.Multiline);

            if (lineMatch.Success)
            {
                lineNumber = int.Parse(lineMatch.Groups[1].Value);
            }

            return lineNumber;
        }

        private void SaveIgnorePlayers()
        {
            File.WriteAllText(_ignorePlayersFile, JsonConvert.SerializeObject(IgnorePlayers));
        }

        private void LoadIgnorePlayers()
        {
            if (File.Exists(_ignorePlayersFile))
            {
                IgnorePlayers = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_ignorePlayersFile)) ?? new List<string>();
            }
        }

        public static bool CanProceed()
        {
            return DynelManager.LocalPlayer.HealthPercent > _settings["KitHealthPercentageBox"].AsInt32()
                && DynelManager.LocalPlayer.NanoPercent > _settings["KitNanoPercentageBox"].AsInt32()
                && DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1
                && DynelManager.LocalPlayer.MovementState != MovementState.Sit
                && !Spell.HasPendingCast;
        }

        public static void SetMongoBasedOnHealth()
        {
            var localPlayer = DynelManager.LocalPlayer;
            var mongoSpells = Spell.GetSpellsForNanoline(NanoLine.MongoBuff).OrderByStackingOrder().ToList();

            var mongoHigh = mongoSpells.FirstOrDefault(); // highest stacking order
            var mongoLow = mongoSpells.LastOrDefault();   // lowest stacking order
            float healthPercentage = localPlayer.HealthPercent;

            if (healthPercentage > _settings["HighMongo"].AsInt32() && mongoLow != null && !localPlayer.Buffs.Contains(mongoHigh.Id))
            {
                mongo = mongoLow;
            }
            else if (mongoHigh != null)
            {
                mongo = mongoHigh;
            }
            else
            {
                mongo = null;
            }
        }

        #endregion

        #region Teaming

        private void DynelSpawned(object sender, Dynel e)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (e.Identity.Type != IdentityType.SimpleChar) return;
            if (DynelManager.Players.Any(p => p.Identity == e.Identity))
                SendTeamInvite();
        }

        private void SendTeamInvite()
        {
            if (!_settings["AutoTeaming"].AsBool()) return;
            foreach (var player in DynelManager.Players)
            {
                if (player.Buffs.Contains(PVPEnabled))
                {
                    if (!IgnorePlayers.Contains(player.Name))
                    {
                        IgnorePlayers.Add(player.Name);
                        SaveIgnorePlayers();
                    }
                    continue;
                }

                if (IgnorePlayers.Contains(player.Name)) continue;

                if (player.IsInTeam()) continue;
                if (player.Identity == DynelManager.LocalPlayer.Identity) continue;
                if (player.Identity == Tank) continue;
                if (player.Level >= _settings["MaxLevel"].AsInt32()) continue;

                if (Team.IsInTeam)
                {
                    int lowestLevel = int.MaxValue;
                    int highestLevel = int.MinValue;

                    foreach (var member in Team.Members)
                    {
                        if (DynelManager.LocalPlayer.Identity == member.Identity) continue;
                        int level = member.Character.Level;
                        if (level < lowestLevel) lowestLevel = level;
                        if (level > highestLevel) highestLevel = level;
                    }

                    if (player.Level <= lowestLevel * 0.7 && highestLevel <= player.Level * 0.7)
                        Team.Invite(player.Identity);
                }
                else if (player.Level * 0.7 >= DynelManager.LocalPlayer.Level)
                    Team.Invite(player.Identity);
            }
        }
    }

    #endregion
}

