using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerHelp.IPCMessages
{
    [AoContract((int)IPCOpcode.Grid)]
    public class UseGridMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Grid;
    }
}
