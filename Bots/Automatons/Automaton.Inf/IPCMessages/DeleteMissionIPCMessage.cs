using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace AutomatonInf.IPCMessages
{
    [AoContract((int)IPCOpcode.DeleteMission)]
    public class DeleteMissionIPCMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.DeleteMission;
    }
}
