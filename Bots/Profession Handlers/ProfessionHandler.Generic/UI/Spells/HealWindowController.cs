using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class HealUiConfig : IUiConfig, IOptionsConfig
{
    public int[] HealIds;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => HealIds ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
    public List<(string Name, int Value)> Options { get; set; }
}

public class HealWindowController
{
    private GenericWindowController<HealUiConfig> _HealWindow;
    public Window CurrentWindow => _HealWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string, int)> HealTargetOptions = new List<(string, int)> { ("Off", 0), ("Self (HP%)", 1), ("Team/Raid (HP%)", 2), ("All Players", 3) };
    public static readonly List<(string, int)> AOEHealOptions = new List<(string, int)> { ("Off", 0), ("On", 1) };

    public static List<HealUiConfig> HealthNano = new List<HealUiConfig>
    {
        new HealUiConfig { HealIds = RelevantGenericItems.SitKits, SettingKey = "KitsOption", Label = "Use Kits", UiType = UiType.Checkbox },
        new HealUiConfig { HealIds = RelevantGenericItems.SitKits, SettingKey = "HealthKitsValue", Label = "Health Kits %", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = RelevantGenericItems.SitKits, SettingKey = "NanoKitsValue", Label = "Nano Kits %", UiType = UiType.TextInput },

        new HealUiConfig { HealIds = RelevantGenericItems.HealthAndNanoStims, SettingKey = "StimsOption", Label = "Use Stims", UiType = UiType.Checkbox },
        new HealUiConfig { HealIds = RelevantGenericItems.HealthAndNanoStims, SettingKey = "HealthStimsValue", Label = "Health Stims %", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = RelevantGenericItems.HealthAndNanoStims, SettingKey = "NanoStimsValue", Label = "Nano Stims %", UiType = UiType.TextInput },
    };

    public static List<HealUiConfig> Heals = new List<HealUiConfig>
    {
        new HealUiConfig { HealIds = Spell.GetSpellsForNanoline(NanoLine.HealPets).Select(s=>s.Id).ToArray(), SettingKey ="PetHealing", Label ="Heal Pet Options", UiType = UiType.DropDownInputBox, Options = new List<(string Name, int Value)>{ ("None", 0), ("Self (HP%)", 1), ("Team/Raid (HP%)", 2), ("Lead (HP%)", 3), ("All Players (HP%)", 4)} },
        new HealUiConfig { HealIds = new[] { SpellID.FountainOfLife }, SettingKey = "FountainOfLifeHeal", Label = "Fountain of Life", UiType = UiType.DropDownInputBoxDropDown },
        
        new HealUiConfig { HealIds = new[] { RelevantGenericItems.DeathsDoor }, SettingKey = "DeathsDoorOption", Label = "Death's Door", UiType = UiType.Checkbox },
        new HealUiConfig { HealIds = new[] { RelevantGenericItems.DeathsDoor }, SettingKey = "DeathsDoorValue", Label = "Death's Door %", UiType = UiType.TextInput },

        new HealUiConfig { HealIds = new[] { (int)PerkHash.BalanceOfYinAndYang }, SettingKey = "BalanceOfYinAndYangValue", Label = "Balance Of Yin And Yang", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.ConsumeTheSoul }, SettingKey = "ConsumeTheSoulValue", Label = "Consume The Soul", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.CuringTouch }, SettingKey = "CuringTouchValue", Label = "Curing Touch", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Devour }, SettingKey = "DevourValue", Label = "Devour", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.DrawBlood }, SettingKey = "DrawBloodValue", Label = "Draw Blood", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.EnhancedHeal }, SettingKey = "EnhancedHealValue", Label = "Enhanced Heal", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Exultation }, SettingKey = "ExultationValue", Label = "Exultation", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.FieldBandage }, SettingKey = "FieldBandageValue", Label = "Field Bandage", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.HarmonizeBodyAndMind }, SettingKey = "HarmonizeBodyAndMindValue", Label = "Harmonize Body And Mind", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.LayOnHands }, SettingKey = "LayOnHandsValue", Label = "Lay On Hands", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.ReapLife }, SettingKey = "ReapLifeValue", Label = "Reap Life", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.ReconstructDNA }, SettingKey = "ReconstructDNAValue", Label = "Reconstruct DNA", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.RedDawn }, SettingKey = "RedDawnValue", Label = "Red Dawn", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.TapVitae }, SettingKey = "TapVitaeValue", Label = "Tap Vitae", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.VitalShock }, SettingKey = "VitalShockValue", Label = "Vital Shock", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Diffuse }, SettingKey = "DiffuseValue", Label = "Diffuse HP %", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Heal }, SettingKey = "HealValue", Label = "Heal HP %", UiType = UiType.TextInput },

        new HealUiConfig { HealIds = RelevantGenericItems.TOTW_Doc_Books, SettingKey = "TotwDocBooksValue", Label = "ToTW Doctor Book % (Locks Bio Met)", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = RelevantGenericItems.SanguisugentBodyArmor, SettingKey = "SanguisugentBodyArmorValue", Label = "Sanguisugent Body Armor", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = RelevantGenericItems.PerniciousBodyArmor, SettingKey = "PerniciousBodyArmorValue", Label = "Pernicious Body Armor", UiType = UiType.TextInput },
    };

    public static List<HealUiConfig> PetHeal = new List<HealUiConfig>
    {
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Reconstruction }, SettingKey = "ReconstructionValue", Label = "Reconstruction", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.RepairPet }, SettingKey = "RepairPetValue", Label = "Repair Pet", UiType = UiType.TextInput },
    };

    public static List<HealUiConfig> HOT = new List<HealUiConfig>
    {
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BioRegrowth }, SettingKey = "BioRegrowthValue", Label = "Bio Regrowth", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Lifeblood }, SettingKey = "LifebloodValue", Label = "Lifeblood", UiType = UiType.TextInput },
    };

    public static List<HealUiConfig> Nano = new List<HealUiConfig>
    {
        new HealUiConfig { HealIds = RelevantGenericItems.TOTWWrists, SettingKey = "TOTWWristsValue", Label = "ToTW Wrists", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = RelevantGenericItems.PremiumNanoRecharger, SettingKey = "PremiumNanoRechargerValue", Label = "Premium Nano Recharger % (Locks Nano Pro)", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[]{ RelevantGenericItems.ViralCommunicationsLarvae}, SettingKey = "ViralCommunicationsLarvaeValue", Label = "Viral Communications Larvae %", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[]{ RelevantGenericItems.NotumFocus}, SettingKey = "NotumFocusValue", Label = "Notum Focus Nano%", UiType = UiType.TextInput },

        new HealUiConfig { HealIds = new[] { (int)PerkHash.NanoHeal }, SettingKey = "NanoHealValue", Label = "Nano Heal", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.RegainNano }, SettingKey = "RegainNanoValue", Label = "Regain Nano", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.SpiritOfPurity }, SettingKey = "SpiritOfPurityValue", Label = "Spirit Of Purity", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.TapNotumSource }, SettingKey = "TapNotumSourceValue", Label = "Tap Notum Source", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.AccessNotumSource }, SettingKey = "AccessNotumSourceValue", Label = "Access Notum Source", UiType = UiType.TextInput },
    };

    public static List<HealUiConfig> AOEHeal = new List<HealUiConfig>
    {
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Awakening }, SettingKey = "AwakeningValue", Label = "Awakening", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BattlegroupHeal1 }, SettingKey = "BattlegroupHeal1Value", Label = "Battlegroup Heal 1", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BattlegroupHeal2 }, SettingKey = "BattlegroupHeal2Value", Label = "Battlegroup Heal 2", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BattlegroupHeal3 }, SettingKey = "BattlegroupHeal3Value", Label = "Battlegroup Heal 3", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BattlegroupHeal4 }, SettingKey = "BattlegroupHeal4Value", Label = "Battlegroup Heal 4", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BioRejuvenation }, SettingKey = "BioRejuvenationValue", Label = "Bio Rejuvenation", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.BlessingOfLife }, SettingKey = "BlessingOfLifeValue", Label = "Blessing Of Life", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.SpiritOfBlessing }, SettingKey = "SpiritOfBlessingValue", Label = "Spirit Of Blessing", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.TeamHeal }, SettingKey = "TeamHealValue", Label = "Team Heal", UiType = UiType.TextInput },
        new HealUiConfig { HealIds = new[] { (int)PerkHash.PurpleHeart }, SettingKey = "PurpleHeartValue", Label = "Purple Heart", UiType = UiType.TextInput },

    };

    public static List<HealUiConfig> AOENano = new List<HealUiConfig>
    {

    };

    public static List<HealUiConfig> AOEHealthAndNano = new List<HealUiConfig>()
    {
        new HealUiConfig { HealIds = new[] { (int)PerkHash.Survival }, SettingKey = "SurvivalValue", Label = "Survival", UiType = UiType.TextInput },
    };

    #endregion

    public HealWindowController(HandlerSettings settings)
    {
        _HealWindow = new GenericWindowController<HealUiConfig>(settings);
    }

    public void HealShowWindow(string pluginDirectory)
    {
        try
        {
            _HealWindow.ShowWindow(
                title: "Heals",
                xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<HealUiConfig>)>
                {
                    ("Health and Nano", HealthNano),
                    ("Target Heals", Heals),
                    ("Pet Heals", PetHeal),
                    ("Hots", HOT),
                    ("Nano heals", Nano),
                    ("AOE Heals", AOEHeal),
                    ("AOE Nano", AOENano),
                    ("AOE Heath and Nano", AOEHealthAndNano),
                },
                extraSection: (null),
                sectionIds: LoadedHeals,
                entryIds: LoadedHeals,
                pluginDirectory: pluginDirectory,
                options: HealTargetOptions);
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
