using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonDB2.IPCMessages
{
    [AoContract((int)IPCOpcode.LeaderInfo)]
    public class LeaderInfoIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.LeaderInfo;
        [AoMember(0)] public Identity LeaderIdentity { get; set; }
        [AoMember(1)] public bool LeaderChanger { get; set; }
        [AoMember(2)] public Identity Sender { get; set; }
    }
}
