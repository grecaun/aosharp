using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonCity.IPCMessages
{
    [AoContract((int)IPCOpcode.SettingsUpdate)]
    public class SettingsUpdateMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.SettingsUpdate;

        [AoMember(5)] public bool Include_Ship { get; set; }
    }
}

