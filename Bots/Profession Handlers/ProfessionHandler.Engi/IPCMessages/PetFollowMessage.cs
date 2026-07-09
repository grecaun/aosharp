using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Engineer
{
    [AoContract(2347)]
    public class PetFollowMessage : IPCMessage
    {
        public override short Opcode => (short)2347;
    }
}
