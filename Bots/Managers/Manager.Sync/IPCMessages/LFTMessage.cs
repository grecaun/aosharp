using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.LFTCode)]
    public class LFTMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.LFTCode;

        [AoMember(0)] public Identity Sender { get; set; }
        [AoMember(1)] public byte TextLength { get; set; }
        [AoMember(2)] public int TextPart0 { get; set; }
        [AoMember(3)] public int TextPart1 { get; set; }
        [AoMember(4)] public int TextPart2 { get; set; }
        [AoMember(5)] public int TextPart3 { get; set; }
    }
}
