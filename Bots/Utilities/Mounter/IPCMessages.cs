using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace Mounter
{
    public partial class Main
    {
        private enum IPCOpcode
        {
            Mount = 8700,
            Dismount = 8701,
        }


        [AoContract((int)IPCOpcode.Mount)]
        private class MountMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.Mount;
        }

        [AoContract((int)IPCOpcode.Dismount)]
        private class DismountMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.Mount;
        }
    }
}
