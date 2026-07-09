using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.NpcChatAnswer)]
    public class NpcChatAnswerIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.NpcChatAnswer;

        [AoMember(0)]
        public Identity Target { get; set; }

        [AoMember(1)]
        public int Answer { get; set; }

        [AoMember(2)]
        public Identity Sender { get; set; }
    }
}
