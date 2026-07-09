using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Bureaucrat
{
    [AoContract((int)IPCOpcode.SettingsUpdate)]
    public class SettingsUpdateMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.SettingsUpdate;

        [AoMember(0)] public bool Buffing {  get; set; }
        [AoMember(1)] public bool Comps {  get; set; }
        [AoMember(2)] public bool Wait_For_Rez { get; set; }
    }
}

