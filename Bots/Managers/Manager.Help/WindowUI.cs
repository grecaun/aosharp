using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using ManagerHelp.IPCMessages;
using Shared;

namespace ManagerHelp
{
    public partial class ManagerHelp
    {
        private List<string> ErrorMessages = new List<string>();
        Window _yalmWindow;

        private void ManagerCommand(string arg1, string[] arg2, ChatWindow window)
        {
            try
            {
                if (settingsWindow?.IsValid == true)
                {
                    Window_Closed_helper();

                    settingsWindow.Close();
                    settingsWindow = null;
                    return;
                }

                settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerHelpSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

                if (settingsWindow.FindView("ManagerHelpInfoView", out Button infoView))
                    infoView.Clicked = HandleInfoViewClick;

                if (settingsWindow.FindView("KitHealthPercentageBox", out TextInputView kitHealthInput))
                    kitHealthInput.Text = $"{_settings["KitHealthPercentageBox"].AsInt32()}";

                if (settingsWindow.FindView("KitNanoPercentageBox", out TextInputView kitNanoInput))
                    kitNanoInput.Text = $"{_settings["KitNanoPercentageBox"].AsInt32()}";

                if (settingsWindow.FindView("KitSave", out Button kitSaveButton))
                    kitSaveButton.Clicked = Save_Button_Clicked;

                if (settingsWindow.FindView("BroadcastSettingsView", out Button settingsButton))
                    settingsButton.Clicked = UISettingsButtonClicked;

                if (settingsWindow.FindView("EumenidesPositionsView", out Button eumenidesView))
                    eumenidesView.Clicked = EumenidesView;

                if (settingsWindow.FindView("YalmButton", out Button yalmButton))
                    yalmButton.Clicked = YalmButtonClicked;

                if (settingsWindow.FindView("Errors", out View errorView))
                    PopulateErrorView(errorView);

                if (settingsWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";

                settingsWindow.Show(true);

            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Button Clicked

        private void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
                return;
            }

            _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerHelpInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            _infoWindow.Show(true);
        }

        private void Save_Button_Clicked(object sender, ButtonBase e)
        {
            if (settingsWindow.FindView("KitHealthPercentageBox", out TextInputView kitHealthInput) &&
            int.TryParse(kitHealthInput.Text, out int kitHealthValue))
            {
                if (_settings["KitHealthPercentageBox"].AsInt32() != kitHealthValue)
                    _settings["KitHealthPercentageBox"] = kitHealthValue;
            }

            if (settingsWindow.FindView("KitNanoPercentageBox", out TextInputView kitNanoInput) &&
                int.TryParse(kitNanoInput.Text, out int kitNanoValue))
            {
                if (_settings["KitNanoPercentageBox"].AsInt32() != kitNanoValue)
                    _settings["KitNanoPercentageBox"] = kitNanoValue;
            }
            Save();
        }

        private void UISettingsButtonClicked(object s, ButtonBase button)
        {
            IPCChannel.Broadcast(new UISettings()
            {
                AutoSit = _settings["AutoSit"].AsBool(),
                MorphPathing = _settings["MorphPathing"].AsBool(),
                BellyPathing = _settings["BellyPathing"].AsBool(),
                Eumenides = _settings["Eumenides"].AsBool(),
                Db3Shapes = _settings["Db3Shapes"].AsBool(),
            });

            Save();
        }

        private void EumenidesView(object s, ButtonBase button)
        {
            if (_eumenidesWindow?.IsValid == true)
            {
                _eumenidesWindow.Close();
                _eumenidesWindow = null;
                return;
            }

            _eumenidesWindow = Window.CreateFromXml("Eumenides", PluginDirectory + "\\UI\\ManagerHelpEumenidesView.xml",
                windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            _eumenidesWindow.MoveTo(390, 50);
            _eumenidesWindow.Show(true);
        }

        private void YalmButtonClicked(object sender, ButtonBase e)
        {
            if (_yalmWindow?.IsValid == true)
            {
                _yalmWindow.Close();
                _yalmWindow = null;
                return;
            }

            _yalmWindow = Window.CreateFromXml("Flight", PluginDirectory + "\\UI\\yalms.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            if (_yalmWindow.FindView("RKYalms", out DropdownMenu rkYalms))
            {
                rkList = Spell.List.Where(s => YalmSpells.Contains(s.Id) || Hoverboards.Contains(s.Id) || SLVehicles.Contains(s.Id) || Sparrow.Contains(s.Id)).Select(s => (s.Name, s.Id))
                    .Concat(Inventory.Items.Where(i => i.Name.Contains("Yalm") || i.Name.Contains("Ganimedes") || GraftSparrowFlight.Contains(i.Id)).Select(i => (i.Name, i.Id))).ToList();

                BuildAndSelect(rkYalms, rkList, _settings["RKYalm"].AsInt32());
            }

            if (_yalmWindow.FindView("SLYalms", out DropdownMenu slYalms))
            {
                slList = Spell.List.Where(s => SLVehicles.Contains(s.Id) || Hoverboards.Contains(s.Id) || Sparrow.Contains(s.Id)).Select(s => (s.Name, s.Id))
                    .Concat(Inventory.Items.Where(i => GraftSparrowFlight.Contains(i.Id)).Select(i => (i.Name, i.Id))).ToList();

                BuildAndSelect(slYalms, slList, _settings["SLYalm"].AsInt32());
            }

            if (_yalmWindow.FindView("BroadcastYalm", out Button broadcastButton))
                broadcastButton.Clicked = Broadcast_Yalms_Button_Clicked;

            if (_yalmWindow.FindView("Save", out Button saveButton))
                saveButton.Clicked = Yalm_Save_Button_Clicked;

            _yalmWindow.Show(true);
        }

        private void BuildAndSelect(DropdownMenu menu, List<(string Name, int Id)> list, int saved)
        {
            for (uint i = 0; i < list.Count; i++)
                menu.DeleteItem(i);

            for (uint i = 0; i < list.Count; i++)
                menu.AppendItem(list[(int)i].Name);

            if (saved != 0)
            {
                int index = list.FindIndex(x => x.Id == saved);
                if (index >= 0)
                    menu.SelectByIndex((uint)index, true);
            }
        }

        private void Broadcast_Yalms_Button_Clicked(object sender, ButtonBase e)
        {
            IPCChannel.Broadcast(new BroadcastYalms() { RKSelection = _settings["RKYalm"].AsInt32(), SLSelection = _settings["SLYalm"].AsInt32() });
        }

        private void Yalm_Save_Button_Clicked(object sender, ButtonBase e)
        {
            _yalmWindow.FindView("RKYalms", out DropdownMenu rkYalms);
            _yalmWindow.FindView("SLYalms", out DropdownMenu slYalms);

            int rkIndex = (int)rkYalms.GetSelection();
            int slIndex = (int)slYalms.GetSelection();

            if (rkIndex >= 0 && rkIndex < rkList.Count)
                _settings["RKYalm"] = rkList[rkIndex].Id;

            if (slIndex >= 0 && slIndex < slList.Count)
                _settings["SLYalm"] = slList[slIndex].Id;
        }

        #endregion

        private void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (settingsWindow != null && settingsWindow.IsValid && settingsWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);
        }


        private void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { "   at " }, StringSplitOptions.None);

                    View xmlRoot = View.CreateFromXml($"{PluginDirectory}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                    xmlRoot.FindChild("TextLabel", out TextView labelView);
                    labelView.Text = parts[0];
                    labelView.SetColor(Color.Red);
                    errorView.AddChild(xmlRoot, true);

                    if (parts.Length > 1)
                    {
                        View methodRoot = View.CreateFromXml($"{PluginDirectory}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        methodRoot.FindChild("TextLabel", out TextView methodLabel);
                        methodLabel.Text = "at " + parts[1];
                        errorView.AddChild(methodRoot, true);
                    }
                }
            }
        }
    }
}
