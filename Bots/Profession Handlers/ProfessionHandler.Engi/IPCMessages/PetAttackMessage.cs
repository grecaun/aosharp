using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Engineer
{
    [AoContract(2344)]
    public class PetAttackMessage : IPCMessage
    {
        public override short Opcode => (short)2344;

        [AoMember(0)]
        public Identity Target { get; set; }
    }
}
