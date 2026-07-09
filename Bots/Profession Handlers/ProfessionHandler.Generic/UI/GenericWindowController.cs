using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using ProfessionHandler.Generic;
using static ProfessionHandler.Generic.GenericProfessionHandler;

public interface IUiConfig
{
    int[] GetIds();
    string SettingKey { get; }
    string Label { get; }
    int UiType { get; }
}

public interface IOptionsConfig
{
    List<(string Name, int Value)> Options { get; set; }
}

public enum UiType { TextInput, Checkbox, RadioButtonGroup, LabelOnly, DropDown, SingleDropDown, DropDownInputBox, DropDownWOption, DualDropDown, ThreeDropDown, DropDownInputBoxDropDown }

public enum Option { None, Label, Button, DropDown }

public class GenericWindowController<TConfig> where TConfig : IUiConfig
{
    private Window _window;
    public Window CurrentWindow => _window;

    private Dictionary<string, TextInputView> _textInputs = new Dictionary<string, TextInputView>();
    private Dictionary<string, Checkbox> _checkBoxes = new Dictionary<string, Checkbox>();
    private Dictionary<string, RadioButtonGroup> _radioGroups = new Dictionary<string, RadioButtonGroup>();
    private Dictionary<string, DropdownMenu> _dropDowns = new Dictionary<string, DropdownMenu>();

    private HandlerSettings _settings;

    public GenericWindowController(HandlerSettings settings) { _settings = settings; }


