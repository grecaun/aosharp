using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.BattleStation)]
    public class BattleStationMessage : IPCMessage
    {
        [AoMember(0)] public int Action { get; set; }
    }
}
