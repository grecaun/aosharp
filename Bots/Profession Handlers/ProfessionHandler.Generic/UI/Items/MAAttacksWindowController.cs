using System;
using System.Collections.Generic;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class MAAttackUiConfig : IUiConfig
{
    public int[] ItemIds;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => ItemIds ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
}

public class MAAttacksWindowController
{
    private GenericWindowController<MAAttackUiConfig> _MAAttackWindow;
    public Window CurrentWindow => _MAAttackWindow.CurrentWindow;

    #region Lists

    public static List<MAAttackUiConfig> MartialArts = new List<MAAttackUiConfig>
    {
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.Sappo }, SettingKey = "SappoOption", Label = "Sappo", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.StingoftheViper }, SettingKey = "StingoftheViperOption", Label = "Sting of the Viper", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.ApeFistofKhalum }, SettingKey = "ApeFistofKhalumOption", Label = "Ape Fist of Khalum", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.KarmicFist }, SettingKey = "KarmicFistOption", Label = "Karmic Fist", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.Shen, SettingKey = "ShenOption", Label = "Shen", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.FlowerOfLife, SettingKey = "FlowerOfLifeOption", Label = "Flower of Life", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.FlowerOfLife, SettingKey = "FlowerOfLifesValue", Label = "Flower of Life %", UiType = UiType.TextInput },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.BrightBlueCloudlessSky, SettingKey = "BrightBlueCloudlessSkyOption", Label = "Bright, Blue, Cloudless Sky", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.BlessedWithThunder, SettingKey = "BlessedWithThunderOption", Label = "Blessed with Thunder", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.BirdOfPrey, SettingKey = "BirdOfPreyOption", Label = "Bird of Prey", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.AttackOfTheSnake, SettingKey = "AttackOfTheSnakeOption", Label = "Attack of the Snake", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.AngelOfNight, SettingKey = "AngelOfNightOption", Label = "Angel of Night", UiType = UiType.Checkbox },
    };

    public static List<MAAttackUiConfig> Dimach = new List<MAAttackUiConfig>
    {
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.TouchOfSaiFung }, SettingKey = "TouchOfSaiFungOption", Label = "Touch Of Sai Fung", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.TheWizdomOfHuzzum}, SettingKey = "TheWizdomOfHuzzumOption", Label = "The Wizdom of Huzzum", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.TreeOfEnlightenment }, SettingKey = "TreeOfEnlightenmentOption", Label = "Tree of Enlightenment", UiType = UiType.Checkbox },
    };

    public static List<MAAttackUiConfig> Riposte = new List<MAAttackUiConfig>
    {
        new MAAttackUiConfig { ItemIds = RelevantGenericItems.UponAWaveOfSummer, SettingKey = "UponAWaveOfSummerOption", Label = "Upon a Wave of Summer", UiType = UiType.Checkbox },
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.StampedeOfTheBoar }, SettingKey = "StampedeOfTheBoarOption", Label = "Stampede of the Boar", UiType = UiType.Checkbox },
    };

    public static List<MAAttackUiConfig> Strength = new List<MAAttackUiConfig>
    {
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.Delirium }, SettingKey = "DeliriumOption", Label = "Delirium", UiType = UiType.Checkbox },
    };

    public static List<MAAttackUiConfig> Stamina = new List<MAAttackUiConfig>
    {
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.InnerBalance }, SettingKey = "InnerBalanceOption", Label = "Inner Balance", UiType = UiType.Checkbox },
    };

    public static List<MAAttackUiConfig> Intelligence = new List<MAAttackUiConfig>
    {
        new MAAttackUiConfig { ItemIds = new[] { RelevantGenericItems.Enigma }, SettingKey = "EnigmaOption", Label = "Enigma", UiType = UiType.Checkbox },
    };

    #endregion

    public MAAttacksWindowController(HandlerSettings settings)
    {
        _MAAttackWindow = new GenericWindowController<MAAttackUiConfig>(settings);
    }

    public void MAAttackShowWindow(string pluginDirectory)
    {
        try
        {
            _MAAttackWindow.ShowWindow(
             title: "MA Attacks",
             xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
             rootViewName: "MainListRoot",
             saveButtonName: "SaveButton",
             sections: new List<(string, IEnumerable<MAAttackUiConfig>)>
             {
                ("Martial Art Special Attacks", MartialArts),
                ("Dimach Special Attacks", Dimach),
                ("Riposte Special Attacks", Riposte),
                ("Strength Special Attacks", Strength),
                ("Stamina Special Attacks", Stamina),
                ("Intelligence Special Attacks", Intelligence)
             },
             extraSection: (null, null, Option.None),
             sectionIds: MaAttacks,
             entryIds: MaAttacks,
             pluginDirectory: pluginDirectory);
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
