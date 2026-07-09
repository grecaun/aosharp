using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Soldier
{
    [AoContract((int)IPCOpcode.Holds)]
    public class HoldsMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Holds;
        [AoMember(0)] public bool HoldsBool { get; set; }
    }
}