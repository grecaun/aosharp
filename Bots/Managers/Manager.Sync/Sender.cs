using AOSharp.Common.GameData;
using AOSharp.Core;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.Messages;
using static ManagerSync.ManagerSync;
using ManagerSync.IPCMessages;
using AOSharp.Core.Inventory;
using System.Linq;
using System.Collections.Generic;
using AOSharp.Core.UI;

namespace ManagerSync
{
    internal static class Sender
    {
        public static List<Item> ItemsInInventory = new List<Item>();

        public static void Network_N3MessageSent(object s, N3Message n3Msg)
        {
            if (!_settings["Enable"].AsBool()) return;
            if (!IsActiveWindow) return;
            if (n3Msg.Identity != DynelManager.LocalPlayer.Identity) return;

            switch (n3Msg.N3MessageType)
            {
                #region Sync LookAt
                case N3MessageType.LookAt:
                    var lookAtMsg = (LookAtMessage)n3Msg;
                    _IPCChannel.Broadcast(new TargetMessage
                    {
                        Target = lookAtMsg.Target,
                        Sender = DynelManager.LocalPlayer.Identity
                    });
                    break;
                #endregion

                #region Sync Move
                case N3MessageType.CharDCMove:
                case N3MessageType.CharacterAction:
                    
                    if (n3Msg is CharDCMoveMessage charDCMoveMsg)
                    {
                        if (!_settings["SyncMove"].AsBool()) return;
                        _IPCChannel.Broadcast(new MoveMessage
                        {
                            MoveType = charDCMoveMsg.MoveType,
                            PlayfieldId = Playfield.Identity.Instance,
                            Position = charDCMoveMsg.Position,
                            Rotation = charDCMoveMsg.Heading,
                            Sender = DynelManager.LocalPlayer.Identity
                        });
                    }
                    else if (n3Msg is CharacterActionMessage charActionMsg)
                    {
                        switch (charActionMsg.Action)
                        {
                            case CharacterActionType.StandUp:
                                if (!_settings["SyncMove"].AsBool()) return;
                                _IPCChannel.Broadcast(new MoveMessage
                                {
                                    MoveType = MovementAction.LeaveSit,
                                    PlayfieldId = Playfield.Identity.Instance,
                                    Position = DynelManager.LocalPlayer.Position,
                                    Rotation = DynelManager.LocalPlayer.Rotation,
                                    Sender = DynelManager.LocalPlayer.Identity
                                });
                                break;
                            case CharacterActionType.JoinBattlestationQueue:
                                if (!_settings["SyncUse"].AsBool()) return;
                                _IPCChannel.Broadcast(new BattleStationMessage { Action = 253 } );
                                break;
                            case CharacterActionType.LeaveBattlestationQueue:
                                if (!_settings["SyncUse"].AsBool()) return;
                                _IPCChannel.Broadcast(new BattleStationMessage { Action = 255 });
                                break;
                        }
                    }
                    break;
                #endregion

                #region Sync Attack
                case N3MessageType.Attack:
                case N3MessageType.StopFight:
                    if (!_settings["SyncAttack"].AsBool()) return;

                    if (n3Msg.N3MessageType == N3MessageType.Attack)
                    {
                        var attackMsg = (AttackMessage)n3Msg;
                        _IPCChannel.Broadcast(new AttackIPCMessage
                        {
                            Target = attackMsg.Target,
                            Start = true,
                            Sender = DynelManager.LocalPlayer.Identity
                        });
                    }
                    else
                    {
                        _IPCChannel.Broadcast(new AttackIPCMessage
                        {
                            Target = Identity.None,
                            Start = false,
                            Sender = DynelManager.LocalPlayer.Identity
                        });
                    }
                    break;
                #endregion

                #region Sync Use
                case N3MessageType.GenericCmd:
                    if (!_settings["SyncUse"].AsBool()) return;

                    var genericCmdMsg = (GenericCmdMessage)n3Msg;
                    var action = genericCmdMsg.Action;
                    var sourceId = genericCmdMsg.Source;
                    var targetId = genericCmdMsg.Target;

                    var sendCommand = new UseMessage
                    {
                        Action = action,
                        Sender = DynelManager.LocalPlayer.Identity,
                        Playfield = Playfield.ModelIdentity.Instance
                    };

                    switch (action)
                    {
                        case GenericCmdAction.Use:
                            if (targetId.Type == IdentityType.Terminal)
                            {
                                sendCommand.Item_Instance = 0;
                                sendCommand.Target = targetId;
                            }
                            else if (targetId.Type == IdentityType.Inventory || targetId.Type == IdentityType.Backpack || targetId.Type == IdentityType.Container)
                            {
                                var useItem = FindItem(targetId);
                                if (useItem == null) return;
                                if (IsOther(useItem)) return;

                                sendCommand.Item_Instance = useItem.Id;
                                sendCommand.Target = Identity.None;
                            }
                            else
                            {
                                return;
                            }
                            break;

                        case GenericCmdAction.UseItemOnItem:
                            if (!sourceId.HasValue) return;

                            var sourceUseItem = FindItem(sourceId.Value);
                            if (sourceUseItem == null) return;

                            var ringName = GetRingNameFromItemName(sourceUseItem.Name);
                            sendCommand.Item_Instance = ringName != RingName.Unknown ? 0 : sourceUseItem.Id;
                            sendCommand.RingName = ringName;
                            sendCommand.Target = targetId;
                            break;

                        case GenericCmdAction.UseItemOnCharacter:
                            if (!sourceId.HasValue) return;

                            var characterItem = FindItem(sourceId.Value);
                            if (characterItem == null) return;

                            sendCommand.Item_Instance = characterItem.Id;
                            sendCommand.Target = targetId;
                            break;

                        default:
                            return;
                    }

                    _IPCChannel.Broadcast(sendCommand);
                    break;

                case N3MessageType.GridSelected:
                    if (!_settings["SyncUse"].AsBool()) return;
                    var cityWindow = (GridSelectedMessage)n3Msg;
                    var msg = new GridCitySelectionMessage
                    {
                        Sender = DynelManager.LocalPlayer.Identity,
                        Selection = cityWindow.Destination,
                    };
                    _IPCChannel.Broadcast(msg);
                    break;

                #endregion

                #region Sync Chat
                case N3MessageType.KnubotOpenChatWindow:
                case N3MessageType.KnubotCloseChatWindow:
                    if (!_settings["SyncChat"].AsBool()) return;

                    var openCloseMsg = n3Msg is KnuBotOpenChatWindowMessage openMsg
                        ? new NpcChatOpenCloseIPCMessage
                        {
                            Target = openMsg.Target,
                            OpenClose = true,
                            Sender = DynelManager.LocalPlayer.Identity
                        }
                        : new NpcChatOpenCloseIPCMessage
                        {
                            Target = ((KnuBotCloseChatWindowMessage)n3Msg).Target,
                            OpenClose = false,
                            Sender = DynelManager.LocalPlayer.Identity
                        };

                    _IPCChannel.Broadcast(openCloseMsg);
                    break;

                case N3MessageType.KnubotAnswer:
                    if (!_settings["SyncChat"].AsBool()) return;

                    var answerMsg = (KnuBotAnswerMessage)n3Msg;

                    _IPCChannel.Broadcast(new NpcChatAnswerIPCMessage
                    {
                        Target = answerMsg.Target,
                        Answer = answerMsg.Answer,
                        Sender = DynelManager.LocalPlayer.Identity
                    });
                    break;
                #endregion

                #region NPC Trade
                case N3MessageType.KnubotStartTrade:
                case N3MessageType.KnubotTrade:
                case N3MessageType.KnubotFinishTrade:
                    if (!_settings["NPCTrade"].AsBool()) return;

                    if (n3Msg is KnuBotStartTradeMessage start)
                    {
                        _IPCChannel.Broadcast(new NPCStartTradeIPCMessage
                        {
                            Target = start.Target,
                            Sender = DynelManager.LocalPlayer.Identity
                        });
                    }
                    else if (n3Msg is KnuBotTradeMessage trade)
                    {
                        var item = ItemsInInventory.FirstOrDefault(i => i.Slot.Instance == trade.Slot.Instance);
                        if (item == null) return;

                        _IPCChannel.Broadcast(new NpcTradeIPCMessage
                        {
                            Id = item.Id,
                            Target = trade.Target,
                            Sender = DynelManager.LocalPlayer.Identity
                        });
                    }
                    else if (n3Msg is KnuBotFinishTradeMessage finish)
                    {
                        _IPCChannel.Broadcast(new NpcFinishTradeMessage
                        {
                            Target = finish.Target,
                            Decline = finish.Decline,
                            Amount = finish.Amount,
                            Sender = DynelManager.LocalPlayer.Identity
                        });

                        ItemsInInventory.Clear();
                    }
                    break;
                    #endregion
            }
        }

        #region Helpers

        static bool IsOther(Item item)
        {
            return item.Id == 305476 || item.Id == 204698 || item.Id == 156576 || item.Id == 267168 || item.Id == 267167
                || item.Id == 204593 || item.Id == 305492 || item.Id == 204595 || item.Id == 305491 || item.Id == 305478
                || item.Id == 206013 || item.Id == 204653 || item.Id == 204698 || item.Id == 206015 || item.Id == 305476
                || item.Id == 267168 || item.Id == 267167 || item.Name.Contains("Health") || item.Name.Contains("Newcomer")
                || item.Name.Contains("Stim") || item.Name.Contains("syncbag")|| item.Name.Contains("Portable Bank Terminal") || item.UniqueIdentity.Type == IdentityType.Container;
        }

        public static Item FindItem(Identity target)
        {
            return Inventory.Find(target, out Item item) ? item :
                   Inventory.Backpacks.SelectMany(b => b.Items).FirstOrDefault(i => i.Slot.Instance == target.Instance);
        }

        #endregion
    }
}
