using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Engineer
{
    [AoContract(2345)]
    public class PetWaitMessage : IPCMessage
    {
        public override short Opcode => (short)2345;
    }
}
