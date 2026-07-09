using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class TauntUiConfig : IUiConfig, IOptionsConfig
{
    public int[] TauntIDs;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => TauntIDs ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
    public List<(string Name, int Value)> Options { get; set; } = new List<(string, int)>();
}

public class TauntWindowController
{
    private GenericWindowController<TauntUiConfig> _TauntWindow;
    public Window CurrentWindow => _TauntWindow.CurrentWindow;

    #region Lists

    public static List<(string, int)> TauntDefualt = new List<(string Name, int Value)> { ("Off", 0), ("Target", 1), ("Adds", 2) };
    public static List<(string, int)> TauntWithAMS = new List<(string Name, int Value)> { ("Off", 0), ("Target", 1), ("Adds", 2), ("AMS/TMS", 3) };


    public static List<TauntUiConfig> Taunts = new List<TauntUiConfig>
    {
        new TauntUiConfig{ TauntIDs = SpellID.SOLDTimedTauntBuffs, SettingKey ="TauntSpellTimedSingleTarget", Label = "Timed Single Target", UiType = UiType.DropDownInputBoxDropDown, Options = TauntWithAMS },

        new TauntUiConfig{ TauntIDs = new[]{ (int)PerkHash.Hatred}, SettingKey = "TauntPerkHatred", Label ="Hatred", UiType= UiType.DropDown },
        new TauntUiConfig{ TauntIDs = new[]{ (int)PerkHash.Taunt}, SettingKey = "TauntPerkTaunt", Label ="Taunt", UiType= UiType.DropDown },
        new TauntUiConfig{ TauntIDs = new[]{ (int)PerkHash.ArouseAnger}, SettingKey = "TauntPerkArouseAnger", Label ="Arouse Anger", UiType= UiType.DropDown },
        new TauntUiConfig{ TauntIDs = new[]{ (int)PerkHash.CauseOfAnger}, SettingKey = "TauntPerkCauseOfAnger", Label ="Cause Of Anger", UiType= UiType.DropDown },

        new TauntUiConfig{ TauntIDs = RelevantGenericItems.TauntTools, SettingKey = "TauntItemTauntTools", Label ="Taunt Tools", UiType =UiType.DropDownWOption},
    };

public static List<TauntUiConfig> AOETaunts = new List<TauntUiConfig>
    {
        new TauntUiConfig{ TauntIDs = Spell.GetSpellsForNanoline(NanoLine.MongoBuff).Select(s=>s.Id).ToArray(), SettingKey ="TauntSpellAOETaunt", Label = "Mongo Buff", UiType = UiType.DropDownInputBoxDropDown, Options = new List<(string Name, int Value)> { ("Off", 0), ("Target", 1), ("Adds", 2), ("Ass Buff", 3) }},
        new TauntUiConfig{ TauntIDs = Spell.GetSpellsForNanoline(NanoLine.AOETauntDOT).Select(s=>s.Id).ToArray(), SettingKey ="TauntSpellAOETaunt", Label = "AOE Taunt DOT", UiType = UiType.DropDownInputBoxDropDown },
    };

#endregion

public TauntWindowController(HandlerSettings settings)
{
    _TauntWindow = new GenericWindowController<TauntUiConfig>(settings);
}

public void TauntShowWindow(string pluginDirectory)
{
    try
    {
        _TauntWindow.ShowWindow(
            title: "Taunts",
            xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
            rootViewName: "MainListRoot",
            saveButtonName: "SaveButton",
            sections: new List<(string, IEnumerable<TauntUiConfig>)>
            {
                ("Taunts", Taunts),
                ("AOE Taunts", AOETaunts),
            },
            extraSection: (null),
            sectionIds: LoadedTaunts,
            entryIds: LoadedTaunts,
            pluginDirectory: pluginDirectory,
            options: TauntDefualt);
    }
    catch (Exception ex)
    {
        ErrorCatch(ex);
    }
}
}
