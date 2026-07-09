using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;
using static ManagerSync.ManagerSync;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.Use)]
    public class UseMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Use;

        [AoMember (0)]
        public GenericCmdAction Action { get; set; }

        [AoMember(1)]
        public int Item_Instance { get; set; }

        [AoMember(2)]
        public Identity Target { get; set; }

        [AoMember(3)]
        public RingName RingName { get; set; }

        [AoMember(4)]
        public Identity Sender { get; set; }

        [AoMember(5)]
        public int Playfield { get; set; }
    }
}
