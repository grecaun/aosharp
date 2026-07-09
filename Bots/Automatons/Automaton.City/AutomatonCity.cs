using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using AutomatonCity.IPCMessages;
using SharpNav;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.ChatMessages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;


namespace AutomatonCity
{
    public class AutomatonCity : AOPluginEntry
    {
        public const string PluginName = "AutomatonCity";
        private const string Version_Number = "2.0.0";

        public static Settings _settings;

        public static StateMachine _stateMachine;
        public static SMovementController SMovementController { get; private set; }
        public static IPCChannel IPCChannel { get; private set; }

        public static bool CityUnderAttack = false;

        public static Identity Leader = Identity.None;
        private bool _leader = false;

        //public static bool Ready = true;
        //private Dictionary<Identity, bool> teamReadiness = new Dictionary<Identity, bool>();
        //private bool? lastSentIsReadyState = null;

        private List<Settings> settingsToSave = new List<Settings>();

        Window _mainWindow;
        Window _kittingWindow;
        Window _infoWindow;

        public static string EnableString = "Enable";
        public static string TeamButtonState = "Team";

        double SendDelay;

        public static readonly Shared.Kits kitsInstance = new Shared.Kits();

        public static readonly BossLootState Boss_Loot_State = new BossLootState();
        public static readonly BossRoomState Boss_Room_State = new BossRoomState();
        public static readonly ButtonExitState Button_Exit_State = new ButtonExitState();
        public static readonly CityAttackState City_Attack_State = new CityAttackState();
        public static readonly CityControllerState City_Controller_State = new CityControllerState();
        public static readonly EnterState Enter_State = new EnterState();
        public static readonly IdleState Idle_State = new IdleState();
        public static readonly NavGenState Nav_Gen_State = new NavGenState();
        public static readonly PathState Path_State = new PathState();
        public static readonly WaitForShipState Wait_For_Ship_State = new WaitForShipState();

        public const int ICCHQ = 655;

        public static int[] validPlayfields = new[] { 5002, 6010, 5001 };

        public static Vector3 _iCCReclaim = new Vector3(3232.2f, 35.2f, 923.2f);
        public static Vector3 _iCCTeleportUp = new Vector3(3160.4f, 36.3f, 866.9f);
        public static Vector3 _iCCCenterofCities = new Vector3(3138.6f, 52.1f, 826.0f);

        public static Vector3 _montroyalGaurdPos = new Vector3(587.1f, 160.7f, 649.4f);
        public static Vector3 _serenityGaurdPos = new Vector3(998.1f, 5.0f, 1178.5f); //998.1, 1178.5, 5.0
        public static Vector3 _playadelGaurdPos = new Vector3(212.6f, 32.7f, 338.7f); //212.6, 338.7, 32.7

        public static Door _exitDoor;

        public static List<string> _ignores = new List<string>
        {
            "Alien Coccoon"
        };

        public static List<Identity> Bosses = new List<Identity>();

        public static string previousErrorMessage = string.Empty;

        
        public static Vector3 _exitDoorLocation = Vector3.Zero;
        public static List<Vector3> _downButtonLocation = new List<Vector3>();

        public static double _buttonTimer = 0;
        public static int _currentFloor = 9999;
        public static int _currentAbsFloor => Math.Abs(_currentFloor);
        public static NavMesh[] _navMeshes = null;

        public static DungeonNavMeshFactory navMeshFactory;

        public override void Run()
        {
            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            SMovementController.Set();

            navMeshFactory = new DungeonNavMeshFactory();

            _settings = new Settings(PluginName);

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;

            _settings.AddVariable("Tank", false);

            _settings.AddVariable("Ship", false);
            _settings.AddVariable("Corpses", false);
            _settings.AddVariable("Looter", false);

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
            IPCChannel.RegisterCallback((int)IPCOpcode.Enter, EnterMessage);

            Chat.RegisterCommand("enable", EnableCommand);
            Chat.RegisterCommand("buddy", BuddyCommand);
            Chat.RegisterCommand("buddychannel", ChannelCommand);
            Chat.RegisterCommand("form", FormCommand);

            Chat.RegisterCommand("gen", (c, p, w) => _stateMachine.SetState(Nav_Gen_State));
            Chat.RegisterCommand("floor", (c, p, w) => Chat.WriteLine($"{DynelManager.LocalPlayer.Room.Floor} , " +
                $"{DynelManager.LocalPlayer.Room.Name}"));

            Chat.RegisterCommand("doors", (c, p, w) =>
            {
                foreach (var door in DynelManager.AllDynels)
                {
                    Chat.WriteLine($"Name = {door.Name}, Type {door.Identity.Type}");
                }

            });

            _stateMachine = new StateMachine(Idle_State);

            Game.OnUpdate += OnUpdate;
            Network.N3MessageSent += N3MessageSent;
            UIController.WindowDeleted += Windowclosed;
            Team.TeamRequest += OnTeamRequest;
            Network.ChatMessageReceived += CityAttackStatus;
            Game.TeleportEnded += TeleportEnded;

            MainUI();

            Chat.WriteLine($"{PluginName} Loaded!");
            Chat.WriteLine("/buddy to open or close the ui. /enable to start or stop.");
        }