    public void ShowWindow(
        string title,
        string xmlPath,
        string rootViewName,
        string saveButtonName,
        List<(List<(string rowTitle, IEnumerable<TConfig> configs)> rows,
              HashSet<int> columnIds,
              HashSet<int> entryIds)> columns,
        (string title, IEnumerable<TConfig> configs, Option option)? extraColumn,
        string pluginDirectory,
        List<(string Name, int Value)> options = null)
    {
        if (_window?.IsValid == true)
        {
            SaveWindowBounds(title);
            _window.Close();
            _window = null;
            return;
        }

        _textInputs.Clear();

        try
        {
            _window = Window.CreateFromXml(title, xmlPath, windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _window.SetTitle(title);
            RestoreWindowBounds(title);

            if (_window.FindView(rootViewName, out View mainRoot))
            {
                mainRoot.DeleteAllChildren();

                foreach (var (rows, columnIds, entryIds) in columns)
                {
                    AddColumn(mainRoot, "Column", pluginDirectory, columnRoot =>
                    {
                        foreach (var (rowTitle, configs) in rows)
                            AddRow(columnRoot, rowTitle, configs, columnIds, entryIds, pluginDirectory, options);
                    });
                }
            }

            _window.FindView("OptionalVeiw", out View optionalVeiw);

            if (extraColumn != null)
            {
                var (extraTitle, extraConfigs, option) = extraColumn.Value;
                View view = null;

                switch (option)
                {
                    case Option.None:
                        break;
                    case Option.Label:
                        view = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\CenteredLabelOnly.xml");
                        if (view.FindChild("TextLabel", out TextView labelView))
                            labelView.Text = extraTitle;
                        break;
                    case Option.Button:
                        view = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\CenteredButtonWithText.xml");
                        if (view.FindChild("TextLabel", out TextView text))
                            text.Text = extraTitle;
                        if (view.FindChild("OptionalButton", out Button optionalButton))
                        {
                            if (DynelManager.LocalPlayer.Profession == Profession.Metaphysicist || DynelManager.LocalPlayer.Profession == Profession.Agent)
                                optionalButton.Show(false, false);
                            optionalButton.SetLabel("Trimmers");
                            optionalButton.Clicked += optionalButton.Clicked += Trimmer_Button_Clicked;
                        }
                        break;
                    case Option.DropDown:
                        view = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\OptionalVeiw.xml");
                        if (view.FindChild("TextLabel", out TextView titleView))
                            titleView.Text = extraTitle;
                        if (view.FindChild("Dropdown", out DropdownMenu menu))
                        {
                            for (int i = extraConfigs.Count() - 1; i >= 0; i--)
                                menu.DeleteItem((uint)i);
                            foreach (var config in extraConfigs)
                                menu.AppendItem(config.Label);
                        }
                        break;
                }

                if (view != null)
                    optionalVeiw.AddChild(view, false);
            }

            if (_window.FindView(saveButtonName, out Button saveButton) && saveButton != null)
                saveButton.Clicked = (s, e) => Save_Button_Clicked(s, e, title, options);

            _window.Show(true);
        }
        catch (Exception ex)
        {
            ErrorCatch(ex);
        }
    }


    public void ShowWindow(
     string title,
     string xmlPath,
     string rootViewName,
     string saveButtonName,
     List<(string sectionTitle, IEnumerable<TConfig> configs)> sections,
     (string title, IEnumerable<TConfig> configs, Option option)? extraSection,
     HashSet<int> sectionIds,
     HashSet<int> entryIds,
     string pluginDirectory,
     List<(string Name, int Value)> options = null)
    {
        ShowWindow(
            title,
            xmlPath,
            rootViewName,
            saveButtonName,
            new List<(List<(string, IEnumerable<TConfig>)>, HashSet<int>, HashSet<int>)>
            {
            (sections, sectionIds, entryIds)
            },
            extraSection,
            pluginDirectory,
            options
        );
    }

    private void SaveWindowBounds(string title)
    {
        if (_window?.IsValid != true) return;

        var frame = _window.GetFrame();
        _settings[$"{title}TopLeftX"] = frame.MinX;
        _settings[$"{title}TopLeftY"] = frame.MinY;
        _settings[$"{title}Width"] = frame.MaxX - frame.MinX;
        _settings[$"{title}Height"] = frame.MaxY - frame.MinY;
    }

    private void RestoreWindowBounds(string title)
    {
        float x = _settings[$"{title}TopLeftX"]?.AsFloat() ?? 50f;
        float y = _settings[$"{title}TopLeftY"]?.AsFloat() ?? 50f;
        float width = _settings[$"{title}Width"]?.AsFloat() ?? 300f;
        float height = _settings[$"{title}Height"]?.AsFloat() ?? 300f;

        _window.MoveTo(x, y);
        _window.ResizeTo(new Vector2(width, height));
    }

    private void AddColumn(View parent, string columnTitle, string pluginDirectory, Action<View> buildRows)
    {
        var column = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\WindowColumn.xml");

        if (column.FindChild("Column", out View root))
            buildRows(root);

        parent.AddChild(column, false);
    }

    private void AddRow(View parent, string rowTitle, IEnumerable<TConfig> configs, HashSet<int> columnIds, HashSet<int> entryIds, string pluginDirectory, List<(string Name, int Value)> options)
    {
        if (!configs.Any(c => c.GetIds().Any(id => columnIds.Contains(id))))
            return;

        var row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\WindowRow.xml");

        if (row.FindChild("Title", out TextView titleView))
            titleView.Text = rowTitle;

        if (row.FindChild("Row", out MultiListView list))
            PopulateList(list, configs, entryIds, pluginDirectory, options);

        parent.AddChild(row, false);
    }

    private void PopulateList(MultiListView listView, IEnumerable<TConfig> configs, HashSet<int> availableIds, string pluginDirectory, List<(string Name, int Value)> options)
    {
        if (listView == null)
            return;

        listView.DeleteAllChildren();

        foreach (var config in configs)
        {
            var ids = config.GetIds();
            if (!ids.Any(id => availableIds.Contains(id)))
                continue;

            View row = null;

            switch ((UiType)config.UiType)
            {
                case UiType.TextInput:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\EntryTextInput.xml");
                        if (row.FindChild("TextLabel", out TextView label1))
                            label1.Text = config.Label;
                        if (row.FindChild("TextInput", out TextInputView input))
                        {
                            var setting = _settings[config.SettingKey];
                            input.Text = (setting != null) ? setting.AsInt32().ToString() : "0";
                            _textInputs[config.SettingKey] = input;
                        }
                    }
                    break;

                case UiType.Checkbox:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\EntryCheckbox.xml");
                        if (row.FindChild("TextLabel", out TextView label2))
                            label2.Text = config.Label;
                        if (row.FindChild("OptionCheckBox", out Checkbox optionCheck))
                        {
                            optionCheck.SetValue(_settings[config.SettingKey]?.AsBool() ?? false);
                            _checkBoxes[config.SettingKey] = optionCheck;
                        }
                    }
                    break;

                case UiType.RadioButtonGroup:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\EntryRadioGroup.xml");
                        if (row.FindChild("TextLabel", out TextView label3))
                            label3.Text = config.Label;
                        if (row.FindChild("OptionRadioButtonGroup", out RadioButtonGroup radioGroup))
                        {
                            int radioValue = _settings[config.SettingKey]?.AsInt32() ?? 0;
                            radioGroup.SetValue(radioValue);
                            _radioGroups[config.SettingKey] = radioGroup;
                        }
                    }
                    break;

                case UiType.LabelOnly:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\EntryLabelOnly.xml");

                        if (row.FindChild("TextLabel", out TextView label4))
                            label4.Text = config.Label;
                    }
                    break;
                case UiType.DropDown:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\EntryDropDown.xml");

