using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonRoamba.IPCMessages
{
    [AoContract((int)IPCOpcode.Enabled)]
    public class EnabledIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Enabled;

        [AoMember(0)]
        public bool SetEnabled { get; set; }

        [AoMember(1, SerializeSize = ArraySizeType.Byte)]
        public string PathFolderPath { get; set; }

        [AoMember(2, SerializeSize = ArraySizeType.Byte)]
        public string ConfigFolderPath { get; set; }

        [AoMember(3)]
        public bool SyncPath { get; set; }

        [AoMember(4)]
        public bool SyncConfig { get; set; }
    }
}
