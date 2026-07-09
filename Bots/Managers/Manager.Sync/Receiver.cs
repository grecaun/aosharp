using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using ManagerSync.IPCMessages;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using static ManagerSync.ManagerSync;

namespace ManagerSync
{
    internal static class Receiver
    {
        #region Received Msgs

        #region OnStartStopMessage

        public static void OnStartStopMessage(int sender, IPCMessage msg)
        {
            var startStopMessage = (StartStopIPCMessage)msg;
            if (startStopMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            if (_settings["Enable"].AsBool() != startStopMessage.IsStarting)
            {
                _settings["Enable"] = startStopMessage.IsStarting;
                EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

                if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                    enableButton.SetLabel(EnableString);

                if (_settings["Enable"].AsBool())
                    Start();
                else
                    Stop();

                Save();
            }
        }

        #endregion

        #region OnAttackMessage

        public static void OnAttackMessage(int sender, IPCMessage msg)
        {
            if (IsActiveWindow) { return; }
            if (!_settings["Enable"].AsBool()) { return; }
            if (!_settings["SyncAttack"].AsBool()) { return; }

            var attackMsg = (AttackIPCMessage)msg;

            if (attackMsg.Sender == DynelManager.LocalPlayer.Identity) { return; }
            if (!DynelManager.Players.Any(p => p.Identity == attackMsg.Sender)) { return; }
            if (attackMsg.Start)
            {
                var targetDynel = DynelManager.GetDynel(attackMsg.Target);
                DynelManager.LocalPlayer.Attack(targetDynel, false);
            }
            else { DynelManager.LocalPlayer.StopAttack(false); }
        }

        #endregion

        #region OnUseMessage

        public static void OnUseMessage(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["SyncUse"].AsBool()) return;
            if (IsActiveWindow) return;

            var useMsg = (UseMessage)msg;
            if (useMsg.Sender == DynelManager.LocalPlayer.Identity) return;
            if (!DynelManager.Players.Any(p => p.Identity == useMsg.Sender)) { return; }

            if (useMsg.RingName != RingName.Unknown)
            {
                var ringItemName = GetItemNameFromRingName(useMsg.RingName);
                FindUseRing(ringItemName);
            }
            else if (useMsg.Item_Instance != 0)
            {
                UseMsgItem = ProcessIDItem(useMsg.Item_Instance);
            }
            else
            {
                UseMsgItem = null;
            }

            UseMsgTarget = useMsg.Target != Identity.None ? useMsg.Target : Identity.None;
            UseMsgSender = useMsg.Sender;
            UseMsgAction = useMsg.Action;
        }

        public static void OnGridSelectionMessage(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["SyncUse"].AsBool()) return;
            if (IsActiveWindow) return;
            var selection = (GridCitySelectionMessage)msg;
            GridDestination.TeleportTo(selection.Selection);
        }

        #endregion

        #region OnMoveMessage

        public static void OnMoveMessage(int s, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (IsActiveWindow) return;
            if (Game.IsZoning) return;

            var moveMsg = (MoveMessage)msg;
            var lp = DynelManager.LocalPlayer;
            if (moveMsg.Sender == lp.Identity) return;
            if (Playfield.Identity.Instance != moveMsg.PlayfieldId) return;
            var sender = DynelManager.Players.FirstOrDefault(p => p.Identity == moveMsg.Sender);
            if (sender == null) return;

            var DistancePerSecond = 1f + (lp.GetStat(Stat.RunSpeed) / 100f);
            
            if (lp.Position.Distance2DFrom(moveMsg.Position) > DistancePerSecond * 2) return;

            MoveSenderID = moveMsg.Sender;
            lp.Position = moveMsg.Position;
            lp.Rotation = moveMsg.Rotation;
            MovementController.Instance.SetMovement(moveMsg.MoveType);
        }

        #endregion

        #region OnLookAt

        public static void OnLookAt(int sender, IPCMessage look)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (IsActiveWindow) return;

            var targetMsg = (TargetMessage)look;
            if (targetMsg.Sender == DynelManager.LocalPlayer.Identity) return;
            if (!DynelManager.Players.Any(p => p.Identity == targetMsg.Sender)) { return; }
            var localPlayer = DynelManager.LocalPlayer;

            if (localPlayer.IsAttacking) return;
            if (localPlayer.IsAttackPending) return;
            if (localPlayer.FightingTarget != null) return;
            if (Spell.HasPendingCast) return;
            if (Item.HasPendingUse) return;
            if (PerkAction.List.Any(p => p.IsExecuting)) return;

