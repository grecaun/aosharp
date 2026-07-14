using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.Plant)]
    public class PlantCommand : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Plant;

        [AoMember(0)]
        public Vector3 Position { get; set; }

        [AoMember(1)]
        public Identity Receiver { get; set; }

        [AoMember(2)]
        public Identity Sender { get; set; }
    }
}
