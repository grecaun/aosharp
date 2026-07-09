using AOSharp.Common.GameData;
using AOSharp.Core;
using System.Linq;

namespace AutomatonMitaar
{
    public static class Extensions
    {
        public static bool Debuffed()
        {
            return DynelManager.LocalPlayer.Buffs.Contains(267283)
                || DynelManager.LocalPlayer.Buffs.Contains(280469)
                || DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Cocoon) // coon
                || DynelManager.LocalPlayer.Buffs.Contains(280470)
                || DynelManager.LocalPlayer.Buffs.Contains(280488);
        }

        public static bool CanProceed()
        {
            return DynelManager.LocalPlayer.HealthPercent > 65 && DynelManager.LocalPlayer.NanoPercent > 65
                && DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1
                && DynelManager.LocalPlayer.MovementState != MovementState.Sit
                && !Spell.HasPendingCast
                && !Debuffed();
        }
    }
}
