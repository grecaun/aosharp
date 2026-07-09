using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerHelp.IPCMessages
{
    [AoContract((int)IPCOpcode.fly)]

    internal class FlyMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.fly;

        [AoMember(0)] public int Action { get; set; }
    }
}
