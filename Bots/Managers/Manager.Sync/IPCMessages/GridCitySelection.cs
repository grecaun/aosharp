using AOSharp.Common.GameData;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ManagerSync.IPCMessages
{
    [AoContract((int)IPCOpcode.Grid)]
    public class GridCitySelectionMessage : IPCMessage
    {
        public override short Opcode => (short)IPCOpcode.Grid;

        [AoMember(0)]
        public Identity Sender { get; set; }

        [AoMember(1)]
        public DestinationInfo Selection { get; set; }

    }
}
