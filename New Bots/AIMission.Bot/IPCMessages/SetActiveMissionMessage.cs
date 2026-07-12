using AIMission.Bot;
using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;
using System.Collections.Generic;

namespace AIMission.Bot.IPCMessages
{
    [AoContract((int)AIMissionBotIPCOpcode.SetActiveMission)]
    public class SetActiveMissionMessage : IPCMessage
    {
        [AoMember(0)]
        public Identity Identity { get; set; }
    }

    [AoContract((int)AIMissionBotIPCOpcode.SendSettings)]
    public class SendSettingsMessage : IPCMessage
    {
        [AoMember(0)]
        public MissionDifficulty Difficulty { get; set; }
        [AoMember(1)]
        public Dictionary<string, bool> Bosses { get; set; }
        [AoMember(2)]
        public bool ClearCoccoons { get; set; }
    }
}
