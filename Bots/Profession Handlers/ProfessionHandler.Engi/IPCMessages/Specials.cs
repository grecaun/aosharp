using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Engineer
{
    [AoContract((int)IPCOpcode.Specials)]
    public class SpecialsMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Specials;

        [AoMember(0)] public bool Specials { get; set; }
    }
}
