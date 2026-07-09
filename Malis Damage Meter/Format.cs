using System;
using System.Collections.Generic;
using System.Linq;
using static MalisDamageMeter.MainWindow;

namespace MalisDamageMeter
{
    public static class Format
    {
        public class Colors
        {
            public const string Title = "b9ff00";
            public const string Accent = "b34c5e";
            public const string Bonus = "b36e4c";
            public const string Info = "498ab6";
            public const string Name = "eec911";
        }

        public static string Time(double seconds) => (TimeSpan.FromSeconds(seconds)).ToString(@"h\:mm\:ss\:f");

        public static string DumpDmgFormatBasic(List<DateTimeRange> elapsedTimeRange, float totalElapsedTime)
        {
            string core = "";
            int i = 0;

            var totalDmg = HitRegisters.Characters.Values.Select(x => x.DamageSources.Total).Sum();

            foreach (var charData in HitRegisters.Characters.Values.Where(x => x.DamageSources.Total > 0).OrderByDescending(x =>x.DamageSources.Total))
            {
                var startTime = charData.DamageStartTime;
                var endTime = charData.DamageEndTime;
                var participationTime = GetParticipationTime(startTime, endTime, elapsedTimeRange);
                core += BasicCoreDumpGeneral(totalElapsedTime, ++i, charData.Name, charData.Profession.ToString(), charData.DamageSources.Total, totalDmg, participationTime);
            }

            string header = BasicHeaderDumpGeneral("Damage", totalElapsedTime, totalDmg);

            string fullLog = $@"<a href=""text://" + $"{header + core}" + $"<font color='#{Colors.Info}'><br>~ Dump provided by Mali's Damage Meter.</font>" + $@""" >Basic Dump (Damage)</a>";

            return fullLog;
        }
        public static float GetParticipationTime(DateTime startTime, DateTime endTime, List<DateTimeRange> elapsedTimeRanges)
        {
            if (endTime <= startTime || elapsedTimeRanges == null || elapsedTimeRanges.Count == 0)
                return 0f;

            double totalSeconds = 0;
            var participationRange = new DateTimeRange { StartTime = startTime, EndTime = endTime };

            foreach (var range in elapsedTimeRanges)
            {
                var overlapStart = participationRange.StartTime > range.StartTime ? participationRange.StartTime : range.StartTime;
                var overlapEnd = participationRange.EndTime < range.EndTime ? participationRange.EndTime : range.EndTime;

                if (overlapEnd > overlapStart)
                {
                    totalSeconds += (overlapEnd - overlapStart).TotalSeconds;
                }
            }

            return (float)totalSeconds;
        }
        public static string DumpHealingFormatBasic(List<DateTimeRange> elapsedTimeRange, float totalElapsedTime)
        {
            string core = "";
            int i = 0;

            var totalDmg = HitRegisters.Characters.Values.Select(x => x.HealSource.Total).Sum();

            foreach (var charData in HitRegisters.Characters.Values.Where(x => x.HealSource.Total > 0).OrderByDescending(x => x.HealSource.Total))
            {
                var startTime = charData.DamageStartTime;
                var endTime = charData.DamageEndTime;
                var participationTime = GetParticipationTime(startTime, endTime, elapsedTimeRange);

                core += BasicCoreDumpGeneral(totalElapsedTime, ++i, charData.Name, charData.Profession.ToString(), charData.HealSource.Total, totalDmg, participationTime);
            }

            string header = BasicHeaderDumpGeneral("Healing", totalElapsedTime, totalDmg);

            string fullLog = $@"<a href=""text://" + $"{header + core}" + $"<font color='#{Colors.Info}'><br>~ Dump provided by Mali's Damage Meter.</font>" + $@""" >Basic Dump (Healing)</a>";

            return fullLog;
        }

        public static string BasicCoreDumpGeneral(double totalElapsedTime, int i, string charName, string charProfession, int totalCharSource, int totalSource, float totalParticipationTime)
        {
            return $"{i++}. <font color='#{Colors.Name}'>{charName}</font> " +
                    $"<font color='#{Colors.Accent}'>({charProfession})</font> " +
                    $"<font color='#{Colors.Info}'>Total:</font> " +
                    $"{TotalDmgFormat(totalCharSource)} <font color='#{Colors.Accent}'>|</font> " +
                    $"<font color='#{Colors.Info}'>Per Minute (Active):</font> " +
                    $"{DpmFormat(totalCharSource, totalParticipationTime)} <font color='#{Colors.Accent}'>|</font> " +
                    $"<font color='#{Colors.Info}'>Per Minute (Total):</font> " +
                    $"{DpmFormat(totalCharSource, totalElapsedTime)} <font color='#{Colors.Accent}'>|</font> " +
                    $"<font color='#{Colors.Info}'>Active Duration:</font> " +
                    $"{Time(totalParticipationTime)} <font color='#{Colors.Accent}'>|</font> " +
                    $"<font color='#{Colors.Info}'>Percent:</font> " +
                    $"{PercentFormat((float)totalCharSource / totalSource)}<br>";
        }

