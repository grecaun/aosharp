using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace Manager.POH
{
    [AoContract((int)IPCOpcode.POHPathing)]
    internal class POHPathing : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.POHPathing;

        [AoMember(0)] public Vector3 Position { get; set; }
        [AoMember(1)] public bool ShouldPath { get; set; }
    }
}