        private void TeleportEnded(object sender, EventArgs e)
        {
            ChangedFloors();
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

            _settings["Ship"] = settingsUpdateMessage.Include_Ship;

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

        private void EnterMessage(int sender, IPCMessage msg)
        {
            if (!(_stateMachine.CurrentState is EnterState))
            {
                Chat.WriteLine("enter");
                _stateMachine.SetState(Enter_State);
            }
        }

        //private void OnWaitAndReadyMessage(int sender, IPCMessage msg)
        //{
        //    if (msg is WaitAndReadyIPCMessage waitAndReadyMessage)
        //    {
        //        Identity senderIdentity = waitAndReadyMessage.PlayerIdentity;

        //        teamReadiness[senderIdentity] = waitAndReadyMessage.IsReady;


        //        bool allReady = true;


        //        foreach (var teamMember in Team.Members)
        //        {
        //            if (teamReadiness.ContainsKey(teamMember.Identity) && !teamReadiness[teamMember.Identity])
        //            {
        //                allReady = false;
        //                break;
        //            }
        //        }

        //        if (Leader == DynelManager.LocalPlayer.Identity)
        //        {
        //            Ready = allReady;

        //        }
        //    }
        //}

        #endregion

        #region Subscriptions
        private void OnUpdate(object s, float deltaTime)
        {
            if (Game.IsZoning) { return; }

            //foreach (Room room in Playfield.Rooms)
            //{
            //    //if (room.Instance != 6)
            //    //    continue;

            //    for (int i = 0; i < room.NumDoors; i++)
            //    {
            //        if (room.GetDoorConnectZone(i) == room.Instance)
            //            continue;

            //        room.GetDoorPosRot(i, out Vector3 pos, out Quaternion rot);

            //        Vector3 Pos0 = pos + rot.Forward;
            //        Vector3 Pos1 = pos;
            //        Debug.DrawLine(Pos0, Pos1, DebuggingColor.Purple);
            //        Debug.DrawSphere(Pos0, 0.2f, DebuggingColor.Purple);
            //        Debug.DrawSphere(Pos1, 0.2f, DebuggingColor.Purple);
            //    }
            //}

            if (_mainWindow?.IsValid == true)
            {
                TeamButtonState = Team.IsInTeam ? "Disband" : "Team";

                if (_mainWindow.FindView("TeamButton", out Button teamButton))
                {
                    if (teamButton.Label != TeamButtonState)
                        teamButton.SetLabel(TeamButtonState);
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

            //old onupdate code, needs reworked
            //if (DynelManager.LocalPlayer.Identity != Leader)
            //{
            //    var localPlayer = DynelManager.LocalPlayer;
            //    bool currentIsReadyState = true;


            //    if (!Shared.Kits.InCombat())
            //    {
            //        if (Spell.HasPendingCast || localPlayer.NanoPercent < 66 || localPlayer.HealthPercent < 66)
            //        {
            //            currentIsReadyState = false;
            //        }
            //    }

            //    else if (!Spell.HasPendingCast && localPlayer.NanoPercent > 70
            //        && localPlayer.HealthPercent > 70)
            //    {
            //        currentIsReadyState = true;
            //    }


            //    if (currentIsReadyState != lastSentIsReadyState)
            //    {
            //        Identity localPlayerIdentity = DynelManager.LocalPlayer.Identity;


            //        IPCChannel.Broadcast(new WaitAndReadyIPCMessage
            //        {
            //            IsReady = currentIsReadyState,
            //            PlayerIdentity = localPlayerIdentity
            //        });
            //        lastSentIsReadyState = currentIsReadyState;
            //    }
            //}

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
            Network.ChatMessageReceived -= CityAttackStatus;

            return;
        }

        private void CityAttackStatus(object s, ChatMessageBody msg)
        {
            if (msg.PacketType != ChatMessageType.GroupMessage) { return; }

            var groupMsg = (GroupMsgMessage)msg;

            if (groupMsg.MessageType != GroupMessageType.Org) { return; }

            if (groupMsg.Text.Contains("Wave counter started."))
            {
                Chat.WriteLine("City is under attack!");

                CityUnderAttack = true;
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

                Handle_Enable();
                return;
            }
        }

        private void BuddyCommand(string arg1, string[] arg2, ChatWindow window)
        {
            MainUI();
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
                Include_Ship = _settings["Ship"].AsBool(),
            });
            Save();
        }

        #endregion

        #region Misc.

        private void MainUI()
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

            if (_mainWindow.FindView("TeamButton", out Button teamButton))
            {
                teamButton.SetLabel(TeamButtonState);
                teamButton.Clicked = Team_Button_Clicked;
            }

            if (_mainWindow.FindView("KittingButton", out Button kittingButton))
                kittingButton.Clicked = Kitting_Button_Clicked;

            if (_mainWindow.FindView("BroadcastSettings", out Button settingsButton))
                settingsButton.Clicked = Broadcast_Settings_Button_Clicked;

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
            Team.TeamRequest -= OnTeamRequest;
            Network.ChatMessageReceived -= CityAttackStatus;
            _mainWindow?.Close();
            _infoWindow?.Close();
        }

