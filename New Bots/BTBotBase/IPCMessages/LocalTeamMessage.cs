using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AOBotBase.IPCMessages
{
    [AoContract((int)BTBotBaseIPCOpcode.Team)]
    internal class LocalTeamMessage : IPCMessage
    {
        public override short Opcode => (int)BTBotBaseIPCOpcode.Team;

        [AoMember(0)]
        public Identity Sender { get; set; }

        [AoMember(1)]
        public int TeamAction { get; set; }

        [AoMember(2)]
        public Identity Receiver { get; set; }
    }
}
