using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonKiteHill.IPCMessages
{
    [AoContract((int)IPCOpcode.IdentityInfo)]
    public class IdentityMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.IdentityInfo;
        [AoMember(0)] public Identity TankIdentity { get; set; }
        [AoMember(1)] public Identity NTIdentity { get; set; }
    }
}
