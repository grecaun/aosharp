using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Agent
{
    [AoContract(2346)]
    public class PetWarpMessage : IPCMessage
    {
        public override short Opcode => (short)2346;
    }
}