        public static string BasicHeaderDumpGeneral(string name, double elapsedTime, int totalSource)
        {
            return$"<font color='#{Colors.Title}'>Fight Duration:</font> {Time(elapsedTime)} (h:m:s:ds)<br>" +
                    $"<font color='#{Colors.Title}'>Total {name}:</font> {totalSource}<br>" +
                    $"<font color='#{Colors.Title}'>{name} Per Minute:</font> {DpmFormat(totalSource, elapsedTime)}<br><br>";
        }

        public static string DumpDmgFormatAdvanced(SimpleCharData charData, List<DateTimeRange> elapsedTime, float elapsedTimeSeconds)
        {
            var participationTime = GetParticipationTime(charData.DamageStartTime, charData.DamageEndTime, elapsedTime);

            string coreDump =
                $"{FormatHeader(charData, elapsedTimeSeconds, participationTime)}" +
                $"{FormatAllTotal(charData, elapsedTimeSeconds, participationTime)}" +
                $"{FormatWeaponInfo(charData)}" +
                $"{FormatAcDamage(charData)}" +
                $"{FormatSpecialDamage(charData)}" +
                $"{FormatOtherDamage(charData)}" +
                $"{FormatHealing(charData)}" +
                $"{FormatHitSourceInfo(charData)}" +
                $"{FormatHitReceivedInfo(charData)}" +
                $"<font color='#{Colors.Info}'>~ Dump provided by Mali's Damage Meter.</font>";

            string fullLog = $@"<a href=""text://" + $"{coreDump}" + $@""" >Advanced Dump ({charData.Name})</a>";

            return fullLog;
        }

        private static string FormatHealing(SimpleCharData charData)
        {
            string log = $"{FormatHitWithPetSingle("Total", charData.HealSource.Total, charData.HealSource.UserTotal, charData.HealSource.PetTotal)}";
            return $"<font color='#{Colors.Name}'>Healing:</font><br>{log}<br>";
        }

        public static string FormatAllTotal(SimpleCharData charData, float elapsedTimeSeconds, float participationTime)
        {
            string log =
                $"{FormatTotal(charData.DamageSources.Total, "Damage", elapsedTimeSeconds,participationTime)}" +
                $"{FormatTotal(charData.HealSource.Total, "Healing", elapsedTimeSeconds, participationTime)}" +
                $"{FormatTotal(charData.AbsorbSource.Total, "Absorbed", elapsedTimeSeconds, participationTime)}" +
                $"{FormatTotal(charData.HitSource.Total, "Total Hits Dealt", elapsedTimeSeconds, participationTime)}" +
                $"{FormatTotal(charData.HitReceived.Total, "Total Hits Received", elapsedTimeSeconds, participationTime)}" +
                $"{FormatTotalPercent((float)charData.HitSource.User.Crit / (charData.HitSource.UserTotal), "Player Crit Chance")}" +
                $"{FormatTotalPercent((float)charData.HitReceived.Miss / (charData.HitReceived.Total), "Player Evade Chance")}";

            return $"<font color='#{Colors.Name}'>Totals:</font><br>{log}<br>";

        }

        private static string FormatTotal(int totalAmount, string text, double totalElapsedTime, double participationTime)
        {
            return $" <font color='#{Colors.Info}'>{text}:</font> " +
                $"{TotalDmgFormat(totalAmount)}<font color='#{Colors.Accent}'> - </font>" +
                $"<font color='#{Colors.Info}'>Per Minute (Active):</font> " +
                $"{DpmFormat(totalAmount, participationTime)} <font color='#{Colors.Accent}'>|</font> " +
                $"<font color='#{Colors.Info}'>Per Minute (Total):</font> " +
                $"{DpmFormat(totalAmount, totalElapsedTime)}<br>";
        }

        private static string FormatTotalPercent(float totalAmount, string text)
        {
            return $" <font color='#{Colors.Info}'>{text}:</font> " +
                $"{PercentFormat(totalAmount)}<br>";
        }

        public static string FormatSpecialDamage(SimpleCharData charData)
        {
            string log = "";

            foreach (var damageAmount in Utils.SetSpecialsStats())
                log += $" <font color='#{Colors.Info}'>{damageAmount.Key}:</font> {charData.DamageSources.Weapon.User.Specials.DamagePerType[damageAmount.Key]}<br>";

            return $"<font color='#{Colors.Name}'>Specials Damage:</font><br>{log}<br>";
        }

        public static string FormatOtherDamage(SimpleCharData charData)
        {
            string log =
                FormatHitWithPetSingle("Reflect", charData.DamageSources.DeflectSource.Reflect.Total, charData.DamageSources.DeflectSource.Reflect.UserTotal, charData.DamageSources.DeflectSource.Reflect.PetTotal) +
                FormatHitWithPetSingle("Shield", charData.DamageSources.DeflectSource.Shield.Total, charData.DamageSources.DeflectSource.Shield.UserTotal, charData.DamageSources.DeflectSource.Shield.PetTotal);

            return $"<font color='#{Colors.Name}'>Other Damage:</font><br>{log}<br>";
        }

        public static string FormatAcDamage(SimpleCharData charData)
        {
            string log = "";

            foreach (var damageAmount in Utils.SetAcStats())
            {
                var UserAutoAttack= charData.DamageSources.Weapon.User.AutoAttack.DamagePerType[damageAmount.Key];
                var UserNanobots = charData.DamageSources.Nanobots.User.DamagePerType[damageAmount.Key];
                var PetAutoAttack = charData.DamageSources.Weapon.Pet.AutoAttack.DamagePerType[damageAmount.Key];
                var PetNanobots = charData.DamageSources.Nanobots.Pet.DamagePerType[damageAmount.Key];

                int totalAmount = UserAutoAttack + UserNanobots + PetAutoAttack + PetNanobots;

                log += 
                    $" <font color='#{Colors.Info}'>{damageAmount.Key}:</font> " +
                    $"{totalAmount}" +
                    $"{FormatDamageAcSingle("User",UserAutoAttack, UserNanobots)}"+
                    $"{FormatDamageAcSingle("Pet",PetAutoAttack, PetNanobots)}<br>";
            }

            return $"<br><font color='#{Colors.Name}'>AC Damage:</font><br>{log}<br>";
        }

        private static string FormatDamageAcSingle(string text, int autoAttackTotal, int nanobotsTotal)
        {
            return
                 $"<font color='#{Colors.Accent}'> - </font>" +
                 $"<font color='#{Colors.Info}'>{text} (</font>" +
                 $"<font color='#{Colors.Accent}'>W:</font> " +
                 $"{autoAttackTotal}" +
                 $"<font color='#{Colors.Info}'> , </font>" +
                 $"<font color='#{Colors.Accent}'>N:</font> " +
                 $"{nanobotsTotal}" +
                 $"<font color='#{Colors.Info}'>)</font>";
        }

        public static string FormatHealingInfo(SimpleCharData charData, double elapsedTime, ModeEnum currentMode)
        {
            string healingInfo = $"<font color='#{Colors.Info}'>Healing Per Minute:</font> " +
                $"{DpmFormat(charData.HealSource.Total, elapsedTime)}<br>" +
                $"<font color='#{Colors.Info}'>Total Percent:</font> " +
                $"{PercentFormat((float)charData.HealSource.Total / HitRegisters.Sum(currentMode))}<br>" +
                $"<font color='#{Colors.Info}'>Total Healing:</font> " +
                $"{TotalDmgFormat(charData.HealSource.Total)}<br>";

            return healingInfo;
        }

        public static string FormatAbsorbInfo(SimpleCharData charData, double elapsedTime, ModeEnum currentMode)
        {
            return $"<font color='#{Colors.Info}'>Absorb Per Minute:</font> " +
                $"{DpmFormat(charData.AbsorbSource.Total, elapsedTime)}<br>" +
                $"<font color='#{Colors.Info}'>Total Percent:</font> " +
                $"{PercentFormat((float)charData.AbsorbSource.Total / HitRegisters.Sum(currentMode))}<br>" +
                $"<font color='#{Colors.Info}'>Total Absorb:</font> " +
                $"{TotalDmgFormat(charData.AbsorbSource.Total)}<br>";
        }

        public static string FormatWeaponInfo(SimpleCharData charData)
        {
            string weaponInfo = $"<font color='#{Colors.Name}'>Weapons:</font><br>";

            foreach (var weaponIds in charData.WeaponIds)
                weaponInfo += $"<font color='#{Colors.Info}'> {weaponIds.Slot}:</font> " +
                $"<a href='itemref://{weaponIds.DummyItem.LowId}/{weaponIds.DummyItem.HighId}/{weaponIds.DummyItem.Ql}'>{weaponIds.DummyItem.Name}</a><br>";

            return weaponInfo;
        }

        public static string FormatHeader(SimpleCharData charData, double elapsedTime, double participationTime)
        {
            return
                $"<font color='#{Colors.Title}'>Name:</font> {charData.Name}</font><br>" +
                $"<font color='#{Colors.Title}'>Profession:</font> {charData.Profession}<br>" +
                $"<font color='#{Colors.Title}'>Active Duration:</font> {Time(participationTime)} (h:m:s:ds)<br>" +
                $"<font color='#{Colors.Title}'>Total Duration:</font> {Time(elapsedTime)} (h:m:s:ds)<br><br>";
        }

        public static string FormatHitReceivedInfo(SimpleCharData charData)
        {
            string hitInfo = $"<font color='#{Colors.Name}'>Hits Received:</font><br>";

            string hitTypeInfo =
                $"{FormatHit("Normal", charData.HitReceived.Normal)}<br>" +
                $"{FormatHit("Critical", charData.HitReceived.Crit)}<br>" +
                $"{FormatHit("Miss", charData.HitReceived.Miss)}<br>" +
                $"{FormatHit("Glancing", charData.HitReceived.Glancing)}<br><br>";

            return hitInfo + hitTypeInfo;
        }

        public static string FormatHitSourceInfo(SimpleCharData charData)
        {
            string hitInfo = $"<font color='#{Colors.Name}'>Hits Dealt:</font><br>";

            string hitTypeInfo =
                $"{FormatHitWithPetSingle("Normal", charData.HitSource.User.Normal + charData.HitSource.Pet.Normal, charData.HitSource.User.Normal, charData.HitSource.Pet.Normal)}" +
                $"{FormatHitWithPetSingle("Critical", charData.HitSource.User.Crit + charData.HitSource.Pet.Crit, charData.HitSource.User.Crit, charData.HitSource.Pet.Crit)}" +
                $"{FormatHitWithPetSingle("Miss", charData.HitSource.User.Miss + charData.HitSource.Pet.Miss, charData.HitSource.User.Miss, charData.HitSource.Pet.Miss)}" +
                $"{FormatHitWithPetSingle("Glancing", charData.HitSource.User.Glancing + charData.HitSource.Pet.Glancing, charData.HitSource.User.Glancing, charData.HitSource.Pet.Glancing)}<br>";

            return hitInfo + hitTypeInfo;
        }

        private static string FormatHitWithPetSingle(string text, int combinedTotal, int charTotal, int petTotal)
        {
            return
                $"<font color='#{Colors.Info}'> {text}:</font> " +
                $"{combinedTotal}" +
                $"<font color='#{Colors.Accent}'> - </font>" +
                $"<font color='#{Colors.Info}'>User:</font> " +
                $"{charTotal}" +
                $"<font color='#{Colors.Accent}'> - </font>" +
                $"<font color='#{Colors.Info}'>Pet:</font> " +
                $"{petTotal}<br>";
        }

        private static string FormatHit(string text, int combinedTotal)
        {
            return
                $"<font color='#{Colors.Info}'> {text}:</font> " +
                $"{combinedTotal}";
        }

        public static string TotalDmgFormat(int totalDamage)
        {
            if (totalDamage < 100000)
                return totalDamage.ToString();
            else if (totalDamage < 1000000)
                return string.Format("{0:0.00}", (double)totalDamage / 1000) + "K";
            else
                return string.Format("{0:0.00}", (double)totalDamage / 1000000) + "M";
        }

        public static string PercentFormat(float number)
        {
            return !float.IsNaN(number) ? string.Format("{0:0.0}", number * 100) + "%" : "0.0%";
        }

        public static string DpmFormat(int totalDamage, double elapsedTime)
        {
            var dpm = (float)totalDamage * 60 / elapsedTime;

            if (dpm < 1000)
                return string.Format("{0:0.0}", dpm);
            else
                return string.Format("{0:0.00}", dpm / 1000) + "K";
        }
    }
}