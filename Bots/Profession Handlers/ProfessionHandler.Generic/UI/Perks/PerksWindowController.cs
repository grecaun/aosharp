using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class PerkUiConfig : IUiConfig
{
    public PerkHash PerkHash;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => new[] { (int)PerkHash };
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
}

public class PerksWindowController
{
    private GenericWindowController<PerkUiConfig> _perksWindow;
    public Window CurrentWindow => _perksWindow.CurrentWindow;

    #region Lists

    public static List<PerkUiConfig> BuffPerks = new List<PerkUiConfig>
    {
       
    };

    #endregion

    public PerksWindowController(HandlerSettings settings)
    {
        _perksWindow = new GenericWindowController<PerkUiConfig>(settings);
    }
    public void ShowPerksWindow(string PluginDirectory)
    {
        try
        {
            //var unconfiguredPerks = new List<PerkUiConfig>();

            //foreach (var perk in OwnedPerks)
            //{
            //    var hash = (PerkHash)perk;

            //    if (!BuffPerks.Any(c => c.PerkHash == hash) )
            //    {
            //        unconfiguredPerks.Add(new PerkUiConfig
            //        {
            //            PerkHash = hash,
            //            SettingKey = null,
            //            Label = PerkAction.List.FirstOrDefault(x => x.Hash == hash)?.Name ?? hash.ToString(),
            //            UiType = UiType.LabelOnly
            //        });
            //    }
            //}

            _perksWindow.ShowWindow(
                title: "Perks",
                xmlPath: PluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<PerkUiConfig>)>
                {
                    ("Buffs", BuffPerks),
                },
                extraSection: null,//("Perks used without options.", unconfiguredPerks, Option.DropDown),
                sectionIds: OwnedPerks,
                entryIds: OwnedPerks,
                pluginDirectory: PluginDirectory
            );
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
