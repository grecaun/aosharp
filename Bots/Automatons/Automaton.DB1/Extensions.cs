using AOSharp.Common.GameData;
using AOSharp.Core;
using System.Linq;

namespace AutomatonDB1
{
    public static class Extensions
    {
        public static bool CanProceed()
        {
            return DynelManager.LocalPlayer.HealthPercent > AutomatonDB1._settings["KitHealthPercentageBox"].AsInt32() 
                && DynelManager.LocalPlayer.NanoPercent > AutomatonDB1._settings["KitNanoPercentageBox"].AsInt32()
                && DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1
                && DynelManager.LocalPlayer.MovementState != MovementState.Sit
                && !Spell.HasPendingCast
                && !Debuffed();
        }

        public static bool Debuffed()
        {
            return DynelManager.LocalPlayer.Buffs.Contains(267283)
                || DynelManager.LocalPlayer.Buffs.Contains(280469)
                || DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Cocoon)
                || DynelManager.LocalPlayer.Buffs.Contains(280470)
                || DynelManager.LocalPlayer.Buffs.Contains(280488);
        }
    }
}
