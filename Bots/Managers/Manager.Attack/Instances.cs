using AOSharp.Core;
using System.Collections.Generic;
using System.Linq;

namespace ManagerAttack
{
    public partial class ManagerAttack
    {
        public static List<SimpleChar> _switchMob = new List<SimpleChar>();
        public static List<SimpleChar> _mob = new List<SimpleChar>();
        public static List<SimpleChar> _bossMob = new List<SimpleChar>();

        public static void Scanning()
        {
            switch (Playfield.ModelIdentity.Instance)
            {
                case 127:
                    ScanningInstance127();
                    break;
                case 6123:
                    ScanningInstance6123();
                    break;

                case 1934:
                    ScanningInstance1934();
                    break;

                case 6015:
                    ScanningInstance6015();
                    break;

                case 8020:
                case 8050:
                    ScanningInstance8020();
                    break;

                case 9070:
                    ScanningInstance9070();
                    break;

                case 9061:
                    ScanningInstance9061();
                    break;

                case 4329:
                case 4330:
                case 4331:
                case 4389:
                case 4328:
                case 4390:
                case 4391:
                    ScanningInstance4389();
                    break;

                case 1931:
                    ScanningInstance1931();
                    break;

                case 1941:
                    ScanningInstance1941();
                    break;
                case 4365:
                    ScanningInstance4365();
                    break;
                case 4366:
                    ScanningInstance4366();
                    break;
                case 4367:
                    ScanningInstance4367();
                    break;
                default:
                    ScanningDefault();
                    break;
            }
        }

