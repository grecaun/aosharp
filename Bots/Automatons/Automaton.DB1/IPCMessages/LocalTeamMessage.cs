using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonDB1.IPCMessages
{
    [AoContract((int)IPCOpcode.Team)]
    public class LocalTeamMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Team;

        [AoMember(0)]
        public Identity Sender { get; set; }

        [AoMember(1)]
        public int TeamAction { get; set; }

        [AoMember(2)]
        public Identity Receiver { get; set; }
    }
}