            Targeting.SetTarget(targetMsg.Target);
        }

        #endregion

        #region BroadcastSettingsReceived

        public static void BroadcastSettingsReceived(int arg1, IPCMessage message)
        {
            if (!(message is UISettings u)) return;
            if (u.Sender == DynelManager.LocalPlayer.Identity) return;
            if (_settings["SyncAttack"].AsBool() != u.Attack) { _settings["SyncAttack"] = u.Attack; Chat.WriteLine($"Sync attack set to: {u.Attack}"); }
            if (_settings["SyncBags"].AsBool() != u.Bags) { _settings["SyncBags"] = u.Bags; Chat.WriteLine($"Sync bags set to: {u.Bags}"); }
            if (_settings["SyncUse"].AsBool() != u.Use) { _settings["SyncUse"] = u.Use; Chat.WriteLine($"Sync use set to: {u.Use}"); }
            if (_settings["SyncChat"].AsBool() != u.Chat) { _settings["SyncChat"] = u.Chat; Chat.WriteLine($"Sync chat set to: {u.Chat}"); }
            if (_settings["SyncTrade"].AsBool() != u.Trade) { _settings["SyncTrade"] = u.Trade; Chat.WriteLine($"Sync trade set to: {u.Trade}"); }
            if (_settings["NPCTrade"].AsBool() != u.NpcTrade) { _settings["NPCTrade"] = u.NpcTrade; Chat.WriteLine($"NPC trade set to: {u.NpcTrade}"); }
            if (_settings["SyncMove"].AsBool() != u.Move) { _settings["SyncMove"] = u.Move; Chat.WriteLine($"Move set to: {u.Move}"); }
            if (_settings["BSSideSelection"].AsInt32() != u.Side) { _settings["BSSideSelection"] = u.Side; Chat.WriteLine($"Side set to: {u.Side}"); }

            Save();
        }

        #endregion

        #region OnLogOut

        public static void OnLogOut(int arg1, IPCMessage message)
        {
            if (!(message is LogOutIPCMessage l)) return;
            if (l.Sender == DynelManager.LocalPlayer.Identity) return;
            if (l.Target != Identity.None && l.Target != DynelManager.LocalPlayer.Identity) return;

            Handle_Log_Out();
        }

        #endregion

        #region LocalTeamMessageReceived

        public static void LocalTeamMessageReceived(int sender, IPCMessage msg)
        {
            if (!(msg is LocalTeamMessage teamMsg)) return;
            var LPID = DynelManager.LocalPlayer.Identity;

            switch (teamMsg.TeamAction)
            {
                case 0:
                    if (Team.IsInTeam) return;
                    TeamSender = teamMsg.Sender;
                    _IPCChannel.Broadcast(new LocalTeamMessage { Sender = LPID, Receiver = LPID, TeamAction = 3 });
                    break;
                case 2:
                    if (Team.IsInTeam)
                        Team.Leave();
                    break;
                case 3:
                    if (TeamSender != DynelManager.LocalPlayer.Identity) return;
                    Team.Invite(teamMsg.Receiver);
                    break;
                case 4:
                    _pendingReform = true;
                    break;
                case 5:
                    if (Team.IsInTeam) return;
                    TeamSender = teamMsg.Sender;
                    _IPCChannel.Broadcast(new LocalTeamMessage { Sender = LPID, Receiver = LPID, TeamAction = 6 });
                    break;
                case 6:
                    if (TeamSender != DynelManager.LocalPlayer.Identity) return;
                    if (RaidTeamIdentities.Contains(teamMsg.Receiver)) return;
                    RaidTeamIdentities.Add(teamMsg.Receiver);
                    break;
            }
        }

        #endregion

        #region Spread out

        public static void ReceivedSpreadOutCommand(int arg1, IPCMessage message)
        {
            var msg = message as SpreadCommand;
            if (msg.Sender == DynelManager.LocalPlayer.Identity) return;
            if (msg.Instance != Playfield.ModelIdentity.Instance) return;

            var player = DynelManager.LocalPlayer;

            Vector3 targetPos = msg.Position;

            switch (msg.FormationType)
            {
                case SpreadFormation.Line:
                    {
                        var team = Team.Members.Where(p => p.Identity != msg.Sender).OrderBy(p => p.Name).ToList();

                        int index = team.FindIndex(p => p.Identity == player.Identity);
                        if (index == -1)
                            return;

                        Vector3 right = msg.Rotation * Vector3.Right;
                        Vector3 back = msg.Rotation * new Vector3(0f, 0f, -2f);

                        float spacing = (msg.SenderRadius + player.Radius) * 2 + 1f;

                        Vector3 lineOrigin = msg.Position + back * (msg.SenderRadius * 2);

                        float offsetIndex = index - (team.Count - 1) / 2f;

                        targetPos = lineOrigin + right * (offsetIndex * spacing);

                        break;
                    }
                case SpreadFormation.Circle:
                    {
                        var team = Team.Members
                            .OrderBy(p => p.Name)
                            .ToList();

                        int index = team.FindIndex(p => p.Identity == player.Identity);
                        if (index == -1)
                            return;

                        float radius = (msg.SenderRadius + player.Radius) * 2 + 1f;
                        float angleStep = 360f / team.Count;
                        double angle = angleStep * index * (Math.PI / 180.0);

                        Vector3 offset = new Vector3(
                            (float)Math.Cos(angle),
                            0f,
                            (float)Math.Sin(angle)
                        ) * radius;

                        targetPos += offset;

                        break;
                    }
                case SpreadFormation.Square:
                    {
                        var team = Team.Members
                            .OrderBy(p => p.Name)
                            .ToList();

                        int index = team.FindIndex(p => p.Identity == player.Identity);
                        if (index == -1)
                            return;

                        int count = team.Count;
                        float spacing = (msg.SenderRadius + player.Radius) * 2 + 1f;

                        int cols = (int)Math.Ceiling(Math.Sqrt(count));
                        int row = (index + 1) / cols;
                        int col = (index + 1) % cols;

                        Vector3 forward = msg.Rotation * new Vector3(0f, 0f, 1f);
                        Vector3 right = msg.Rotation * new Vector3(1f, 0f, 0f);

                        Vector3 offset = right * (col * spacing) + forward * (row * spacing);

                        targetPos += offset;

                        break;
                    }
                case SpreadFormation.SingleFile:
                    {
                        var team = Team.Members.OrderBy(p => p.Name).ToList();

                        int senderIndex = team.FindIndex(p => p.Identity == msg.Sender);
                        int receiverIndex = team.FindIndex(p => p.Identity == player.Identity);

                        if (senderIndex == -1 || receiverIndex == -1 || senderIndex == receiverIndex)
                            return;

                        int relativeIndex = receiverIndex - senderIndex;

                        Vector3 right = msg.Rotation * Vector3.Right;
                        float spacing = (msg.SenderRadius + player.Radius) * 2 + 1f;

                        targetPos = msg.Position + right * (relativeIndex * spacing);

                        break;
                    }
                case SpreadFormation.V:
                    {
                        var team = Team.Members.Where(p => p.Identity != msg.Sender).OrderBy(p => p.Name).ToList();

                        int index = team.FindIndex(p => p.Identity == player.Identity);
                        if (index == -1)
                            return;

                        float spacing = (msg.SenderRadius + player.Radius) * 2 + 1f;

                        Vector3 back = msg.Rotation * new Vector3(0f, 0f, -1f);
                        Vector3 right = msg.Rotation * Vector3.Right;

                        int side = index % 2 == 0 ? 1 : -1;
                        int depth = index / 2 + 1;

                        Vector3 offset = back * (depth * spacing) + right * (side * depth * spacing);

                        targetPos = msg.Position + offset;

                        break;
                    }
                case SpreadFormation.Random:
                default:
                    {
                        targetPos.AddRandomness((int)3.0f);
                        break;
                    }
            }

            float distance = player.Position.Distance2DFrom(targetPos);

            if (distance < 10 && distance > 1)
            {
                player.Position = targetPos;
                player.Rotation = msg.Rotation;
                MovementController.Instance.SetMovement(MovementAction.JumpStart);
            }
        }

        #endregion

        #region OnNpcChatOpenClose

        public static void OnNpcChatOpenClose(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["SyncChat"].AsBool()) return;
            if (IsActiveWindow) return;

            var openCloseMsg = (NpcChatOpenCloseIPCMessage)msg;
            if (openCloseMsg.Sender == DynelManager.LocalPlayer.Identity) return;

            if (openCloseMsg.OpenClose)
            {
                NpcDialog.Open(openCloseMsg.Target);
            }
            else
            {
                Network.Send(new KnuBotCloseChatWindowMessage
                {
                    Unknown1 = 2,
                    Target = openCloseMsg.Target
                });
            }
        }

        #endregion

        #region OnNpcChatAnswer

        public static void OnNpcChatAnswer(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["SyncChat"].AsBool()) return;
            if (IsActiveWindow) return;

            var answerMsg = (NpcChatAnswerIPCMessage)msg;
            if (answerMsg.Sender == DynelManager.LocalPlayer.Identity) return;

            NpcDialog.SelectAnswer(answerMsg.Target, answerMsg.Answer);
        }

        #endregion

        #region OnNPCStartTrade

        public static void OnNPCStartTrade(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["NPCTrade"].AsBool()) return;
            if (IsActiveWindow) return;
            var NPCStartTrade = (NPCStartTradeIPCMessage)msg;
            if (NPCStartTrade.Sender == DynelManager.LocalPlayer.Identity) return;
            NPCChatStartTrade(DynelManager.LocalPlayer.Identity, NPCStartTrade.Target);
        }

        #endregion

        #region OnNPCTrade

        public static void OnNPCTrade(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["NPCTrade"].AsBool()) return;
            if (IsActiveWindow) return;

            var NpcTrade = (NpcTradeIPCMessage)msg;
            if (NpcTrade.Sender == DynelManager.LocalPlayer.Identity) return;

            var item = Inventory.Items.FirstOrDefault(i => i.Id == NpcTrade.Id);
            if (item == null) return;
            NPCChatAddTradeItem(DynelManager.LocalPlayer.Identity, NpcTrade.Target, item.Slot);
        }

        #endregion

        #region OnNPCFinishTrade

        public static void OnNPCFinishTrade(int sender, IPCMessage msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!_settings["NPCTrade"].AsBool()) return;
            if (IsActiveWindow) return;

            var NpcFinishTrade = (NpcFinishTradeMessage)msg;
            if (NpcFinishTrade.Sender == DynelManager.LocalPlayer.Identity) return;
            switch (NpcFinishTrade.Decline)
            {
                case 0:
                    NPCChatEndTrade(DynelManager.LocalPlayer.Identity, NpcFinishTrade.Target, NpcFinishTrade.Amount);
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Outside Invites

        internal static void OutsideInvitesRecieved(int arg1, IPCMessage message)
        {
            var invite = (OutsideInvitesIPCMessage)message;

            _settings["OutsideTeamInvites"] = invite.Invites;

            if (settingsWindow?.IsValid == true)
            {
                if (settingsWindow.FindView("OutsideTeamInvites", out Button invitesButton))
                    invitesButton.SetLabel($"Outside invites is {(_settings["OutsideTeamInvites"].AsBool() ? "on" : "off")}");
            }
        }

        #endregion

        internal static void Battle_Station_Message_Received(int arg1, IPCMessage message)
        {
            if (!(message is BattleStationMessage msg)) return;

            switch (msg.Action)
            {
                case (int)CharacterActionType.JoinBattlestationQueue:
                    Network.Send(new CharacterActionMessage
                    {
                        Action = CharacterActionType.JoinBattlestationQueue,
                        Target = DynelManager.LocalPlayer.Identity,
                        Parameter2 = _settings["BSSideSelection"].AsInt32()
                    });
                    break;
                case (int)CharacterActionType.LeaveBattlestationQueue:
                    Network.Send(new CharacterActionMessage
                    {
                        Action = CharacterActionType.LeaveBattlestationQueue,
                        Target = DynelManager.LocalPlayer.Identity
                    });
                    break;
            }
        }

        #endregion

        #region Sync Use Tick

        public static void HandleSyncUseTick()
        {
            try
            {
                if (UseMsgAction == GenericCmdAction.None) return;
                if (DynelManager.LocalPlayer.Identity == UseMsgSender) return;

                if (Item.HasPendingUse) return;

                if (PerkAction.List.Any(perk => perk.IsExecuting || perk.IsPending)) return;

                if (Spell.HasPendingCast) return;
                if (Spell.Find("Composite Attribute Boost", out Spell spell) && !spell.IsReady) return;

                var playerPos = DynelManager.LocalPlayer.Position;
                Dynel target = null;

                if (UseMsgTarget != Identity.None)
                    target = DynelManager.AllDynels.FirstOrDefault(x => x != null && x.Identity == UseMsgTarget
                            && playerPos.DistanceFrom(x.Position) < 8 && x.Name != "Rubi-Ka Banking Service Terminal" && x.Name != "Mail Terminal");

                switch (UseMsgAction)
                {
                    case GenericCmdAction.Use:
                        if (UseMsgTarget != Identity.None)
                            target?.Use();
                        else
                            UseMsgItem?.Use();
                        break;
                    case GenericCmdAction.UseItemOnItem:
                        if (UseMsgTarget != Identity.None)
                            UseMsgItem?.UseOn(target);
                        break;
                    case GenericCmdAction.UseItemOnCharacter:
                        Network.Send(new GenericCmdMessage()
                        {
                            Identity = DynelManager.LocalPlayer.Identity,
                            Action = UseMsgAction,
                            User = DynelManager.LocalPlayer.Identity,
                            Target = UseMsgTarget,
                            Source = UseMsgItem.Slot,
                        });
                        break;
                }

                UseMsgTarget = Identity.None;
                UseMsgItem = null;
                UseMsgSender = Identity.None;
                UseMsgAction = GenericCmdAction.None;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #endregion

        internal static void TeamRequestReceived(object sender, TeamRequestEventArgs e)
        {
            if (e.Requester == TeamSender || _settings["OutsideTeamInvites"].AsBool())
            {
                e.Accept();
                return;
            }

            //e.Decline();
        }
        internal static void LFT_Message_Received(int arg1, IPCMessage message)
        {
            if (!(message is LFTMessage msg)) return;
            if (msg.Sender == DynelManager.LocalPlayer.Identity) return;

            string text = "";

            if (msg.TextLength > 0)
            {
                byte[] bytes = new byte[msg.TextLength];

                for (int i = 0; i < msg.TextLength; i++)
                {
                    int seg = i / 4;
                    int shift = (i % 4) * 8;
                    int src = 0;

                    if (seg == 0)
                        src = msg.TextPart0;
                    else if (seg == 1)
                        src = msg.TextPart1;
                    else if (seg == 2)
                        src = msg.TextPart2;
                    else if (seg == 3)
                        src = msg.TextPart3;

                    bytes[i] = (byte)((src >> shift) & 0xFF);
                }

                text = System.Text.Encoding.ASCII.GetString(bytes);
                Chat.WriteLine($"RECV LFT text: '{text}'");
            }

            if (Team.IsInTeam)
                Team.Leave();

            _settings["OutsideTeamInvites"] = true;

            if (settingsWindow?.IsValid == true)
            {
                if (settingsWindow.FindView("OutsideTeamInvites", out Button invitesButton))
                    invitesButton.SetLabel($"Outside invites is {(_settings["OutsideTeamInvites"].AsBool() ? "on" : "off")}");
            }

            LookingForTeam.Join(text);
        }

        #region Helpers

        public static void FindUseRing(string itemName)
        {
            if (itemName == null)
                return;

            var ring = Inventory.Items.FirstOrDefault(c => c.Name.Contains(itemName));
            if (ring != null)
            {
                UseMsgItem = ring;
                return;
            }

            ring = Inventory.Backpacks.SelectMany(b => b.Items).FirstOrDefault(c => c.Name.Contains(itemName));
            if (ring != null)
            {
                UseMsgItem = ring;
                return;
            }
        }

        public static string GetItemNameFromRingName(RingName ringName)
        {
            if (_ringMap.TryGetValue(ringName, out var itemName))
                return itemName;

            return null;
        }

        public static Item ProcessIDItem(int instance)
        {
            int[] ignoredItemIds = {
                301679, 85907, 85908, 267167, 305478, 206013, 204653, 245990, 204698, 206015,
                305476, 156576, 164780, 164781, 244204, 245323, 244214, 244216, 204593,
                305493, 204595, 305491, 204598, 305495, 157296, 303179, 267168, 244655,
                152028, 253187, 151693, 83919, 152029, 151692, 253186, 83920, 291043,
                204103, 204104, 204105, 204106, 204107, 303138, 303141, 303137
            };

            if (ignoredItemIds.Contains(instance))
                return null;

            int[] ICCModifiedHackingTool = { 273512, 273513, 273514, 273515, 273516, 273517, 273230 };
            int[] ZeroPointTransmissionRelayScoop = { 275035, 375038, 275039, 275040, 275042 };

            if (ICCModifiedHackingTool.Contains(instance))
            {
                if (Inventory.Find("ICC Modified Hacking Tool", out Item tool, false))
                    return tool;
            }

            if (ZeroPointTransmissionRelayScoop.Contains(instance))
            {
                if (Inventory.Find("Zero-Point Transmission Relay Scoop", out Item tool, false))
                    return tool;
            }

            var inventoryItem = Inventory.Items.FirstOrDefault(i => i.Id == instance);
            if (inventoryItem != null)
                return inventoryItem;

            var backpackItem = Inventory.Backpacks.SelectMany(b => b.Items).FirstOrDefault(i => i.Id == instance);
            if (backpackItem != null)
                return backpackItem;

            return null;
        }

        #endregion
    }
}
