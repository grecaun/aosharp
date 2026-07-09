using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonKiteHill.IPCMessages
{
    [AoContract((int)IPCOpcode.SideSelections)]
    public class SideSelectionsIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.SideSelections;

        [AoMember(0)] public int Side { get; set; }
       
    }
}