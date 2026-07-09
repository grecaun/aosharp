using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerAttack.IPCMessages
{
    [AoContract((int)IPCOpcode.LeaderInfo)]
    public class LeaderInfoIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.LeaderInfo;

        [AoMember(0)]
        public Identity LeaderIdentity { get; set; }

        [AoMember(1)]
        public int Request { get; set; }

        // 0 no, 1 yes, 3 other
    }
}
