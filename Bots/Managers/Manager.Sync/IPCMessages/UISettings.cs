using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.UISettings)]
    internal class UISettings : IPCMessage
    {
        [AoMember(0)] public bool Use { get; set; }
        [AoMember(1)] public bool Bags { get; set; }
        [AoMember(2)] public bool Chat { get; set; }
        [AoMember(3)] public bool NpcTrade { get; set; }
        [AoMember(4)] public bool Trade { get; set; }
        [AoMember(5)] public bool Attack { get; set; }
        [AoMember(6)] public Identity Sender { get; set; }
        [AoMember(7)] public bool Log { get; set; }
        [AoMember(8)] public bool Move { get; set; }
        [AoMember(9)] public int Side { get; set; }
    }
}
