using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.MartialArtist
{
    [AoContract((int)IPCOpcode.AOE)]
    public class AOEMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.AOE;
        [AoMember(0)] public bool AOEBool { get; set; }
    }
}
