using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Bureaucrat
{
    [AoContract(2345)]
    public class PetWaitMessage : IPCMessage
    {
        public override short Opcode => (short)2345;
    }
}
