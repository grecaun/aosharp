using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Agent
{
    [AoContract((int)IPCOpcode.RootSnare)]
    public class RootSnareMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.RootSnare;
        [AoMember(0)] public Identity Target { get; set; }
    }
}
