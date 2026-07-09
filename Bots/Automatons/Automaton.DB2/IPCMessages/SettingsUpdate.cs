using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonDB2.IPCMessages
{

    [AoContract((int)IPCOpcode.SettingsUpdate)]
    public class SettingsUpdateMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.SettingsUpdate;

        [AoMember(5)] public bool Farming { get; set; }
    }
}
