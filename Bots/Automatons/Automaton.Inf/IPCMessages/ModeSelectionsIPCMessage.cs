using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonInf.IPCMessages
{
    [AoContract((int)IPCOpcode.ModeSelections)]
    public class ModeSelectionsIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.ModeSelections;

        [AoMember(0)] public int Mode { get; set; }
        [AoMember(1)] public int Faction { get; set; }
        [AoMember(2)] public int Difficulty { get; set; }
        [AoMember(3)] public bool Clear { get; set; }
        [AoMember(4)] public Identity Sender { get; set; }
    }
}
