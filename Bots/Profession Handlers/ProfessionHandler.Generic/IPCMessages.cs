using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        private enum IPCOpcode
        {
            RemainingNCU = 2000,
            SettingsUpdate = 2001,
            ClearBuffs = 2002,
            AOE = 2003,
            Holds = 2004,
            RootSnare = 2005,
            Specials = 2006,
        }


        [AoContract((int)IPCOpcode.RemainingNCU)]
        private class RemainingNCUMessage : IPCMessage
        {
            public static RemainingNCUMessage ForLocalPlayer()
            {
                return new RemainingNCUMessage()
                {
                    Character = DynelManager.LocalPlayer.Identity,
                    RemainingNCU = DynelManager.LocalPlayer.RemainingNCU,
                };
            }

            public override short Opcode => (short)IPCOpcode.RemainingNCU;

            [AoMember(0)]
            public Identity Character { get; set; }

            [AoMember(1)]
            public int RemainingNCU { get; set; }
        }

        [AoContract((int)IPCOpcode.SettingsUpdate)]
        private class SettingsUpdateMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.SettingsUpdate;

            [AoMember(0)] public bool Buffing { get; set; }
            [AoMember(1)] public bool Comps { get; set; }
            [AoMember(2)] public bool Wait_For_Rez { get; set; }
        }

        [AoContract((int)IPCOpcode.ClearBuffs)]
        private class ClearBuffsMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.ClearBuffs;
            [AoMember(0)] public int Type { get; set; }
        }

        [AoContract((int)IPCOpcode.AOE)]
        private class AOEMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.AOE;

            [AoMember(0)] public bool AOEBool { get; set; }
        }

        [AoContract((int)IPCOpcode.Holds)]
        private class HoldsMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.Holds;

            [AoMember(0)] public bool HoldsBool { get; set; }
        }

        [AoContract((int)IPCOpcode.RootSnare)]
        private class RootSnareMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.RootSnare;

            [AoMember(0)] public Identity Target { get; set; }
        }

        [AoContract((int)IPCOpcode.Specials)]
        private class SpecialsMessage : IPCMessage
        {
            public override short Opcode => (short)IPCOpcode.Specials;

            [AoMember(0)] public bool Specials { get; set; }
        }

        [AoContract(2344)]
        private class PetAttackMessage : IPCMessage
        {
            public override short Opcode => (short)2344;

            [AoMember(0)]
            public Identity Target { get; set; }
        }

        [AoContract(2346)]
        private class PetWarpMessage : IPCMessage
        {
            public override short Opcode => (short)2346;
        }

        [AoContract(2347)]
        private class PetFollowMessage : IPCMessage
        {
            public override short Opcode => (short)2347;
        }

        [AoContract(2348)]
        private class PetSync_On_Off_Message : IPCMessage
        {
            public override short Opcode => (short)2348;

            [AoMember(0)]
            public bool Sync_On_Off { get; set; }

            [AoMember(1)]
            public Identity Sender { get; set; }
        }

        [AoContract(2345)]
        private class PetWaitMessage : IPCMessage
        {
            public override short Opcode => (short)2345;
        }
    }
}
