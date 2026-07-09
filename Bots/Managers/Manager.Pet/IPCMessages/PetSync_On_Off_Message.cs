using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerPet.IPCMessages
{
    [AoContract((int)IPCOpcode.PetSync)]
    public class PetSync_On_Off_Message : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.PetSync;

        [AoMember(0)]
        public bool Sync_On_Off { get; set; }

        [AoMember(1)]
        public Identity Sender { get; set; }
    }
}
