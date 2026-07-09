using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    public enum SpreadFormation
    {
        Random = 0,
        Line = 1,
        Circle = 2,
        Square = 3,
        SingleFile = 4,
        V = 5,
    }
    [AoContract((int)IPCOpcode.Spread)]
    public class SpreadCommand : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Spread;

        [AoMember(0)]
        public Vector3 Position { get; set; }

        [AoMember(1)]
        public int Instance { get; set; }

        [AoMember(2)]
        public Quaternion Rotation { get; set; }

        [AoMember(3)]
        public float SenderRadius { get; set; }

        [AoMember(4)]
        public SpreadFormation FormationType { get; set; } = SpreadFormation.Random;
        [AoMember(5)]
        public Identity Sender { get; set; }
    }
}
