using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.NPCStartTrade)]
    public class NPCStartTradeIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.NPCStartTrade;

        [AoMember(0)]
        public Identity Target { get; set; }

        [AoMember(1)]
        public Identity Sender { get; set; }

    }
}
