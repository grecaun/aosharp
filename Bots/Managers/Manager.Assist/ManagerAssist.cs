using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerAssist
{
    public class ManagerAssist : AOPluginEntry
    {
        private const string PluginName = "ManagerAssist";
        private string Version_Number = "2.0.0";

        private Settings _settings;

        Window settingsWindow;
        Window _infoWindow;

        private static List<Settings> _settingsToSave = new List<Settings>();
        public static string EnableString;

        public static List<string> AssistPlayers = new List<string>();

        private static double _assistTimer;

        public static string previousErrorMessage = string.Empty;

        private string _assistPlayersFile => Path.Combine(PluginDataDirectory.FullName, "AssistPlayers.json");

        public override void Run()
        {
            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            _settings = new Settings(PluginName);

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            _settingsToSave.Add(_settings);

            Chat.RegisterCommand(PluginName, ManagerCommand);
            Chat.RegisterCommand(PluginName, ManagerAssistCommand);

            LoadAssistPlayers();

            UIController.WindowDeleted += Windowclosed;
            Network.N3MessageSent += N3MessageSent;

            Chat.WriteLine($"{PluginName} Loaded!");
            Chat.WriteLine($"/{PluginName} for UI.");
            Chat.WriteLine($"/macro {PluginName} /{PluginName}");
        }

        public override void Teardown()
        {
            Save();
            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
        }

        private void Windowclosed(object sender, Window e)
        {
            switch (e.Name)
            {
                case PluginName:
                    Window_Closed_helper();
                    break;
            }
        }

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            _settings["Enable"] = false;
            EnableString = "Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Save();

            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;

            return;
        }

        private void Window_Closed_helper()
        {
            if (settingsWindow?.IsValid == true)
            {
                Rect frame = settingsWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

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

                settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerAssistSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                settingsWindow.Show(true);

                if (settingsWindow.FindView("InfoView", out Button infoView))
                    infoView.Clicked = HandleInfoViewClick;

                if (settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                {
                    enableButton.SetLabel(EnableString);
                    enableButton.Clicked = Enable_Disable_Button_Clicked;
                }

                if (settingsWindow.FindView("NameDropdown", out DropdownMenu menu))
                {
                    for (int i = (int)AssistPlayers.Count - 1; i >= 0; i--)
                        menu.DeleteItem((uint)i);

                    LoadAssistPlayers();

                    foreach (var item in AssistPlayers)
                        menu.AppendItem(item);

                }

                if (settingsWindow.FindView("AddHelper", out Button addHelperView))
                    addHelperView.Clicked = HandleAddHelperViewClick;

                if (settingsWindow.FindView("RemoveHelper", out Button removeHelperView))
                    removeHelperView.Clicked = HandleRemoveHelperViewClick;

                if (settingsWindow.FindView("ClearHelpers", out Button clearHelpersView))
                    clearHelpersView.Clicked = HandleClearHelpersViewClick;

                if (settingsWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";

            }
            catch (Exception e) { Chat.WriteLine(e.Message); }
        }

        private void ManagerAssistCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length < 1)
            {
                Helper_Enable();
            }
        }

        private void Enable_Disable_Button_Clicked(object sender, ButtonBase e)
        {
            Helper_Enable();
        }

        private void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
                return;
            }

            _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerAssistInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _infoWindow.Show(true);
        }

        private void HandleAddHelperViewClick(object s, ButtonBase button)
        {
            if (settingsWindow?.IsValid == true)
            {
                settingsWindow.FindView("NameInputBox", out TextInputView nameInput);
                settingsWindow.FindView("NameDropdown", out DropdownMenu menu);

                if (nameInput != null && !string.IsNullOrWhiteSpace(nameInput.Text))
                {
                    string entry = nameInput.Text.Trim();

                    if (!AssistPlayers.Contains(entry))
                    {
                        AssistPlayers.Add(entry);
                        menu.AppendItem(entry);
                        SaveAssistPlayers();
                    }

                    nameInput.Text = "";
                }
                else if (Targeting.Target != null)
                {
                    var name = Targeting.Target.Name;
                    if (!AssistPlayers.Contains(name))
                    {
                        AssistPlayers.Add(name);
                        menu.AppendItem(name);
                        SaveAssistPlayers();
                    }
                }
            }
        }

        private void HandleRemoveHelperViewClick(object s, ButtonBase button)
        {
            if (settingsWindow?.IsValid == true && settingsWindow.FindView("NameDropdown", out DropdownMenu menu))
            {
                uint selectedIndex = menu.GetSelection();

                if (selectedIndex < AssistPlayers.Count)
                {
                    AssistPlayers.RemoveAt((int)selectedIndex);
                    menu.DeleteItem(selectedIndex);
                    SaveAssistPlayers();
                }
            }
        }

        private void HandleClearHelpersViewClick(object s, ButtonBase button)
        {
            if (settingsWindow.FindView("NameDropdown", out DropdownMenu menu))
            {
                for (int i = (int)AssistPlayers.Count - 1; i >= 0; i--)
                    menu.DeleteItem((uint)i);
            }

            AssistPlayers.Clear();
            SaveAssistPlayers();
            menu.Update();
        }

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                if (Time.AONormalTime < _assistTimer + 0.5)
                    return;

                var localPlayer = DynelManager.LocalPlayer;

                var assistTarget = DynelManager.Characters
                    .Where(c => AssistPlayers.Contains(c.Name) && c.Health > 0)
                    .OrderBy(c =>
                        c.IsAttacking && c.FightingTarget != null
                            ? localPlayer.Position.Distance2DFrom(c.FightingTarget.Position)
                            : float.MaxValue)
                    .FirstOrDefault();

                if (assistTarget == null)
                    return;

                var target = assistTarget.FightingTarget;
                var current = localPlayer.FightingTarget;

                if (!assistTarget.IsAttacking && current != null)
                {
                    localPlayer.StopAttack(false);
                }
                else if (assistTarget.IsAttacking && target != null && (current == null || current.Identity != target.Identity))
                {
                    localPlayer.Attack(target, false);
                }

                _assistTimer = Time.AONormalTime;
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred on line " + GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != previousErrorMessage)
                {
                    Chat.WriteLine(errorMessage);
                    Chat.WriteLine("Stack Trace: " + ex.StackTrace);
                    previousErrorMessage = errorMessage;
                }
            }
        }

        private void Save()
        {
            _settingsToSave.ForEach(settings => settings.Save());
        }
        private void Helper_Enable()
        {
            _settings["Enable"] = !_settings["Enable"].AsBool();
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            if (_settings["Enable"].AsBool())
            {
                Chat.WriteLine($"{PluginName} enabled");
                Game.OnUpdate += OnUpdate;
            }
            else
            {
                Chat.WriteLine($"{PluginName} disabled");
                Game.OnUpdate -= OnUpdate;
            }

            Save();
        }
        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;

            var lineMatch = Regex.Match(ex.StackTrace ?? "", @":line (\d+)$", RegexOptions.Multiline);

            if (lineMatch.Success)
            {
                lineNumber = int.Parse(lineMatch.Groups[1].Value);
            }

            return lineNumber;
        }

        private void SaveAssistPlayers()
        {
            File.WriteAllText(_assistPlayersFile, JsonConvert.SerializeObject(AssistPlayers));
        }

        private void LoadAssistPlayers()
        {
            if (File.Exists(_assistPlayersFile))
            {
                AssistPlayers = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_assistPlayersFile)) ?? new List<string>();
            }
        }
    }
}
