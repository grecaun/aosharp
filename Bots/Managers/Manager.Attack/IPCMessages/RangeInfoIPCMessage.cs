using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerAttack.IPCMessages
{
    [AoContract((int)IPCOpcode.RangeInfo)]
    public class RangeInfoIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.RangeInfo;

        [AoMember(0)]
        public int AttackRange { get; set; }

        [AoMember(1)]
        public int TauntRange { get; set; }

        [AoMember(2)]
        public Identity Sender { get; set; }
    }
}
