using System;
using System.Collections.Generic;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class TrimmerUiConfig : IUiConfig
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

public class TrimmersWindowController
{
    private GenericWindowController<TrimmerUiConfig> _TrimmerWindow;
    public Window CurrentWindow => _TrimmerWindow.CurrentWindow;

    #region Lists

    public static List<TrimmerUiConfig> AttackPet = new List<TrimmerUiConfig>
    {
        new TrimmerUiConfig { ItemIds = IncreaseAggressiveness, SettingKey = "AttackIncreaseAggressivenessCheckBox", Label = "Increase Aggressiveness", UiType = UiType.Checkbox },

        new TrimmerUiConfig { ItemIds = DmgChangeSelectionArray, SettingKey = "AttackDmgChangeSelectionArray", Label = "Damage Change", UiType = UiType.DropDownWOption },

        new TrimmerUiConfig { ItemIds = MechEngiSelectionArray, SettingKey = "AttackMechEngiSelectionArray", Label = "Mech. Engi", UiType = UiType.DropDownWOption },

        new TrimmerUiConfig { ItemIds = ElecEngiSelectionArray, SettingKey = "AttackElecEngiSelectionArray", Label = "Elec. Engi", UiType = UiType.DropDownWOption },

        new TrimmerUiConfig { ItemIds = AggressiveDefensiveSelectionArray, SettingKey = "AttackAggressiveDefensiveSelectionArray", Label = "Aggressive/Defensive", UiType = UiType.DropDownWOption },

    };

    public static List<TrimmerUiConfig> SupportPet = new List<TrimmerUiConfig>
    {
        new TrimmerUiConfig { ItemIds = DmgChangeSelectionArray, SettingKey = "SupportDmgChangeSelectionArray", Label = "Damage Change", UiType = UiType.DropDownWOption },
    };

    #endregion

    public TrimmersWindowController(HandlerSettings settings)
    {
        _TrimmerWindow = new GenericWindowController<TrimmerUiConfig>(settings);
    }

    public void TrimmerShowWindow(string pluginDirectory)
    {
        try
        {
            _TrimmerWindow.ShowWindow(
             title: "Trimmers",
             xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
             rootViewName: "MainListRoot",
             saveButtonName: "SaveButton",
             sections: new List<(string, IEnumerable<TrimmerUiConfig>)> { ("Attack", AttackPet), ("Support", SupportPet)},
             extraSection: (null, null, Option.None),
             sectionIds: OwnedTrimmers,
             entryIds: OwnedTrimmers,
             pluginDirectory: pluginDirectory);

        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