        private static void ScanningInstance6123() //s10?
        {
            if (!_config.Instances.TryGetValue("6123", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs
                .Where(c => !c.IsPlayer
                    && !_config.GlobalIgnores.Contains(c.Name)
                    && c.Name != "Zix"
                    && !c.Name.Contains("sapling")
                    && c.Health > 0
                    && c.IsInLineOfSight
                    && c.MaxHealth < 1000000
                    && (!c.IsPet || c.Name == "Drop Trooper - Ilari'Ra"))
                .ToList();

            _bossMob = DynelManager.NPCs.Where(c => bossOrder.Contains(c.Name) && DynelManager.NPCs.Where(t => t.Name == "Kyr'Ozch Technician" && c.Buffs.Contains(new[] { 287525, 287515, 287526 })).Any()).ToList();
        }

        private static void ScanningInstance6015() //12m
        {
            if (!_config.Instances.TryGetValue("6015", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
           
            List<string> bossOrder;

            switch (_settings["12M"].AsInt32())
            {
                case 0:
                    bossOrder = new List<string> { "Right Hand of Madness", "Left Hand of Insanity", "Deranged Xan" };
                    break;
                case 1:
                    bossOrder = new List<string> { "Left Hand of Insanity", "Right Hand of Madness", "Deranged Xan" };
                    break;
                case 2:
                    bossOrder = new List<string> { "Right Hand of Madness", "Deranged Xan", "Left Hand of Insanity" };
                    break;
                default:
                    bossOrder = new List<string> { "Right Hand of Madness", "Left Hand of Insanity", "Deranged Xan" };
                    break;
            }

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _bossMob = DynelManager.NPCs.Where(c => bossOrder.Any(b => c.Name.Contains(b)) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && !c.Buffs.Contains(new[] { 253953, 205607 })).OrderBy(c => bossOrder.FindIndex(p => c.Name.Contains(p))).ToList();
        }

        private static void ScanningInstance127() // Subway
        {
            if (!_config.Instances.TryGetValue("127", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && !c.Buffs.Contains(new[] { 253953, 205607 })).ToList();

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => !c.IsPlayer && !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && c.MaxHealth < 1000000).ToList();
        }

        private static void ScanningInstance9070() //Subway Raid
        {
            if (!_config.Instances.TryGetValue("9070", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && !c.Buffs.Contains(new[] { 253953, 205607 }) && c.MaxHealth >= 1000000).ToList();

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => !c.IsPlayer && !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && c.MaxHealth < 1000000).ToList();
        }

        private static void ScanningInstance9061()//TOTW Raid
        {
            if (!_config.Instances.TryGetValue("9061", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && !c.Buffs.Contains(new[] { 253953, 205607 }) && c.MaxHealth >= 1000000).OrderBy(c => bossOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => !c.IsPlayer && !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && c.MaxHealth < 1000000 && !c.IsPet).ToList();
        }

        private static void ScanningInstance1934() //Inner Sanctum
        {
            _bossMob = DynelManager.NPCs
               .Where(c => !_config.GlobalIgnores.Contains(c.Name)
                   && c.Health > 0 && c.IsInLineOfSight && !c.IsPet
                   && !c.Buffs.Contains(253953) && !c.Buffs.Contains(205607))
               .OrderByDescending(c => c.Name == "Hezak the Immortal")
               .OrderByDescending(c => c.Name == "Inobak the Gelid")
               .OrderByDescending(c => c.Name == "Dominus Jiannu")
               .OrderByDescending(c => c.Name == "Dominus Facut the Bloodless")
               .OrderByDescending(c => c.Name == "Dominus Ummoh the Pedagogue")
               .OrderByDescending(c => c.Name == "Jeuru the Defiler")
               .OrderByDescending(c => c.Name == "Iskop the Idolator")
               .ToList();

            _switchMob = DynelManager.NPCs
               .Where(c => !_config.GlobalIgnores.Contains(c.Name)
                               && c.Health > 0 && c.IsInLineOfSight && !c.IsPet
                               && (c.Name == "Devoted Fanatic"
                               || c.Name == "Hallowed Acolyte"
                               || c.Name == "Fanatic"
                               || c.Name == "Turbulent Windcaller"
                               || c.Name == "Ruinous Reverend"
                               || c.Name == "Eternal Guardian"
                               || c.Name == "Defensive Drone"
                               || c.Name == "Confounding Bloated Carcass"))
                           .OrderByDescending(c => c.Name == "Hallowed Acolyte")
                           .OrderByDescending(c => c.Name == "Confounding Bloated Carcass")
                           .OrderByDescending(c => c.Name == "Devoted Fanatic")
                           .ToList();

            _mob = DynelManager.NPCs
                .Where(c => !c.IsPlayer
                                && !_config.GlobalIgnores.Contains(c.Name) && c.Health > 0
                                && c.IsInLineOfSight && !c.IsPet)
                            .OrderByDescending(c => c.Name == "Faithful Cultist")
                            .OrderByDescending(c => c.Name == "Ruinous Reverend")
                            .OrderByDescending(c => c.Name == "Hallowed Acolyte")
                            .OrderByDescending(c => c.Name == "Turbulent Windcaller")
                            .OrderByDescending(c => c.Name == "Seraphic Exarch")
                            .OrderByDescending(c => c.Name == "Cultist Silencer")
                            .OrderByDescending(c => c.Name == "Devoted Fanatic")
                            .ToList();
        }

        private static void ScanningInstance1931() //TOTW
        {
            if (!_config.Instances.TryGetValue("1931", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.Characters.Where(c => bossOrder.Contains(c.Name) && c.Health > 0 && !c.IsPet && c.IsInLineOfSight).OrderBy(c => bossOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => c.Health > 0 && !_config.GlobalIgnores.Contains(c.Name) && !ignores.Contains(c.Name) && c.IsInLineOfSight && !c.IsPet).ToList();
        }

        private static void ScanningInstance4389()//IPande/Pande
        {
            if (!_config.Instances.TryGetValue("4389", out var entry))
                return;

            var ignores = entry.Ignores;

            List<string> switchOrder;

            switch (_settings["Pandemonium"].AsInt32())
            {
                case 0: // Pink, Blue, Spider, Hiis
                    switchOrder = new List<string> { "Corrupted Xan-Len", "Corrupted Xan-Cur", "Corrupted Xan-Kuir", "Corrupted Hiisi Berserker" };
                    break;
                case 1: // Pink, Spider, Blue, Hiis
                    switchOrder = new List<string> { "Corrupted Xan-Len", "Corrupted Xan-Kuir", "Corrupted Xan-Cur", "Corrupted Hiisi Berserker" };
                    break;
                case 2: // Pink, Blue, Hiis, Spider
                    switchOrder = new List<string> { "Corrupted Xan-Len", "Corrupted Xan-Cur", "Corrupted Hiisi Berserker", "Corrupted Xan-Kuir" };
                    break;
                case 3: // Hiis, Blue, Spider, Pink
                    switchOrder = new List<string> { "Corrupted Hiisi Berserker", "Corrupted Xan-Cur", "Corrupted Xan-Kuir", "Corrupted Xan-Len", };
                    break;
                default:
                    switchOrder = new List<string> { "Corrupted Xan-Len", "Corrupted Xan-Cur", "Corrupted Xan-Kuir", "Corrupted Hiisi Berserker" };
                    break;
            }

            _bossMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0  && c.IsInLineOfSight && !c.IsPet && !c.Buffs.Contains(new[] { 253953, 205607 }) && c.MaxHealth >= 450000).ToList();

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => c.Health > 0 && !c.IsPet && !ignores.Contains(c.Name) && c.IsInLineOfSight).ToList();
        }
        private static void ScanningInstance8020() //POH
        {
            if (!_config.Instances.TryGetValue("8020", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => c.Health > 0 && bossOrder.Contains(c.Name) && c.IsInLineOfSight && !c.IsPet).ToList();

            _switchMob = DynelManager.NPCs.Where(c => switchOrder.Any(p => c.Name.Contains(p)) && c.Health > 0 && c.IsInLineOfSight).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => c.Health > 0 && !ignores.Contains(c.Name) && !bossOrder.Contains(c.Name) && !c.IsPet && c.IsInLineOfSight).ToList();
        }

        private static void ScanningInstance1941() //Formans
        {
            if (!_config.Instances.TryGetValue("1941", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && !c.Buffs.Contains(new[] { 253953, 205607 }) && c.Name == "Lab Director").ToList();

            _switchMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => !c.IsPlayer && !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet).ToList();
        }

        private static void ScanningInstance4365() //Sector 13
        {
            if (!_config.Instances.TryGetValue("4365", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => c.Health > 0 && !ignores.Contains(c.Name) && c.IsInLineOfSight && !c.IsPet && c.MaxHealth >= 450000).ToList();

            _switchMob = DynelManager.NPCs.Where(c => (switchOrder.Contains(c.Name) || c.Buffs.Contains(new[] { 256509, 252409, 252411 })) && c.Health > 0 && c.IsInLineOfSight && !ignores.Contains(c.Name)).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => !c.Name.Contains("Unicorn") && !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet).ToList();
        }

        private static void ScanningInstance4366() //Sector 28
        {
            if (!_config.Instances.TryGetValue("4366", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            _bossMob = DynelManager.NPCs.Where(c => bossOrder.Contains(c.Name) && !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && c.MaxHealth >= 4000000).ToList();

            _switchMob = DynelManager.NPCs.Where(c => (switchOrder.Contains(c.Name) || c.FightingTarget?.Name == "Rookie Alien Hunter") && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && !ignores.Contains(c.Name)).OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p))).ToList();

            _mob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && !bossOrder.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet).ToList();
        }
        private static void ScanningInstance4367() //Sector 35
        {
            if (!_config.Instances.TryGetValue("4367", out var entry))
                return;

            var ignores = entry.Ignores;
            var switchOrder = entry.Switch;
            var bossOrder = entry.Boss;

            var assist = new List<string>() { "Unicorn Service Tower Alpha", "Unicorn Service Tower Delta", "Unicorn Service Tower Gamma" };

            _bossMob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet && c.MaxHealth >= 450000 && !c.Buffs.Contains(256507)).OrderBy(c => c.Name == "Field Support  - Cha'Khaz").ToList();

            _switchMob = DynelManager.NPCs.Where(c => (c.Name.Contains("Support") || assist.Contains(c.FightingTarget?.Name)) && c.MaxHealth < 450000 && c.Health > 0 && c.IsInLineOfSight && !ignores.Contains(c.Name)).ToList();

            _mob = DynelManager.NPCs.Where(c => !ignores.Contains(c.Name) && c.Health > 0 && c.IsInLineOfSight && !c.IsPet).ToList();
        }

        private static void ScanningDefault()
        {
            var instanceId = Playfield.ModelIdentity.Instance.ToString();
            if (_config.Instances.TryGetValue(instanceId, out var entry))
            {
                var ignores = entry.Ignores;
                var switchOrder = entry.Switch;
                var bossOrder = entry.Boss;

                _bossMob = DynelManager.NPCs
                           .Where(c => bossOrder.Contains(c.Name) && !ignores.Contains(c.Name)
                               && c.Health > 0 && c.IsInLineOfSight && !c.IsPet
                               && !c.Buffs.Contains(253953) && !c.Buffs.Contains(205607)
                               && c.MaxHealth >= 1000000)
                           .ToList();

                _switchMob = DynelManager.NPCs
                    .Where(c => !ignores.Contains(c.Name)
                        && c.Health > 0 && c.IsInLineOfSight && c.MaxHealth < 1000000 && !c.IsPet
                        && switchOrder.Any(p => c.Name.Contains(p)))
                    .OrderBy(c => switchOrder.FindIndex(p => c.Name.Contains(p)))
                    .ToList();

                _mob = DynelManager.NPCs
                    .Where(c => !c.IsPlayer
                        && !ignores.Contains(c.Name)
                        && c.Health > 0
                        && c.IsInLineOfSight
                        && c.MaxHealth < 1000000
                        && !c.IsPet)
                    .ToList();
            }
            else
            {
                _bossMob = DynelManager.NPCs
                           .Where(c => !_config.GlobalIgnores.Contains(c.Name)
                               && c.Health > 0 && c.IsInLineOfSight && !c.IsPet
                               && !c.Buffs.Contains(253953) && !c.Buffs.Contains(205607)
                               && c.MaxHealth >= 1000000).ToList();

                _switchMob = DynelManager.NPCs
                   .Where(c => !_config.GlobalIgnores.Contains(c.Name)
                       && c.Name != "Zix" && !c.Name.Contains("sapling")
                       && c.Health > 0 && c.IsInLineOfSight && c.MaxHealth < 1000000 && !c.IsPet
                       && (c.Name == "Hand of the Colonel"
                       || c.Name == "Security Supervisor - Ankari'Sinuh"
                       || c.Name == "Hacker'Uri"
                       || c.Name == "Drone Harvester - Jaax'Sinuh"
                       || c.Name == "Support Sentry - Ilari'Uri"
                       || c.Name == "Fanatic"
                       || c.Name == "Alien Coccoon"
                       || c.Name == "Alien Cocoon"
                       || c.Name == "Stasis Containment Field"))
                   .OrderByDescending(c => c.Name == "Drone Harvester - Jaax'Sinuh")
                   .OrderByDescending(c => c.Name == "Support Sentry - Ilari'Uri")
                   .OrderByDescending(c => c.Name == "Alien Cocoon")
                   .OrderByDescending(c => c.Name == "Alien Coccoon" && c.MaxHealth < 40001)
                   .ToList();

                _mob = DynelManager.NPCs
                    .Where(c => !c.IsPlayer
                        && !_config.GlobalIgnores.Contains(c.Name)
                        && c.Name != "Zix" && !c.Name.Contains("sapling") && c.Health > 0
                        && c.IsInLineOfSight && c.MaxHealth < 1000000
                        && (!c.IsPet || c.Name == "Drop Trooper - Ilari'Ra"))
                    .OrderByDescending(c => c.Name == "Drone Harvester - Jaax'Sinuh")
                    .OrderByDescending(c => c.Name == "Support Sentry - Ilari'Uri")
                    .OrderByDescending(c => c.Name == "Alien Cocoon")
                    .OrderByDescending(c => c.Name == "Alien Coccoon" && c.MaxHealth < 40001)
                    .OrderByDescending(c => c.Name == "Masked Operator")
                    .OrderByDescending(c => c.Name == "Masked Technician")
                    .OrderByDescending(c => c.Name == "Masked Engineer")
                    .OrderByDescending(c => c.Name == "Masked Superior Commando")
                    .OrderByDescending(c => c.Name == "Hacker'Uri")
                    .OrderByDescending(c => c.Name == "Hand of the Colonel")
                    .ToList();
            }
        }

    }
}
