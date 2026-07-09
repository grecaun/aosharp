using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerFollow.IPCMessages
{
    [AoContract((int)IPCOpcode.Follow)]
    public class FollowMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Follow;

        [AoMember(0)]
        public Identity Target { get; set; }

        [AoMember(1)]
        public int Distance { get; set; }

        [AoMember(2)]
        public Identity Sender { get; set; }
    }
}
