using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.LogOut)]
    public class LogOutIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.LogOut;

        [AoMember(0)]
        public Identity Sender { get; set; }

        [AoMember(1)]
        public Identity Target { get; set; }
    }
}
