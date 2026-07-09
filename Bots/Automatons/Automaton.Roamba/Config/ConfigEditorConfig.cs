
using Newtonsoft.Json;
using System;
using System.IO;

namespace AutomatonRoamba
{
    public class ConfigEditorConfig
    {
        public bool AttackMobs { get; set; }

        public AttackRangeMode AttackRangeMode { get; set; }

        public int ManualModeAttackRange { get; set; }

        public int AutomaticModeAttackPadding { get; set; }

        public bool UseTauntItem { get; set; }

        public bool PathToCorpses { get; set; }

        public bool PathToMobs { get; set; }

        public bool AttackPriorityOnly { get; set; }

        public int TauntRange { get; set; }

        public int PathRange { get; set; }

        public int WanderLimit { get; set; }

        public int FightTimeoutPeriod { get; set; }

        public int LootTimeoutPeriod { get; set; }

        public bool FollowTarget { get; set; }

        public string FollowTargetName { get; set; }

        public bool DisableIf { get; set; }

        public bool DisableIfHp { get; set; }

        public bool DisableIfNp { get; set; }

        public bool DisableIfAttacked { get; set; }

        public bool DisableIfPlayersNearby { get; set; }

        public int HealthPercent { get; set; }

        public int NanoPercent { get; set; }
       
        public bool IgnoreLos { get; set; }

        public ConfigEditorConfig()
        {
            AttackMobs = true;
            FightTimeoutPeriod = 60;
            AttackRangeMode = AttackRangeMode.Automatic;
            AutomaticModeAttackPadding = 1;
            ManualModeAttackRange = 10;

            UseTauntItem = true;
            TauntRange = 40;

            PathToMobs = true;
            PathRange = 30;

            PathToCorpses = false;
            LootTimeoutPeriod = 5;

            FollowTarget = false;
            FollowTargetName = "None1\nNone2\nNone3";

            DisableIf = false;
            DisableIfHp = false;
            DisableIfNp = false;
            HealthPercent = 66;
            NanoPercent = 66;
            DisableIfAttacked = false;
            DisableIfPlayersNearby = false;

            WanderLimit = 70;
            AttackPriorityOnly = false;

            IgnoreLos = false;
        }

        public static ConfigEditorConfig Load(string path)
        {
            ConfigEditorConfig config;

            try
            {
                config = JsonConvert.DeserializeObject<ConfigEditorConfig>(File.ReadAllText(path));
            }
            catch
            {
                config = new ConfigEditorConfig();
            }


            return config;
        }

        public void Save(string savePath)
        {
            try
            {
                File.WriteAllText(savePath, JsonConvert.SerializeObject(this, Formatting.Indented));
                AutomatonRoamba.Log.Information($"Config Editor Config file saved");
            }
            catch (Exception ex)
            {
                AutomatonRoamba.Log.Warning(ex.Message);
            }
        }
    }
}