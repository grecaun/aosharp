using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.OSInvites)]

    public class OutsideInvitesIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.OSInvites;
        [AoMember(0)] public bool Invites { get; set; }
    }
}