                        if (row.FindChild("TextLabel", out TextView label6))
                            label6.Text = config.Label;

                        if (row.FindChild("DropdownC", out DropdownMenu dropDownC))
                        {
                            var optionList = (config as IOptionsConfig)?.Options;
                            if (optionList == null || optionList.Count == 0)
                                optionList = options;

                            if (optionList != null)
                            {
                                for (int i = optionList.Count - 1; i >= 0; i--)
                                    dropDownC.DeleteItem((uint)i);

                                foreach (var p in optionList)
                                    dropDownC.AppendItem(p.Name);

                                _dropDowns[config.SettingKey + "Option"] = dropDownC;
                                dropDownC.Tag = optionList;

                                int idx = optionList.FindIndex(p => p.Value == _settings[config.SettingKey + "Option"].AsInt32());
                                if (idx >= 0)
                                    dropDownC.SelectByIndex((uint)idx, true);
                            }
                        }
                    }
                    break;
                case UiType.SingleDropDown:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\EntryDropDown.xml");

                        if (row.FindChild("TextLabel", out TextView label6))
                            label6.Text = config.Label;

                        if (row.FindChild("DropdownC", out DropdownMenu dropDownD))
                        {
                            for (int i = config.GetIds().Length - 1; i >= 0; i--)
                                dropDownD.DeleteItem((uint)i);

                            foreach (var rawId in config.GetIds().Where(id => availableIds.Contains(id)))
                            {
                                int id = rawId;

                                if (Spell.Find(id, out Spell spell))
                                    dropDownD.AppendItem(spell.Name);

                                if (PerkAction.Find((PerkHash)id, out PerkAction perkAction))
                                {
                                    id = (int)perkAction.Hash;
                                    dropDownD.AppendItem(perkAction.Name);
                                }

                                if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)))
                                    .FirstOrDefault(i => i.Id == id) is Item item)
                                    dropDownD.AppendItem(item.Name);
                            }

                            _dropDowns[config.SettingKey] = dropDownD;

                            if (config.GetIds().Where(id => availableIds.Contains(id)).ToList().IndexOf(_settings[config.SettingKey].AsInt32()) is int idx && idx >= 0)
                                dropDownD.SelectByIndex((uint)idx, true);
                        }
                    }
                    break;

                case UiType.DropDownInputBox:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\DropDownInputBox.xml");

                        if (row.FindChild("TextLabel", out TextView label6))
                            label6.Text = config.Label;

                        if (row.FindChild("DropdownC", out DropdownMenu dropDownC))
                        {
                            var optionList = (config as IOptionsConfig)?.Options;
                            if (optionList == null || optionList.Count == 0)
                                optionList = options;

                            if (optionList != null)
                            {
                                for (int i = optionList.Count - 1; i >= 0; i--)
                                    dropDownC.DeleteItem((uint)i);

                                foreach (var p in optionList)
                                    dropDownC.AppendItem(p.Name);

                                _dropDowns[config.SettingKey + "Option"] = dropDownC;
                                dropDownC.Tag = optionList;

                                int idx = optionList.FindIndex(p => p.Value == _settings[config.SettingKey + "Option"].AsInt32());
                                if (idx >= 0)
                                    dropDownC.SelectByIndex((uint)idx, true);
                            }
                        }

                        if (row.FindChild("TextInput", out TextInputView input))
                        {
                            var setting = _settings[config.SettingKey + "Value"];
                            input.Text = (setting != null) ? setting.AsInt32().ToString() : "0";
                            _textInputs[config.SettingKey + "Value"] = input;
                        }
                    }
                    break;
                case UiType.DropDownWOption:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\DropDownWithCheckBox.xml");

                        if (row.FindChild("OptionCheckBoxB", out Checkbox optionCheck2))
                        {
                            optionCheck2.SetValue(_settings[config.SettingKey + "CheckBox"]?.AsBool() ?? false);
                            _checkBoxes[config.SettingKey + "CheckBox"] = optionCheck2;
                        }

                        if (row.FindChild("TextLabel", out TextView label6))
                            label6.Text = config.Label;

                        if (row.FindChild("DropdownD", out DropdownMenu dropDownD))
                        {
                            for (int i = config.GetIds().Length - 1; i >= 0; i--)
                                dropDownD.DeleteItem((uint)i);

                            foreach (var rawId in config.GetIds().Where(id => availableIds.Contains(id)))
                            {
                                int id = rawId;

                                if (Spell.Find(id, out Spell spell))
                                    dropDownD.AppendItem(spell.Name);

                                if (PerkAction.Find((PerkHash)id, out PerkAction perkAction))
                                {
                                    id = (int)perkAction.Hash;
                                    dropDownD.AppendItem(perkAction.Name);
                                }

                                if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)))
                                    .FirstOrDefault(i => i.Id == id) is Item item)
                                    dropDownD.AppendItem(item.Name);
                            }

                            _dropDowns[config.SettingKey] = dropDownD;

                            if (config.GetIds().Where(id => availableIds.Contains(id)).ToList().IndexOf(_settings[config.SettingKey].AsInt32()) is int idx && idx >= 0)
                                dropDownD.SelectByIndex((uint)idx, true);
                        }
                    }
                    break;
                case UiType.DualDropDown:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\DualEntryDropDown.xml");

                        if (row.FindChild("TextLabel", out TextView label5))
                            label5.Text = config.Label;

                        if (row.FindChild("DropdownA", out DropdownMenu dropDownA))
                        {
                            for (int i = config.GetIds().Length - 1; i >= 0; i--)
                                dropDownA.DeleteItem((uint)i);


                            foreach (var rawId in config.GetIds().Where(id => availableIds.Contains(id)))
                            {
                                int id = rawId;

                                if (Spell.Find(id, out Spell spell))
                                    dropDownA.AppendItem(spell.Name);

                                if (PerkAction.Find((PerkHash)id, out PerkAction perkAction))
                                {
                                    id = (int)perkAction.Hash;
                                    dropDownA.AppendItem(perkAction.Name);
                                }

                                if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)))
                                    .FirstOrDefault(i => i.Id == id) is Item item)
                                    dropDownA.AppendItem(item.Name);
                            }

                            _dropDowns[config.SettingKey] = dropDownA;

                            if (config.GetIds().Where(id => availableIds.Contains(id)).ToList().IndexOf(_settings[config.SettingKey].AsInt32()) is int idx && idx >= 0)
                                dropDownA.SelectByIndex((uint)idx, true);
                        }

                        if (row.FindChild("DropdownC", out DropdownMenu dropDownC))
                        {

                            var optionList = (config as IOptionsConfig)?.Options;
                            if (optionList == null || optionList.Count == 0)
                                optionList = options;

                            if (optionList != null)
                            {
                                for (int i = optionList.Count - 1; i >= 0; i--)
                                    dropDownC.DeleteItem((uint)i);

                                foreach (var p in optionList)
                                    dropDownC.AppendItem(p.Name);

                                _dropDowns[config.SettingKey + "Option"] = dropDownC;
                                dropDownC.Tag = optionList;

                                int idx = optionList.FindIndex(p => p.Value == _settings[config.SettingKey + "Option"].AsInt32());
                                if (idx >= 0)
                                    dropDownC.SelectByIndex((uint)idx, true);
                            }
                        }
                    }
                    break;
                case UiType.ThreeDropDown:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\ThreeEntryDropDown.xml");

                        if (row.FindChild("TextLabel", out TextView label5))
                            label5.Text = config.Label;

                        if (row.FindChild("DropdownA", out DropdownMenu dropDownA))
                        {
                            for (int i = config.GetIds().Length - 1; i >= 0; i--)
                                dropDownA.DeleteItem((uint)i);

                            foreach (var rawId in config.GetIds().Where(id => availableIds.Contains(id)))
                            {
                                int id = rawId;

                                if (Spell.Find(id, out Spell spell))
                                    dropDownA.AppendItem(spell.Name);

                                if (PerkAction.Find((PerkHash)id, out PerkAction perkAction))
                                {
                                    id = (int)perkAction.Hash;
                                    dropDownA.AppendItem(perkAction.Name);
                                }

                                if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)))
                                    .FirstOrDefault(i => i.Id == id) is Item item)
                                    dropDownA.AppendItem(item.Name);
                            }

                            _dropDowns[config.SettingKey] = dropDownA;

                            if (config.GetIds().Where(id => availableIds.Contains(id)).ToList().IndexOf(_settings[config.SettingKey].AsInt32()) is int idx && idx >= 0)
                                dropDownA.SelectByIndex((uint)idx, true);
                        }

                        if (row.FindChild("DropdownB", out DropdownMenu dropDownB))
                        {
                            for (int i = Priorities.Count - 1; i >= 0; i--)
                                dropDownB.DeleteItem((uint)i);


                            foreach (var p in Priorities)
                                dropDownB.AppendItem(p.Name);

                            _dropDowns[config.SettingKey + "Priority"] = dropDownB;

                            if (Priorities.FindIndex(p => p.Value == _settings[config.SettingKey + "Priority"].AsInt32()) is int idx && idx >= 0)
                                dropDownB.SelectByIndex((uint)idx, true);
                        }

                        if (row.FindChild("DropdownC", out DropdownMenu dropDownC))
                        {
                            var optionList = (config as IOptionsConfig)?.Options;
                            if (optionList == null || optionList.Count == 0)
                                optionList = options;

                            if (optionList != null)
                            {
                                for (int i = optionList.Count - 1; i >= 0; i--)
                                    dropDownC.DeleteItem((uint)i);

                                foreach (var p in optionList)
                                    dropDownC.AppendItem(p.Name);

                                _dropDowns[config.SettingKey + "Option"] = dropDownC;
                                dropDownC.Tag = optionList;

                                int idx = optionList.FindIndex(p => p.Value == _settings[config.SettingKey + "Option"].AsInt32());
                                if (idx >= 0)
                                    dropDownC.SelectByIndex((uint)idx, true);
                            }
                        }
                    }
                    break;
                case UiType.DropDownInputBoxDropDown:
                    {
                        row = View.CreateFromXml(pluginDirectory + "\\UI\\ScrollViewLayouts\\DropDownInputBoxDropDown.xml");

                        if (row.FindChild("TextLabel", out TextView label6))
                            label6.Text = config.Label;

                        if (row.FindChild("DropdownA", out DropdownMenu dropDownD))
                        {
                            for (int i = config.GetIds().Length - 1; i >= 0; i--)
                                dropDownD.DeleteItem((uint)i);

                            foreach (var rawId in config.GetIds().Where(id => availableIds.Contains(id)))
                            {
                                int id = rawId;

                                if (Spell.Find(id, out Spell spell))
                                    dropDownD.AppendItem(spell.Name);

                                if (PerkAction.Find((PerkHash)id, out PerkAction perkAction))
                                {
                                    id = (int)perkAction.Hash;
                                    dropDownD.AppendItem(perkAction.Name);
                                }

                                if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity)))
                                    .FirstOrDefault(i => i.Id == id) is Item item)
                                    dropDownD.AppendItem(item.Name);
                            }

                            _dropDowns[config.SettingKey] = dropDownD;

                            if (config.GetIds().Where(id => availableIds.Contains(id)).ToList().IndexOf(_settings[config.SettingKey].AsInt32()) is int idx && idx >= 0)
                                dropDownD.SelectByIndex((uint)idx, true);
                        }

                        if (row.FindChild("TextInput", out TextInputView input))
                        {
                            var setting = _settings[config.SettingKey + "Value"];
                            input.Text = (setting != null) ? setting.AsInt32().ToString() : "0";
                            _textInputs[config.SettingKey + "Value"] = input;
                        }

                        if (row.FindChild("DropdownC", out DropdownMenu dropDownC))
                        {
                            var optionList = (config as IOptionsConfig)?.Options;
                            if (optionList == null || optionList.Count == 0)
                                optionList = options;

                            if (optionList != null)
                            {
                                for (int i = optionList.Count - 1; i >= 0; i--)
                                    dropDownC.DeleteItem((uint)i);

                                foreach (var p in optionList)
                                    dropDownC.AppendItem(p.Name);

                                _dropDowns[config.SettingKey + "Option"] = dropDownC;
                                dropDownC.Tag = optionList;

                                int idx = optionList.FindIndex(p => p.Value == _settings[config.SettingKey + "Option"].AsInt32());
                                if (idx >= 0)
                                    dropDownC.SelectByIndex((uint)idx, true);
                            }
                        }
                    }
                    break;
            }

            if (row != null)
                listView.AddChild(row, false);
        }
    }

    private void Save_Button_Clicked(object sender, ButtonBase e, string title, List<(string Name, int Value)> options)
    {
        foreach (var kvp in _textInputs)
            if (int.TryParse(kvp.Value.Text, out int value))
                _settings[kvp.Key] = value;

        foreach (var kvp in _checkBoxes)
            _settings[kvp.Key] = kvp.Value.IsChecked;

        foreach (var kvp in _radioGroups)
            _settings[kvp.Key] = kvp.Value.GetValue();

        foreach (var kvp in _dropDowns)
        {
            var label = kvp.Value.GetItemLabel(kvp.Value.GetSelection());

            if (kvp.Key.EndsWith("Priority"))
            {
                var Priority = Priorities.FirstOrDefault(x => x.Name == label);
                _settings[kvp.Key] = Priority.Value;
            }
            if (kvp.Key.EndsWith("Option"))
            {
                var optList = kvp.Value.Tag as List<(string Name, int Value)>;
                if (optList != null)
                {
                    var opt = optList.FirstOrDefault(x => x.Name == label);
                    _settings[kvp.Key] = opt.Value;
                }
            }
            else if (Spell.Find(label, out Spell spell))
            {
                _settings[kvp.Key] = spell.Id;
            }
            else if (PerkAction.List.FirstOrDefault(p => p.Name == label) is PerkAction perkAction)
            {
                _settings[kvp.Key] = (int)perkAction.Hash;
            }
            else if (Inventory.Items.Concat(Inventory.Backpacks.SelectMany(bp => Inventory.GetContainerItems(bp.Identity))).FirstOrDefault(i => i.Name == label) is Item item)
            {
                _settings[kvp.Key] = item.Id;
            }
        }

        SaveWindowBounds(title);

        Handler.Save();

        _window?.Close();
        _window = null;
    }
}
