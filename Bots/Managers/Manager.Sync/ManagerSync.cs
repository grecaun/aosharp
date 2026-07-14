using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using AOSharp.Common.GameData;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using ManagerSync.IPCMessages;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerSync
{
    public class ManagerSync : AOPluginEntry
    {
        public const string PluginName = "ManagerSync";

        public static string Version_Number = "Version = 2.0.9";

        public static IPCChannel _IPCChannel;

        public static Settings _settings;
        public static string PluginDir;

        public static Item UseMsgItem = null;
        public static Identity UseMsgTarget = Identity.None;
        public static GenericCmdAction UseMsgAction = GenericCmdAction.None;
        public static Identity UseMsgSender = Identity.None;
        public static Identity MoveSenderID = Identity.None;
        public static HashSet<Identity> Cached_Identities = new HashSet<Identity>();

        public static string EnableString;
        public static string TeamButtonState = "Team";
        public static Identity TeamSender = Identity.None;

        public static Window settingsWindow;
        public static Window _infoWindow;

        public static Dictionary<RingName, string> _ringMap = new Dictionary<RingName, string> {{ RingName.PureNovictumRing, "Pure Novictum Ring" }, { RingName.RimyRing, "Rimy Ring" },
            { RingName.AchromicRing, "Achromic Ring" }, { RingName.SanguineRing, "Sanguine Ring" }, { RingName.CaliginousRing, "Caliginous Ring" }};

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        public static bool IsActiveWindow => GetForegroundWindow() == Process.GetCurrentProcess().MainWindowHandle;

        static List<Settings> settingsToSave = new List<Settings>();
        public static List<string> ErrorMessages = new List<string>();
        static int _currentFormation = 1;

        public static bool _pendingReform = false;

        public static List<Identity> RaidTeamIdentities = new List<Identity>();
        private double teamRaidDelay = 0.0;
        private double bagDelay = 1.0;
        private List <Identity> Bags = new List<Identity>();

        internal static Vector3 plantedLocation = new Vector3();
        internal static bool planted = false;

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

                _settings.AddVariable("Enable", false);
                EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

                _settings.AddVariable("IPCChannel", 4);
                _settings["IPCChannel"] = 4;

                _settings.AddVariable("SyncAttack", false);
                _settings.AddVariable("SyncMove", false);
                _settings.AddVariable("SyncBags", false);
                _settings.AddVariable("SyncUse", true);
                _settings.AddVariable("SyncChat", false);
                _settings.AddVariable("NPCTrade", false);
                _settings.AddVariable("SyncTrade", false);
                _settings.AddVariable("BSSideSelection", 0);

                _settings.AddVariable("OutsideTeamInvites", false);
                _settings["OutsideTeamInvites"] = false;

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);
                _settings.AddVariable("WindowBottomRightX", 300f);
                _settings.AddVariable("WindowBottomRightY", 300f);

                settingsToSave.Add(_settings);

                _IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                _IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, Receiver.OnStartStopMessage);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Attack, Receiver.OnAttackMessage);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Use, Receiver.OnUseMessage);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Grid, Receiver.OnGridSelectionMessage);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Move, Receiver.OnMoveMessage);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Target, Receiver.OnLookAt);
                _IPCChannel.RegisterCallback((int)IPCOpcode.UISettings, Receiver.BroadcastSettingsReceived);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Spread, Receiver.ReceivedSpreadOutCommand);
                _IPCChannel.RegisterCallback((int)IPCOpcode.NpcChatOpenClose, Receiver.OnNpcChatOpenClose);
                _IPCChannel.RegisterCallback((int)IPCOpcode.NpcChatAnswer, Receiver.OnNpcChatAnswer);

                _IPCChannel.RegisterCallback((int)IPCOpcode.NPCStartTrade, Receiver.OnNPCStartTrade);
                _IPCChannel.RegisterCallback((int)IPCOpcode.NPCTrade, Receiver.OnNPCTrade);
                _IPCChannel.RegisterCallback((int)IPCOpcode.NPCFinishTrade, Receiver.OnNPCFinishTrade);
                _IPCChannel.RegisterCallback((int)IPCOpcode.LogOut, Receiver.OnLogOut);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Team, Receiver.LocalTeamMessageReceived);
                _IPCChannel.RegisterCallback((int)IPCOpcode.OSInvites, Receiver.OutsideInvitesRecieved);
                _IPCChannel.RegisterCallback((int)IPCOpcode.LFTCode, Receiver.LFT_Message_Received);
                _IPCChannel.RegisterCallback((int)IPCOpcode.Plant, Receiver.PlantReceived);

                _IPCChannel.RegisterCallback((int)IPCOpcode.BattleStation, Receiver.Battle_Station_Message_Received);

                Chat.RegisterCommand(PluginName, Chat_Commands.ManagerCommand);
                Chat.RegisterCommand("ManagerSyncchannel", Chat_Commands.ChannelCommand);
                Chat.RegisterCommand("sync", Chat_Commands.ManagerSyncCommand);

                Chat.RegisterCommand("syncattack", (cmd, param, win) => ToggleSetting("SyncAttack", "Sync attack", param));
                Chat.RegisterCommand("syncuse", (cmd, param, win) => ToggleSetting("SyncUse", "Sync use", param));
                Chat.RegisterCommand("syncchat", (cmd, param, win) => ToggleSetting("SyncChat", "Sync chat", param));
                Chat.RegisterCommand("syncnpctrade", (cmd, param, win) => ToggleSetting("NPCTrade", "Npc trade", param));
                Chat.RegisterCommand("synctrade", (cmd, param, win) => ToggleSetting("SyncTrade", "Sync trading", param));
                Chat.RegisterCommand("syncmove", (cmd, param, win) => ToggleSetting("SyncMove", "Sync move", param));
                Chat.RegisterCommand("syncbags", (cmd, param, win) => ToggleSetting("SyncBags", "Sync bags", param));

                Chat.RegisterCommand("logout", Chat_Commands.Send_Single_Logout);

                Chat.RegisterCommand("form", Chat_Commands.FormCommand);
                Chat.RegisterCommand("formraid", Chat_Commands.FormRaidCommand);
                Chat.RegisterCommand("convert", Chat_Commands.RaidCommand);
                Chat.RegisterCommand("reform", Chat_Commands.ReformCommand);

                Chat.RegisterCommand("synclft", Chat_Commands.LFT);

                Chat.RegisterCommand("spreadem", (command, param, chatWindow) =>
                {
                    SpreadOut();
                });

                Chat.RegisterCommand("plant", (cmd, param, win) => Plant(param));

                Game.OnUpdate += OnUpdate;
                Network.N3MessageSent += Sender.Network_N3MessageSent;
                Network.N3MessageReceived += N3MessageReceived;
                Game.TeleportEnded += TeleportEnded;
                Game.PlayfieldInit += PlayfieldInit;
                UIController.WindowDeleted += Windowclosed;
                Team.TeamRequest += Receiver.TeamRequestReceived;
                Battlestation.Invited += BSInvite;

                Chat.WriteLine($"{PluginName} Loaded!");
                Chat.WriteLine($"/{PluginName} for UI.");
                Chat.WriteLine($"/macro mSync /{PluginName}");
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void BSInvite(object sender, BattlestationInviteEventArgs e)
        {
            e.Accept();
        }

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= Sender.Network_N3MessageSent;
            Network.N3MessageReceived -= N3MessageReceived;
            Game.TeleportEnded -= TeleportEnded;
            Game.PlayfieldInit -= PlayfieldInit;
            UIController.WindowDeleted -= Windowclosed;
            Team.TeamRequest -= Receiver.TeamRequestReceived;
            settingsWindow?.Close();
            _infoWindow?.Close();
        }

        #region Event Subscriptions

        void OnUpdate(object s, float deltaTime)
        {
            try
            {
                if (Game.IsZoning) {
                    planted = false;
                    return;
                }

                if (settingsWindow != null && settingsWindow.IsValid)
                {
                    string newState = Team.IsInTeam ? "Disband" : "Team";

                    if (TeamButtonState != newState)
                    {
                        TeamButtonState = newState;

                        if (settingsWindow.FindView("TeamButton", out Button teamButton))
                            teamButton.SetLabel(newState);
                    }
                }

                if (_pendingReform && !Team.IsInTeam)
                {
                    _pendingReform = false;
                    if (TeamSender == DynelManager.LocalPlayer.Identity)
                        _IPCChannel.Broadcast(new LocalTeamMessage
                        {
                            Sender = TeamSender,
                            TeamAction = 0,
                            Receiver = Identity.None,
                        });
                }

                if (RaidTeamIdentities.Count > 0)
                {
                    if (Time.AONormalTime > teamRaidDelay)
                    {
                        if (Team.IsInTeam)
                        {
                            if (!Team.IsRaid)
                                Team.ConvertToRaid();
                            else
                            {
                                var id = RaidTeamIdentities.Find(i => !Team.Members.Exists(m => m.Identity == i));
                                if (id.Type != IdentityType.None)
                                    Team.Invite(id);

                                var member = RaidTeamIdentities.Find(i => Team.Members.Exists(m => m.Identity == i));
                                if (member.Type != IdentityType.None)
                                    RaidTeamIdentities.Remove(member);
                            }
                        }
                        else
                        {
                            var firstId = RaidTeamIdentities.First();
                            if (firstId != IdentityType.None)
                                Team.Invite(firstId);
                        }

                        teamRaidDelay = Time.AONormalTime + 0.2;
                    }
                }

                if (!_settings["Enable"].AsBool()) return;

                if (_settings["SyncUse"].AsBool())
                    Receiver.HandleSyncUseTick();

                if (_settings["SyncBags"].AsBool())
                    SyncBag();

                if (MoveSenderID == Identity.None) return;
                var leader = DynelManager.Players.FirstOrDefault(l => l.Identity == MoveSenderID);
                var localplayer = DynelManager.LocalPlayer;
                if (leader == null) return;

                if (_settings["SyncMove"].AsBool())
                {
                    if (localplayer.MovementState == MovementState.Run && leader.MovementState != MovementState.Run)
                        MovementController.Instance.SetMovement(MovementAction.FullStop);
                }

                if (planted && localplayer.Position.DistanceFrom(plantedLocation) > 1)
                {
                    MovementController.Instance.SetDestination(plantedLocation);
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        void N3MessageReceived(object s, N3Message n3Msg)
        {
            if (!_settings["Enable"].AsBool()) return;

            switch (n3Msg.N3MessageType)
            {
                case N3MessageType.Trade:
                    if (!_settings["SyncTrade"].AsBool()) return;
                    var tradeMsg = (TradeMessage)n3Msg;

                    if (DynelManager.LocalPlayer.Identity == tradeMsg.Identity)
                    {
                        if (tradeMsg.Action == TradeAction.Accept)
                        {
                            if (Inventory.NumFreeSlots >= 1)
                                Trade.Accept(tradeMsg.Identity);
                            else
                                Trade.Decline();
                        }
                    }
                    else
                    {
                        if (tradeMsg.Action == TradeAction.Confirm)
                            Trade.Confirm(tradeMsg.Identity);
                    }
                    break;
                case N3MessageType.KnubotStartTrade:
                    if (!_settings["NPCTrade"].AsBool()) return;
                    if (!IsActiveWindow) return;
                    Sender.ItemsInInventory = Inventory.Items;
                    break;
                case N3MessageType.StartLogout:
                    Chat.WriteLine("N3MessageReceived, N3MessageType.StartLogout");
                    break;
            }
        }

        void TeleportEnded(object s, EventArgs e)
        {
            
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["SyncBags"].AsBool()) return;
            if (bagDelay == 0)
            {
                bagDelay = Time.AONormalTime + 2;
                Bags.Clear();
            }
        }

        private void PlayfieldInit(object sender, uint e)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["SyncBags"].AsBool()) return;
            if (bagDelay == 0)
            {
                bagDelay = Time.AONormalTime + 2;
                Bags.Clear();
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

        #endregion

        #region Helpers

        private void SyncBag()
        {
            try
            {
                if (Time.AONormalTime != 0 && Time.AONormalTime > bagDelay)
                {
                    foreach (var bag in Inventory.Backpacks.Where(b => b != null && b.Name != null && b.Name.IndexOf("syncbag", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        if (Bags.Contains(bag.Identity)) continue;
                        var bagAsItem = Inventory.Items.FirstOrDefault(i => i.UniqueIdentity.Instance == bag.Identity.Instance);
                        bagAsItem?.Use();
                        bagAsItem?.Use();
                        Bags.Add(bag.Identity);
                    }

                    bagDelay = 0;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public static void ToggleSetting(string settingKey, string label, string[] param)
        {
            _settings[settingKey] = !_settings[settingKey].AsBool();
            Chat.WriteLine($"{label} set to: {_settings[settingKey].AsBool()}");
            Save();

            if (settingKey.Equals("SyncMove") && _settings[settingKey].AsBool())
            {
                planted = false;
            }

            if (param.Length > 0 && param[0].Equals("all", StringComparison.OrdinalIgnoreCase))
                Broadcast_Settings();
        }
        public static void Broadcast_Settings()
        {
            _IPCChannel.Broadcast(new UISettings()
            {
                Attack = _settings["SyncAttack"].AsBool(),
                Bags = _settings["SyncBags"].AsBool(),
                Use = _settings["SyncUse"].AsBool(),
                Chat = _settings["SyncChat"].AsBool(),
                Trade = _settings["SyncTrade"].AsBool(),
                NpcTrade = _settings["NPCTrade"].AsBool(),
                Move = _settings["SyncMove"].AsBool(),
                Side = _settings["BSSideSelection"].AsInt32(),
            });
        }

        public static void Helper_Enable()
        {
            _settings["Enable"] = !_settings["Enable"].AsBool();
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            _IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = _settings["Enable"].AsBool() });

            if (_settings["Enable"].AsBool())
                Start();
            else
                Stop();

            Save();
        }

        public static void Window_Closed_helper()
        {
            if (settingsWindow?.IsValid == true)
            {
                Rect frame = settingsWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        public static void SpreadOut()
        {
            var player = DynelManager.LocalPlayer;

            var msg = new SpreadCommand
            {
                Position = player.Position,
                Instance = Playfield.ModelIdentity.Instance,
                Rotation = player.Rotation,
                SenderRadius = player.Radius,
                FormationType = (SpreadFormation)_currentFormation,
                Sender = player.Identity
            };

            _IPCChannel.Broadcast(msg);
            _currentFormation = (_currentFormation + 1) % 6;
        }

        public static void Plant(string[] param)
        {
            var player = DynelManager.LocalPlayer;
            if (param != null && param.Length > 0)
            {
                foreach (string p in param)
                {
                    foreach (var person in DynelManager.Players)
                    {
                        if (person.Name.Equals(p))
                        {
                            var msg = new PlantCommand
                            {
                                Position = player.Position,
                                Receiver = person.Identity,
                                Sender = player.Identity
                            };

                            Chat.WriteLine($"Telling {p} to stay at {player.Position}.");

                            _IPCChannel.Broadcast(msg);
                            break;
                        }
                    }
                }
            }
            else
            {
                Chat.WriteLine("One or more character names are specified for this command.");
            }
        }

        public static void Start()
        {
            Chat.WriteLine("Sync enabled");
        }

        public static void Stop()
        {
            Chat.WriteLine("Sync disabled");
        }

        #endregion

        #region Misc

        public enum RingName
        {
            Unknown = 0,
            PureNovictumRing,
            RimyRing,
            AchromicRing,
            SanguineRing,
            CaliginousRing
        }

        public static RingName GetRingNameFromItemName(string itemName)
        {
            foreach (var pair in _ringMap)
            {
                if (itemName.Contains(pair.Value))
                    return pair.Key;
            }

            return RingName.Unknown;
        }

        public static void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        public static void Handle_Log_Out()
        {
            ActivateGameClosing(QuitStatus.Exit);

            _settings["Enable"] = false;
            EnableString = "Disable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Stop();

            Save();
        }

        public static void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (settingsWindow != null && settingsWindow.IsValid && settingsWindow.FindView("Errors", out View errorView))
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

        #region Dll Imports

        [DllImport("Gamecode.dll", EntryPoint = "?N3Msg_NPCChatStartTrade@n3EngineClientAnarchy_t@@QAEXABVIdentity_t@@0@Z", CallingConvention = CallingConvention.ThisCall)]
        public static extern void eNPCChatStartTrade(IntPtr pEngine, ref Identity self, ref Identity npc);
        public static void NPCChatStartTrade(Identity self, Identity npc)
        {
            IntPtr pEngine = N3Engine_t.GetInstance();

            if (pEngine != IntPtr.Zero)
            {
                eNPCChatStartTrade(pEngine, ref self, ref npc);
            }
        }

        [DllImport("Gamecode.dll", EntryPoint = "?N3Msg_NPCChatAddTradeItem@n3EngineClientAnarchy_t@@QAEXABVIdentity_t@@00@Z", CallingConvention = CallingConvention.ThisCall)]
        public static extern void eNPCChatAddTradeItem(IntPtr pEngine, ref Identity self, ref Identity npc, ref Identity slot);
        public static void NPCChatAddTradeItem(Identity self, Identity npc, Identity item)
        {
            IntPtr pEngine = N3Engine_t.GetInstance();

            if (pEngine != IntPtr.Zero)
            {
                eNPCChatAddTradeItem(pEngine, ref self, ref npc, ref item);
            }
        }

        [DllImport("Gamecode.dll", EntryPoint = "?N3Msg_NPCChatEndTrade@n3EngineClientAnarchy_t@@QAEXABVIdentity_t@@0H_N@Z", CallingConvention = CallingConvention.ThisCall)]
        public static extern void eNPCChatEndTrade(IntPtr pEngine, ref Identity self, ref Identity npc, int credits, bool decline);
        public static void NPCChatEndTrade(Identity self, Identity npc, int credits = 0, bool accept = true)
        {
            IntPtr pEngine = N3Engine_t.GetInstance();

            if (pEngine != IntPtr.Zero)
            {
                eNPCChatEndTrade(pEngine, ref self, ref npc, credits, accept);
            }
        }

        public enum QuitStatus
        {
            Exit = 1,
            Camp = 2
        }

        [DllImport("GUI.dll", EntryPoint = "?ActivateGameClosing@FlowControlModule_t@@CAXW4QuitStatus_e@1@@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ActivateGameClosing(QuitStatus quitStatus);

        #endregion
    }
}