        public static void Start()
        {
            Chat.WriteLine($"{PluginName} Enabled.");

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(Idle_State);
        }

        private void Stop()
        {
            Chat.WriteLine($"{PluginName} Disabled.");

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(Idle_State);

            MovementController.Instance.Halt();
            SMovementController.Halt();

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
            settingsToSave.ForEach(_settings => _settings.Save());
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

        public static bool CanProceed()
        {
            return DynelManager.LocalPlayer.HealthPercent > 65
                && DynelManager.LocalPlayer.NanoPercent > 65
                && DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1
                && DynelManager.LocalPlayer.MovementState != MovementState.Sit
                && !Spell.HasPendingCast;
        }

        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;

            var lineMatch = Regex.Match(ex.StackTrace ?? "", @":line (\d+)$", RegexOptions.Multiline);

            if (lineMatch.Success)
                lineNumber = int.Parse(lineMatch.Groups[1].Value);

            return lineNumber;
        }

        public static void LockedDoors()
        {
            var lockPick = Inventory.Items.FirstOrDefault(c => c.Name.Contains("Lock Pick"));
            if (lockPick == null) return;
            var door = Playfield.Doors.Where(d => d.IsLocked).OrderBy(c => c.DistanceFrom(DynelManager.LocalPlayer)).FirstOrDefault();
            if (door == null) return;

            if (DynelManager.LocalPlayer.Position.DistanceFrom(door.Position) < 5)
                lockPick.UseOn(door.Identity);
        }

        public static void HandleButtonUsage(Dynel button)
        {
            if (button == null) return;
            if (button.Position.DistanceFrom(DynelManager.LocalPlayer.Position) < 4f)
            {
                if (SMovementController.IsNavigating())
                    SMovementController.Halt();

                if (Time.AONormalTime < _buttonTimer + 3.0) return;
                _buttonTimer = Time.AONormalTime;
                button.Use();
                return;
            }
            else if (!SMovementController.IsNavigating())
            {
                SMovementController.SetNavDestination(button.Position);
                return;
            }
        }

        public static void HandleAttack(SimpleChar target, bool attacking)
        {
            if (target.IsInAttackRange(true) && target.IsInLineOfSight)
            {
                if (SMovementController.IsNavigating())
                    SMovementController.Halt();
                else if (!DynelManager.LocalPlayer.IsAttacking && DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)

                    DynelManager.LocalPlayer.Attack(target, false);
            }
            else
                SMovementController.SetNavDestination(target.Position);
            return;
        }

        public static void ChangedFloors()
        {
            if (_currentFloor == DynelManager.LocalPlayer.Room.Floor) return;
            SMovementController.Halt();
            Chat.WriteLine($"ChangedFloors: Last floor {_currentFloor}, current floor {DynelManager.LocalPlayer.Room.Floor}");
            _currentFloor = DynelManager.LocalPlayer.Room.Floor;
            if (_navMeshes != null)
                SMovementController.LoadNavmesh(_navMeshes[_currentFloor], true);
            else
            {
                navMeshFactory.GenerateNavMeshAsync().ContinueWith(navMesh =>
                {
                    if (navMesh.Result == null)
                    {
                        Chat.WriteLine("NavGen failed");
                        return;
                    }

                    _navMeshes = navMesh.Result;
                    SMovementController.LoadNavmesh(_navMeshes[_currentAbsFloor], true);
                    Chat.WriteLine("NavGen Finished");
                });
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
