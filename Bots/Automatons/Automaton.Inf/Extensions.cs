using System;
using System.Linq;
using System.Security.Cryptography;
using AOSharp.Common.GameData;
using AOSharp.Core;

namespace AutomatonInf
{
    public static class Extensions
    {
        public static void AddRandomness(this ref Vector3 pos, int entropy)
        {
            pos.X += Next(-entropy, entropy);
            pos.Z += Next(-entropy, entropy);
        }

        public static int Next(int min, int max)
        {
            if (min >= max)
                throw new ArgumentException("Min value is greater or equals than Max value.");

            byte[] intBytes = new byte[4];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                rng.GetNonZeroBytes(intBytes);

            return min + Math.Abs(BitConverter.ToInt32(intBytes, 0)) % (max - min + 1);
        }

        public static bool CanProceed()
        {
            return DynelManager.LocalPlayer.HealthPercent > 65
                && DynelManager.LocalPlayer.NanoPercent > 65
                && DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) <= 1
                && DynelManager.LocalPlayer.MovementState != MovementState.Sit
                && !Spell.HasPendingCast && Spell.List.Any(spell => spell.IsReady);
        }

        public static bool InAttackRange(SimpleChar target)
        {
            var lp = DynelManager.LocalPlayer;
            if (lp == null || target == null) return false;

            float range = lp.GetStat(Stat.AttackRange) == 0
                ? 8f
                : lp.GetStat(Stat.AttackRange);

            var weapons = lp.Weapons?.Values;
            if (weapons != null && weapons.Any())
                range += weapons.Min(v => v.AttackRange);

            return lp.Position.DistanceFrom(target.Position) <= (range - 2f);
        }
    }
}
