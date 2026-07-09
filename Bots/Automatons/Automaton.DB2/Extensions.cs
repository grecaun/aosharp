using AOSharp.Common.GameData;
using AOSharp.Core;
using System.Linq;

namespace AutomatonDB2
{
    public static class Extensions
    {
        public static bool CanProceed()
        {
            return DynelManager.LocalPlayer.HealthPercent > 65 && DynelManager.LocalPlayer.NanoPercent > 65
                && DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1
                && DynelManager.LocalPlayer.MovementState != MovementState.Sit
                && !Spell.HasPendingCast
                && !Debuffed();
        }

        public static bool Debuffed()
        {
            var buffs = DynelManager.LocalPlayer.Buffs;

            return buffs.Contains(AutomatonDB2.Nanos.SeismicActivity)
                || buffs.Contains(AutomatonDB2.Nanos.SpatialDisplacement)
                || buffs.Contains(AutomatonDB2.Nanos.MachineShockwave)
                || buffs.Contains(AutomatonDB2.Nanos.ActivatingtheMachine)
                || buffs.Contains(AutomatonDB2.Nanos.Stunned)
                || buffs.Contains(AutomatonDB2.Nanos.TheMachineGrindstoLife);
        }
    }
}
