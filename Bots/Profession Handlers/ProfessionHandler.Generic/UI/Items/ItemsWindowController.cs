using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public class ItemUiConfig : IUiConfig
{
    public int[] ItemIds;
    public string SettingKey;
    public string Label;
    public UiType UiType;

    public int[] GetIds() => ItemIds ?? new int[0];
    string IUiConfig.SettingKey => SettingKey;
    string IUiConfig.Label => Label;
    int IUiConfig.UiType => (int)UiType;

    public List<(string Name, int Value)> Options { get; set; }
}

public class ItemsWindowController
{
    private readonly GenericWindowController<ItemUiConfig> _ItemsWindow;
    public Window CurrentWindow => _ItemsWindow.CurrentWindow;

    #region Lists

    public static readonly List<(string Name, int Value)> CombatOptions = new List<(string, int)> { ("Off", 0), ("Target", 1), ("Boss", 2) };

    public static List<ItemUiConfig> DamageItems = new List<ItemUiConfig>
    {
        
    };

    public static List<ItemUiConfig> BuffItems = new List<ItemUiConfig>
    {

    };

    #endregion

    public ItemsWindowController(HandlerSettings settings)
    {
        _ItemsWindow = new GenericWindowController<ItemUiConfig>(settings);
    }

    public void ShowItemsWindow(string pluginDirectory)
    {
        try
        {
            var unconfiguredItems = new List<ItemUiConfig>();

            foreach (var id in OwnedItems)
            {
                if (!DamageItems.Any(c => c.GetIds().Contains(id)) &&
                    !BuffItems.Any(c => c.GetIds().Contains(id)))
                {
                    unconfiguredItems.Add(new ItemUiConfig
                    {
                        ItemIds = new[] { id },
                        SettingKey = null,
                        Label = Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)))
                        .FirstOrDefault(x => x.Id == id)?.Name ?? id.ToString(),

                        UiType = UiType.LabelOnly
                    });
                }
            }

            _ItemsWindow.ShowWindow(
                title: "Items",
                xmlPath: pluginDirectory + "\\UI\\GenericWindow.xml",
                rootViewName: "MainListRoot",
                saveButtonName: "SaveButton",
                sections: new List<(string, IEnumerable<ItemUiConfig>)>
                {
                ("Damage Items", DamageItems),
                ("Buffs", BuffItems),
                },
                extraSection: ("Other Items Used", unconfiguredItems, Option.DropDown),
                sectionIds: OwnedItems,
                entryIds: OwnedItems,
                pluginDirectory: pluginDirectory,
                options: CombatOptions
            );
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }
}
