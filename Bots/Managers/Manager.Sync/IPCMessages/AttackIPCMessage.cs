using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.Attack)]
    public class AttackIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Attack;

        [AoMember(0)]
        public Identity Target { get; set; }

        [AoMember(1)]
        public bool Start { get; set; }

        [AoMember(2)]
        public Identity Sender { get; set; }

    }
}
