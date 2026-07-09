using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerHelp.IPCMessages
{
    [AoContract((int)IPCOpcode.YalmSelection)]

    internal class BroadcastYalms : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.YalmSelection;

        [AoMember(0)] public int RKSelection { get; set; }
        [AoMember(1)] public int SLSelection { get; set; }
    }
}
