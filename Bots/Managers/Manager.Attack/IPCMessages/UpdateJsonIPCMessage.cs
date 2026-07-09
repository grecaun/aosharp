using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerAttack.IPCMessages
{
    [AoContract((int)IPCOpcode.Update)]
    public class UpdateJsonIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Update;
    }
}