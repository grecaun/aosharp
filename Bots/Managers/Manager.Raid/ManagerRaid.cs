
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerRaid
{
    public class ManagerRaid : AOPluginEntry
    {

        private const string PluginName = "ManagerRaid";
        private const string Version_Number = "0.0.1";
        private static Settings _settings;
        private List<Settings> settingsToSave = new List<Settings>();

        Window _mainWindow;
        Window _infoWindow;

        private string _playersFile => Path.Combine(PluginDataDirectory.FullName, "Players.json");

        Dictionary<string, Identity> _cachedPlayers = new Dictionary<string, Identity>();
        Dictionary<string, Identity> _savedPlayers = new Dictionary<string, Identity>();

        public override void Run()
        {

            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            _settings = new Settings(PluginName);

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            settingsToSave.Add(_settings);

            Chat.RegisterCommand(PluginName, ManagerCommand);

            LoadPlayers();

            //foreach (var player in DynelManager.Players.Where(p=>p.Identity != DynelManager.LocalPlayer.Identity))
            //{
            //    if (!_cachedPlayers.ContainsKey(player.Name))
            //    {
            //        _cachedPlayers[player.Name] = player.Identity;
            //    }
            //}

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

            Save();

            UIController.WindowDeleted -= Windowclosed;
            Network.N3MessageSent -= N3MessageSent;

            return;
        }

        private void Window_Closed_helper()
        {
            if (_mainWindow?.IsValid == true)
            {
                Rect frame = _mainWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        private void ManagerCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (_mainWindow?.IsValid == true)
            {
                Window_Closed_helper();

                _mainWindow.Close();
                _mainWindow = null;
                return;
            }
            else
            {
                _mainWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\RaidMainWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                _mainWindow.Show(true);

                if (_mainWindow.FindView("InfoButton", out Button infoButton))
                    infoButton.Clicked = Info_Button_Clicked;

                if (_mainWindow.FindView("RefreshButton", out Button refreshButton))
                    refreshButton.Clicked = Refresh_Button_Clicked;

                if (_mainWindow.FindView("InviteButton", out Button inviteButton))
                    inviteButton.Clicked = Invite_Button_Clicked;

                if (_mainWindow.FindView("AddPlayerDropdown", out DropdownMenu addDropdown))
                {
                    for (uint i = (uint)_cachedPlayers.Count - 1; i < uint.MaxValue; i--)
                        addDropdown.DeleteItem(i);

                    foreach (var name in _cachedPlayers.Keys.OrderBy(p => p))
                        addDropdown.AppendItem(name);
                }

                if (_mainWindow.FindView("SavePlayer", out Button addHelperView))
                    addHelperView.Clicked = Add_Button_Clicked;


                if (_mainWindow.FindView("SavedDropdown", out DropdownMenu menu))
                {
                    LoadPlayers();

                    for (int i = (int)_savedPlayers.Count - 1; i >= 0; i--)
                        menu.DeleteItem((uint)i);

                    foreach (var item in _savedPlayers.Keys.OrderBy(p => p))
                        menu.AppendItem(item);
                }

                if (_mainWindow.FindView("RemovePlayer", out Button removeHelperView))
                    removeHelperView.Clicked = Remove_Button_Clicked;

                if (_mainWindow.FindView("ClearPlayers", out Button clearHelpersView))
                    clearHelpersView.Clicked = Clear_Button_Clicked;

                if (_mainWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version = {Version_Number}";
            }
        }

        private void Info_Button_Clicked(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }
            else
            {
                _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\RaidInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade );
                _infoWindow.Show(true);
            }
        }

        private void Refresh_Button_Clicked(object sender, ButtonBase e)
        {
            if (_mainWindow.FindView("AddPlayerDropdown", out DropdownMenu dropdown))
            {
                for (int i = _cachedPlayers.Count - 1; i >= 0; i--)
                    dropdown.DeleteItem((uint)i);

                _cachedPlayers.Clear();

                foreach (var player in DynelManager.Players.OrderBy(p => p.Name))
                {
                    if (player.Identity == DynelManager.LocalPlayer.Identity) continue;
                    if (Team.IsInTeam && Team.Members.Contains(player.Identity)) continue;
                    _cachedPlayers[player.Name] = player.Identity;
                    dropdown.AppendItem(player.Name);
                }
            }
        }

        private void Invite_Button_Clicked(object sender, ButtonBase e)
        {
            if (!Team.IsInTeam)
            {
                if (_mainWindow.FindView("ErrorMessage", out TextView error))
                    error.Text = $"ERROR = FORM TEAM FIRST!";
                return;
            }

            if (!Team.IsRaid)
                Team.ConvertToRaid();

            foreach (var player in _savedPlayers.Values)
            {
                if (Team.Members.Contains(player)) continue;
                Team.Invite(player);
            }
        }

        private void Add_Button_Clicked(object s, ButtonBase button)
        {
            if (_mainWindow.FindView("AddPlayerDropdown", out DropdownMenu addDropdown) && _mainWindow.FindView("SavedDropdown", out DropdownMenu saveDropdown))
            {
                uint selectedIndex = addDropdown.GetSelection();
                string entry = addDropdown.GetItemLabel(selectedIndex);

                if (!string.IsNullOrWhiteSpace(entry) && !_savedPlayers.ContainsKey(entry))
                {
                    if (_cachedPlayers.TryGetValue(entry, out var identity))
                        _savedPlayers[entry] = identity;

                    saveDropdown.AppendItem(entry);
                    SavePlayers();
                }
            }
        }

        private void Remove_Button_Clicked(object s, ButtonBase button)
        {
            if (_mainWindow.FindView("SavedDropdown", out DropdownMenu menu))
            {
                uint selectedIndex = menu.GetSelection();

                if (selectedIndex < _savedPlayers.Count)
                {
                    var key = _savedPlayers.Keys.ElementAt((int)selectedIndex);
                    _savedPlayers.Remove(key);
                    menu.DeleteItem(selectedIndex);
                    SavePlayers();
                }
            }
        }

        private void Clear_Button_Clicked(object s, ButtonBase button)
        {
            if (_mainWindow.FindView("SavedDropdown", out DropdownMenu menu))
            {
                for (int i = _savedPlayers.Count - 1; i >= 0; i--)
                    menu.DeleteItem((uint)i);

                _savedPlayers.Clear();
                SavePlayers();
                menu.Update();
            }
        }

        private void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        private void SavePlayers()
        {
            File.WriteAllText(_playersFile, JsonConvert.SerializeObject(_savedPlayers));
        }

        private void LoadPlayers()
        {
            try
            {
                if (File.Exists(_playersFile))
                    _savedPlayers = JsonConvert.DeserializeObject<Dictionary<string, Identity>>(File.ReadAllText(_playersFile)) ?? new Dictionary<string, Identity>();
            }
            catch (Exception ex)
            {
                Chat.WriteLine($"Failed to load Players.json: {ex.Message}");
                _savedPlayers = new Dictionary<string, Identity>();
            }
        }
    }
}
