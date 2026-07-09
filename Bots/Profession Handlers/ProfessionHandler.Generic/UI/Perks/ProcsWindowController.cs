using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using ProfessionHandler.Generic;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class ProcUiConfig : IUiConfig
{
    public int[] ProcIds;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    
    public int[] GetIds() => ProcIds ?? new int[0];

    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;
}

public class ProcsWindowController
{
    private GenericWindowController<ProcUiConfig> _ProcsWindow;
    public Window CurrentWindow => _ProcsWindow.CurrentWindow;

    #region Lists

    public static List<ProcUiConfig> ProcType1 = new List<ProcUiConfig>
    {
        new ProcUiConfig { ProcIds = OwnedProcsType1.ToArray(), SettingKey = "ProcType1Selection1", Label = "Selection 1", UiType = UiType.SingleDropDown },
        new ProcUiConfig { ProcIds = OwnedProcsType1.ToArray(), SettingKey = "ProcType1Selection2", Label = "Selection 2", UiType = UiType.SingleDropDown },
    };

    public static List<ProcUiConfig> ProcType2 = new List<ProcUiConfig>
    {
       new ProcUiConfig { ProcIds = OwnedProcsType2.ToArray(), SettingKey = "ProcType2Selection1", Label = "Selection 1", UiType = UiType.SingleDropDown },
       new ProcUiConfig { ProcIds = OwnedProcsType2.ToArray(), SettingKey = "ProcType2Selection2", Label = "Selection 2", UiType = UiType.SingleDropDown },
    };

    #endregion

    public ProcsWindowController(HandlerSettings settings)
    {
        _ProcsWindow = new GenericWindowController<ProcUiConfig>(settings);
    }
    public void ShowProcsWindow(string PluginDirectory)
    {
        try
        {
            _ProcsWindow.ShowWindow(
                title: "Procs",
                xmlPath: PluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<ProcUiConfig>)>
                {
                ("Type 1", ProcType1),
                ("Type 2", ProcType2)
                },
                extraSection: (ProcDescription(), Enumerable.Empty<ProcUiConfig>(), Option.Label),
                sectionIds: OwnedProcs,
                entryIds: OwnedProcs,
                pluginDirectory: PluginDirectory
            );
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
