using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using ManagerAttack.IPCMessages;
using Newtonsoft.Json;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerAttack
{
    public partial class ManagerAttack : AOPluginEntry
    {
        const string PluginName = "ManagerAttack";
        private string Version_Number = "2.0.4";

        private static InstancesConfig _config;
        private static string _configPath;

        public static IPCChannel IPCChannel { get; private set; }

        public static bool Enable = false;
        public static string EnableString = "Enable";
        private string TauntString;
        public static bool _init = false;

        public static Identity Leader = Identity.None;

        public static double _stateTimeOut = Time.AONormalTime;

        double BroadcastTime;

        public static Window _settingsWindow;
        private Window _infoWindow;
        private Window _helperWindow;
        private Window _advSettingsWindow;

        public static Settings _settings;

        public static List<string> _helpers = new List<string>();

        public static List<string> ErrorMessages = new List<string>();

        private readonly List<Settings> settingsToSave = new List<Settings>();

        Kits kits = new Kits();

        public static string PluginDir;

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                PluginDir = PluginDirectory;

                _settings = new Settings(PluginName);

                _configPath = Path.Combine(PluginDirectory, "Instances.json");

                if (File.Exists(_configPath))
                {
                    _config = JsonConvert.DeserializeObject<InstancesConfig>(File.ReadAllText(_configPath));
                }
                else
                {
                    _config = new InstancesConfig();
                    File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
                }
                
                _settings.AddVariable("Taunt", false);
                TauntString = _settings["Taunt"].AsBool() ? "Taunt Disable" : " Taunt Enable";

                _settings.AddVariable("TeamLead", false);
                _settings.AddVariable("Switching", false);
                _settings.AddVariable("Rings", false);

                _settings.AddVariable("AttackRange", 20);
                _settings.AddVariable("TauntRange", 21);

                _settings.AddVariable("IPCChannel", 48);
                _settings["IPCChannel"] = 48;

                _settings.AddVariable("Pandemonium", 0);
                _settings.AddVariable("12M", 0);

                _settings.AddVariable("KitNanoPercentageBox", 0);
                _settings.AddVariable("KitHealthPercentageBox", 0);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                settingsToSave.Add(_settings);

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, OnStartStopMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.RangeInfo, OnRangeInfoMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.LeaderInfo, OnLeaderInfoMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.Update, UpdateJson);

                Chat.RegisterCommand("enable", EnableCommand);
                Chat.RegisterCommand("buddy", BuddyCommand);
                Chat.RegisterCommand("buddychannel", ChannelCommand);

                Chat.RegisterCommand("info", (string command, string[] param, ChatWindow chatWindow) =>
                        {
                            if (Leader != Identity.None)
                            {
                                var leader = DynelManager.Players.FirstOrDefault(p => p.Identity == Leader);
                                if (leader != null)
                                {
                                    Chat.WriteLine($"Leader is {leader.Name}");
                                }
                            }

                            if (DynelManager.LocalPlayer.Identity == Leader)
                            {
                                foreach (var mob in _mob)
                                {
                                    Chat.WriteLine($"mob: {mob.Name}");
                                }

                                foreach (var boss in _bossMob)
                                {
                                    Chat.WriteLine($"Boss: {boss.Name}");
                                }

                                foreach (var switchMob in _switchMob)
                                {
                                    Chat.WriteLine($"Switch mob: {switchMob.Name}");
                                }
                            }
                        });

                Game.OnUpdate += OnUpdate;
                Network.N3MessageSent += N3MessageSent;
                UIController.WindowDeleted += Windowclosed;
                Game.TeleportStarted += TeleportStarted;

                Chat.WriteLine($"{PluginName} Loaded!");
                Chat.WriteLine("/buddy opens and closes the UI. /enable to start and stop.");
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region Helpers

        private void Window_Closed_helper()
        {
            if (_settingsWindow?.IsValid == true)
            {
                Rect frame = _settingsWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            _settingsWindow?.Close();
            _infoWindow?.Close();
        }

        private void Start()
        {
            Chat.WriteLine($"{PluginName} enabled.");
        }

        private void Stop()
        {
            if (DynelManager.LocalPlayer.IsAttacking)
                DynelManager.LocalPlayer.StopAttack(false);

            Chat.WriteLine($"{PluginName} disabled.");
        }

        #endregion

        #region Received IPC Messages

        private void UpdateJson(int arg1, IPCMessage message)
        {
            _config = JsonConvert.DeserializeObject<InstancesConfig>(File.ReadAllText(_configPath));
        }

        private void OnStartStopMessage(int sender, IPCMessage msg)
        {
            if (!(msg is StartStopIPCMessage startStopMessage)) return;
            if (startStopMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            Enable = startStopMessage.IsStarting;

            if (Enable)
                Start();
            else
                Stop();
        }

        private void OnRangeInfoMessage(int sender, IPCMessage msg)
        {
            if (!(msg is RangeInfoIPCMessage rangeInfoMessage)) return;
            if (rangeInfoMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            _settings["AttackRange"] = rangeInfoMessage.AttackRange;
            _settings["TauntRange"] = rangeInfoMessage.TauntRange;

            if (_settingsWindow?.IsValid == true)
            {
                if (_settingsWindow.FindView("AttackRangeBox", out TextInputView attackRangeInput))
                    attackRangeInput.Text = _settings["AttackRange"].ToString();

                if (_settingsWindow.FindView("TauntRangeBox", out TextInputView tauntRangeInput))
                    tauntRangeInput.Text = _settings["TauntRange"].ToString();
            }

            Save();
        }

        private void OnLeaderInfoMessage(int sender, IPCMessage msg)
        {
            if (msg is LeaderInfoIPCMessage leaderInfoMessage)
            {
                //Chat.WriteLine($"Received request: {leaderInfoMessage.Request}");

                switch (leaderInfoMessage.Request)
                {
                    case 0: // no
                        //var leader = DynelManager.Players.FirstOrDefault(l => l.Identity == leaderInfoMessage.LeaderIdentity);
                        //Chat.WriteLine($"New leader received, leader = {leader.Name}");
                        Leader = leaderInfoMessage.LeaderIdentity;
                        break;
                    case 1: // yes
                        if (DynelManager.LocalPlayer.Identity == Leader)
                        {
                            //Chat.WriteLine("Received leader request.");
                            IPCChannel.Broadcast(new LeaderInfoIPCMessage() { LeaderIdentity = Leader, Request = 0 });
                        }
                        break;
                    case 2: // reset leader.
                            //Chat.WriteLine("Resetting leader");
                        Leader = Identity.None;

                        break;
                }
            }
        }

        #endregion

        #region Events

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            Enable = false;
            EnableString = "Enable";

            if (_settingsWindow?.IsValid == true && _settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Stop();
            Save();

            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            return;
        }
        private void TeleportStarted(object sender, EventArgs e)
        {
            Enable = false;
            Leader = Identity.None;
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

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                var localPlayer = DynelManager.LocalPlayer;

                if (Game.IsZoning)
                {
                    Enable = false;
                    Leader = Identity.None;
                    return;
                }

                if (_settings["Rings"].AsBool())
                {
                    Debug_DrawCircle(localPlayer.Position, _settings["AttackRange"].AsInt32(), DebuggingColor.Red);
                    if (_settings["Taunt"].AsBool())
                        Debug_DrawCircle(localPlayer.Position, _settings["TauntRange"].AsInt32(), DebuggingColor.Green);
                }

                if (Team.IsInTeam)
                {
                    if (!_settings["TeamLead"].AsBool())
                    {
                        if (Leader == Identity.None)
                        {
                            if (Time.AONormalTime > BroadcastTime)
                            {
                                //Chat.WriteLine("Leader is none. Requesting leader from IPC.");
                                IPCChannel.Broadcast(new LeaderInfoIPCMessage() { Request = 1 });
                                //LeaderTimeout = Time.AONormalTime + 0.5;
                                BroadcastTime = Time.AONormalTime + 1.0;
                            }
                        }
                    }
                    else
                    {
                        if (Leader != localPlayer.Identity)
                        {
                            //Chat.WriteLine($"Team Lead checked, leader = {localPlayer.Name}.");
                            IPCChannel.Broadcast(new LeaderInfoIPCMessage() { LeaderIdentity = localPlayer.Identity, Request = 0 });
                            Leader = localPlayer.Identity;
                        }
                    }
                }

                if (!Enable) { return; }

                AttackBase();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #endregion

        #region Chat Commands

        private void EnableCommand(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {
                if (param.Length < 1)
                {
                    Handle_Enable();
                    return;
                }

                switch (param[0].ToLower())
                {
                    case "ignore":
                        if (param.Length > 1)
                        {
                            string name = string.Join(" ", param.Skip(1));

                            if (!_config.GlobalIgnores.Contains(name))
                            {
                                _config.GlobalIgnores.Add(name);
                                chatWindow.WriteLine($"Added \"{name}\" to global ignore list");
                            }
                            else
                            {
                                _config.GlobalIgnores.Remove(name);
                                chatWindow.WriteLine($"Removed \"{name}\" from global ignore list");
                            }

                            File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
                        }
                        else
                        {
                            chatWindow.WriteLine("Please specify a name");
                        }
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void BuddyCommand(string arg1, string[] arg2, ChatWindow window)
        {
            try
            {
                if (_settingsWindow?.IsValid == true)
                {
                    Window_Closed_helper();

                    _settingsWindow.Close();
                    _settingsWindow = null;
                    return;
                }

                _settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerAttackSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                _settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

                if (_settingsWindow.FindView("ManagerAttackInfoView", out Button infoView))
                    infoView.Clicked = HandleInfoViewClick;

                if (_settingsWindow.FindView("Enable_Disable_Button", out Button enableBtn))
                {
                    enableBtn.SetLabel(EnableString);
                    enableBtn.Clicked = Enable_Disable_Button_Clicked;
                }

                if (_settingsWindow.FindView("Taunt_Button", out Button tauntButton))
                {
                    tauntButton.SetLabel(TauntString);
                    tauntButton.Clicked = Taunt_Button_Clicked;
                }

                if (_settingsWindow.FindView("AttackRangeBox", out TextInputView attackRange))
                    attackRange.Text = _settings["AttackRange"].AsInt32().ToString();

                _settingsWindow.FindView("TauntRange", out TextView tauntText);
                _settingsWindow.FindView("TauntRangeBox", out TextInputView tauntRange);
                tauntRange.Text = _settings["TauntRange"].AsInt32().ToString();

                if (!_settings["Taunt"].AsBool())
                {
                    tauntText.Show(false, true);
                    tauntRange.Show(false, true);
                }
                else
                    tauntText.Text = "Taunt Range";

                if (_settingsWindow.FindView("SetRange", out Button setRangeBtn))
                    setRangeBtn.Clicked = SaveRangeButtonClicked;

                if (_settingsWindow.FindView("ManagerAttackHelpersView", out Button helperView))
                    helperView.Clicked = HandleHelpersViewClick;

                if (_settingsWindow.FindView("AdvancedSettingsButton", out Button settingsButton))
                    settingsButton.Clicked = Settings_Button_Clicked;

                if (_settingsWindow.FindView("Errors", out View errorView))
                    PopulateErrorView(errorView);

                if (_settingsWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";

                _settingsWindow.Show(true);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void ChannelCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (arg2 == null || arg2.Length == 0)
            {
                Chat.WriteLine($"Current IPC Channel: {_settings["IPCChannel"].AsInt32()}");
                return;
            }

            if (int.TryParse(arg2[0], out int newChannel))
            {
                if (newChannel < 1 || newChannel > 255)
                {
                    Chat.WriteLine("Invalid channel. Please enter a number between 1 and 255.");
                    return;
                }

                _settings["IPCChannel"] = newChannel;
                IPCChannel.SetChannelId(Convert.ToByte(_settings["IPCChannel"].AsInt32()));
                Chat.WriteLine($"IPC Channel set to: {_settings["IPCChannel"].AsInt32()}");
                Save();
            }
            else
            {
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
            }
        }

        #endregion

        #region Buttons Clicked

        private void Taunt_Button_Clicked(object sender, ButtonBase e)
        {
            _settings["Taunt"] = !_settings["Taunt"].AsBool();
            TauntString = _settings["Taunt"].AsBool() ? "Taunt Disable" : " Taunt Enable";

            if (_settingsWindow.FindView("Taunt_Button", out Button tauntButton))
                tauntButton.SetLabel(TauntString);

            _settingsWindow.FindView("TauntRange", out TextView tauntText);
            _settingsWindow.FindView("TauntRangeBox", out TextInputView tauntRange);

            tauntText.Show(_settings["Taunt"].AsBool(), true);
            tauntRange.Show(_settings["Taunt"].AsBool(), true);

            if (_settings["Taunt"].AsBool())
                tauntText.Text = "Taunt Range";
        }

        internal void Enable_Disable_Button_Clicked(object s, ButtonBase button)
        {
            Handle_Enable();
        }

        internal void SaveRangeButtonClicked(object sender, ButtonBase e)
        {
            if (!_settingsWindow.FindView("AttackRangeBox", out TextInputView attackRangeInput) ||
                !_settingsWindow.FindView("TauntRangeBox", out TextInputView tauntRangeInput))
                return;

            if (int.TryParse(attackRangeInput.Text, out int attackValue) && _settings["AttackRange"].AsInt32() != attackValue)
                _settings["AttackRange"] = attackValue;

            if (int.TryParse(tauntRangeInput.Text, out int tauntValue))
            {
                if (_settings["Taunt"].AsBool())
                {
                    if (_settings["TauntRange"].AsInt32() != tauntValue)
                        _settings["TauntRange"] = tauntValue;
                }
                else
                {
                    int expected = _settings["AttackRange"].AsInt32() + 1;
                    if (tauntValue != expected)
                    {
                        tauntRangeInput.Text = expected.ToString();
                        _settings["TauntRange"] = expected;
                    }
                }
            }

            Save();

            IPCChannel.Broadcast(new RangeInfoIPCMessage()
            {
                Sender = DynelManager.LocalPlayer.Identity,
                AttackRange = _settings["AttackRange"].AsInt32(),
                TauntRange = _settings["TauntRange"].AsInt32()

            });
        }

        internal void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
                return;
            }

            _infoWindow = Window.CreateFromXml("ManagerAttackInfo", PluginDirectory + "\\UI\\ManagerAttackInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _infoWindow.Show(true);
        }

        internal void HandleHelpersViewClick(object s, ButtonBase button)
        {
            if (_helperWindow?.IsValid == true)
            {
                _helperWindow.Close();
                _helperWindow = null;
                return;
            }

            _helperWindow = Window.CreateFromXml("Assist", PluginDirectory + "\\UI\\ManagerAttackHelpersView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);


            if (_helperWindow.FindView("Dropdown", out DropdownMenu menu))
            {
                for (int i = (int)_helpers.Count - 1; i >= 0; i--)
                    menu.DeleteItem((uint)i);

                foreach (var item in _helpers)
                    menu.AppendItem(item);
            }

            if (_helperWindow.FindView("ManagerAttackAddHelper", out Button addHelperView))
                addHelperView.Clicked = HandleAddHelperViewClick;

            if (_helperWindow.FindView("ManagerAttackRemoveHelper", out Button removeHelperView))
                removeHelperView.Clicked = HandleRemoveHelperViewClick;

            if (_helperWindow.FindView("ManagerAttackClearHelpers", out Button clearHelpersView))
                clearHelpersView.Clicked = HandleClearHelpersViewClick;

            _helperWindow.Show(true);
        }

        private void Settings_Button_Clicked(object sender, ButtonBase e)
        {
            if (_advSettingsWindow?.IsValid == true)
            {
                _advSettingsWindow.Close();
                _advSettingsWindow = null;
                return;
            }

            _advSettingsWindow = Window.CreateFromXml("Settings", PluginDirectory + "\\UI\\ManagerAttackAdvancedSettings.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

            if (_advSettingsWindow.FindView("KitHealthPercentageBox", out TextInputView healthBox))
                healthBox.Text = $"{_settings["KitHealthPercentageBox"].AsInt32()}";

            if (_advSettingsWindow.FindView("KitNanoPercentageBox", out TextInputView nanoBox))
                nanoBox.Text = $"{_settings["KitNanoPercentageBox"].AsInt32()}";

            if (_advSettingsWindow.FindView("Save_Button", out Button saveButton))
                saveButton.Clicked = Save_Button_Clicked;

            var instanceId = Playfield.ModelIdentity.Instance.ToString();
            
            if (_advSettingsWindow.FindView("InstanceId", out TextView pfInstance))
                pfInstance.Text = $"Playfield ID = {instanceId}";

            if (_advSettingsWindow.FindView("IgnoreInput", out TextInputView ignoreInput))
                ignoreInput.Text = string.Empty;

            if (_advSettingsWindow.FindView("IgnoreDropdown", out DropdownMenu ignoreDropdown))
            {
                if (_config.Instances.TryGetValue(instanceId, out var entry) && entry != null && entry.Ignores != null)
                {
                    for (uint i = (uint)entry.Ignores.Count; i > 0; i--)
                        ignoreDropdown.DeleteItem(i - 1);

                    foreach (var name in entry.Ignores)
                        ignoreDropdown.AppendItem(name);
                }
            }

            if (_advSettingsWindow.FindView("AddIgnore", out Button addIgnore))
                addIgnore.Clicked = AddIgnore_Clicked;

            if (_advSettingsWindow.FindView("RemoveIgnore", out Button removeIgnore))
                removeIgnore.Clicked = RemoveIgnore_Clicked;

            

            if (_advSettingsWindow.FindView("SwitchInput", out TextInputView switchInput))
                switchInput.Text = string.Empty;

            if (_advSettingsWindow.FindView("SwitchDropdown", out DropdownMenu switchDropdown))
            {
                if (_config.Instances.TryGetValue(instanceId, out var entry) && entry != null && entry.Switch != null)
                {
                    for (uint i = (uint)entry.Switch.Count; i > 0; i--)
                        switchDropdown.DeleteItem(i - 1);

                    foreach (var name in entry.Switch)
                        switchDropdown.AppendItem(name);
                }
            }

            if (_advSettingsWindow.FindView("AddSwitch", out Button addSwitch))
                addSwitch.Clicked = AddSwitch_Clicked;

            if (_advSettingsWindow.FindView("RemoveSwitch", out Button removeSwitch))
                removeSwitch.Clicked = RemoveSwitch_Clicked;


            if (_advSettingsWindow.FindView("BossInput", out TextInputView bossInput))
                bossInput.Text = string.Empty;

            if (_advSettingsWindow.FindView("BossDropdown", out DropdownMenu bossDropdown))
            {
                if (_config.Instances.TryGetValue(instanceId, out var entry) && entry != null && entry.Boss != null)
                {
                    for (uint i = (uint)entry.Boss.Count; i > 0; i--)
                        bossDropdown.DeleteItem(i - 1);

                    foreach (var name in entry.Boss)
                        bossDropdown.AppendItem(name);
                }
            }

            if (_advSettingsWindow.FindView("AddBoss", out Button addBoss))
                addBoss.Clicked = AddBoss_Clicked;

            if (_advSettingsWindow.FindView("RemoveBoss", out Button removeBoss))
                removeBoss.Clicked = RemoveBoss_Clicked;



            if (_advSettingsWindow.FindView("GlobalInput", out TextInputView globalInput))
                globalInput.Text = string.Empty;

            if (_advSettingsWindow.FindView("GlobalDropdown", out DropdownMenu globalDropdown))
            {
                if (_config.GlobalIgnores.Count > 0)
                {
                    for (uint i = (uint)_config.GlobalIgnores.Count; i > 0; i--)
                        globalDropdown.DeleteItem(i - 1);

                    foreach (var name in _config.GlobalIgnores)
                        globalDropdown.AppendItem(name);
                }
            }

            if (_advSettingsWindow.FindView("AddGlobal", out Button addGlobal))
                addGlobal.Clicked = Add_Global_Clicked;

            if (_advSettingsWindow.FindView("RemoveGlobal", out Button removeGlobal))
                removeGlobal.Clicked = Remove_Global_Clicked;

            _advSettingsWindow.Show(true);
        }

        private void Add_Global_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("GlobalInput", out TextInputView input);
            _advSettingsWindow.FindView("GlobalDropdown", out DropdownMenu dropdown);

            if (!string.IsNullOrWhiteSpace(input.Text))
            {
                if (!_config.GlobalIgnores.Contains(input.Text))
                    _config.GlobalIgnores.Add(input.Text);

                input.Text = string.Empty;
            }
            else if (Targeting.Target is Dynel target)
            {
                if (!_config.GlobalIgnores.Contains(target.Name))
                    _config.GlobalIgnores.Add(target.Name);
            }

            File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config, Formatting.Indented));

            _config = JsonConvert.DeserializeObject<InstancesConfig>(File.ReadAllText(_configPath));

            IPCChannel.Broadcast(new UpdateJsonIPCMessage { });

            for (uint i = (uint)_config.GlobalIgnores.Count; i > 0; i--)
                dropdown.DeleteItem(i - 1);

            foreach (var name in _config.GlobalIgnores)
                dropdown.AppendItem(name);
        }

        private void Remove_Global_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("GlobalDropdown", out DropdownMenu dropdown);

            var selectedIndex = dropdown.GetSelection();
            if (selectedIndex >= _config.GlobalIgnores.Count)
                return;

            _config.GlobalIgnores.RemoveAt((int)selectedIndex);

            File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config, Formatting.Indented));

            dropdown.DeleteItem(selectedIndex);

            _config = JsonConvert.DeserializeObject<InstancesConfig>(File.ReadAllText(_configPath));

            IPCChannel.Broadcast(new UpdateJsonIPCMessage { });
        }

        private void AddIgnore_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("IgnoreInput", out TextInputView ignoreInput);
            _advSettingsWindow.FindView("IgnoreDropdown", out DropdownMenu ignoreDropdown);
            AddEntry("Ignores", ignoreInput, ignoreDropdown);
        }

        private void RemoveIgnore_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("IgnoreDropdown", out DropdownMenu ignoreDropdown);
            RemoveEntry("Ignores", ignoreDropdown);
        }

        private void AddSwitch_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("SwitchInput", out TextInputView switchInput);
            _advSettingsWindow.FindView("SwitchDropdown", out DropdownMenu switchDropdown);
            AddEntry("Switch", switchInput, switchDropdown);
        }

        private void RemoveSwitch_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("SwitchDropdown", out DropdownMenu switchDropdown);
            RemoveEntry("Switch", switchDropdown);
        }

        private void AddBoss_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("BossInput", out TextInputView bossInput);
            _advSettingsWindow.FindView("BossDropdown", out DropdownMenu bossDropdown);
            AddEntry("Boss", bossInput, bossDropdown);
        }

        private void RemoveBoss_Clicked(object sender, ButtonBase e)
        {
            _advSettingsWindow.FindView("BossDropdown", out DropdownMenu bossDropdown);
            RemoveEntry("Boss", bossDropdown);
        }

        private void Save_Button_Clicked(object sender, ButtonBase e)
        {
            if (_advSettingsWindow?.IsValid == true)
            {
                if (_advSettingsWindow.FindView("KitHealthPercentageBox", out TextInputView kitHealthInput) &&
                int.TryParse(kitHealthInput.Text, out int kitHealthValue))
                {
                    if (_settings["KitHealthPercentageBox"].AsInt32() != kitHealthValue)
                        _settings["KitHealthPercentageBox"] = kitHealthValue;
                }

                if (_advSettingsWindow.FindView("KitNanoPercentageBox", out TextInputView kitNanoInput) &&
                    int.TryParse(kitNanoInput.Text, out int kitNanoValue))
                {
                    if (_settings["KitNanoPercentageBox"].AsInt32() != kitNanoValue)
                        _settings["KitNanoPercentageBox"] = kitNanoValue;
                }
                Save();
            }
        }

        private void HandleAddHelperViewClick(object s, ButtonBase button)
        {
            if (_helperWindow?.IsValid == true)
            {
                _helperWindow.FindView("HelperNameBox", out TextInputView nameInput);
                _helperWindow.FindView("Dropdown", out DropdownMenu menu);

                if (nameInput != null && !string.IsNullOrWhiteSpace(nameInput.Text))
                {
                    string entry = nameInput.Text.Trim();

                    if (!_helpers.Contains(entry))
                    {
                        _helpers.Add(entry);
                        menu.AppendItem(entry);
                    }

                    nameInput.Text = "";
                }
                else if (Targeting.Target != null)
                {
                    var name = Targeting.Target.Name;
                    _helpers.Add(name);
                    menu.AppendItem(name);
                }
            }
        }

        private void HandleRemoveHelperViewClick(object s, ButtonBase button)
        {
            if (_helperWindow?.IsValid == true &&
                _helperWindow.FindView("Dropdown", out DropdownMenu menu))
            {
                uint selectedIndex = menu.GetSelection();

                if (selectedIndex < _helpers.Count)
                {
                    _helpers.RemoveAt((int)selectedIndex);
                    menu.DeleteItem(selectedIndex);
                }
            }
        }

        private void HandleClearHelpersViewClick(object s, ButtonBase button)
        {
            if (_helperWindow.FindView("Dropdown", out DropdownMenu menu))
            {
                for (int i = (int)_helpers.Count - 1; i >= 0; i--)
                    menu.DeleteItem((uint)i);
            }

            _helpers.Clear();
        }

        #endregion

        #region Misc.

        void Handle_Enable()
        {
            Enable = !Enable;
            EnableString = Enable ? "Disable" : "Enable";

            if (_settingsWindow?.IsValid == true && _settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = Enable, Sender = DynelManager.LocalPlayer.Identity });

            if (Enable)
                Start();
            else
                Stop();

            return;
        }

        public void Debug_DrawCircle(Vector3 position, float radius, Vector3 color, int numVertices = 12)
        {
            numVertices = Math.Max(numVertices, 3);
            var vertices = new Vector3[numVertices];

            for (int i = 0; i < numVertices; i++)
            {
                vertices[i] = position + (Quaternion.AngleAxis((360f / numVertices) * i, Vector3.Up) * Vector3.Forward) * radius;

                if (i > 0)
                {
                    AOSharp.Core.Debug.DrawLine(vertices[i - 1], vertices[i], color);

                    if (i == numVertices - 1)
                    {
                        AOSharp.Core.Debug.DrawLine(vertices[0], vertices[i], color);
                    }
                }
            }
        }

        public void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        public static void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (_settingsWindow != null && _settingsWindow.IsValid && _settingsWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);
        }


        public static void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { "   at " }, StringSplitOptions.None);

                    View xmlRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                    xmlRoot.FindChild("TextLabel", out TextView labelView);
                    labelView.Text = parts[0];
                    labelView.SetColor(Color.Red);
                    errorView.AddChild(xmlRoot, true);

                    if (parts.Length > 1)
                    {
                        View methodRoot = View.CreateFromXml($"{PluginDir}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        methodRoot.FindChild("TextLabel", out TextView methodLabel);
                        methodLabel.Text = "at " + parts[1];
                        errorView.AddChild(methodRoot, true);
                    }
                }
            }
        }

        private void AddEntry(string type, TextInputView input, DropdownMenu dropdown)
        {
            var instanceId = Playfield.ModelIdentity.Instance.ToString();

            InstanceEntry entry;
            if (_config.Instances.ContainsKey(instanceId))
                entry = _config.Instances[instanceId];
            else
            {
                entry = new InstanceEntry();
                _config.Instances.Add(instanceId, entry);
            }

            List<string> list = null;
            switch (type)
            {
                case "Ignores": list = entry.Ignores; break;
                case "Switch": list = entry.Switch; break;
                case "Boss": list = entry.Boss; break;
            }

            if (list == null)
                return;

            if (!string.IsNullOrWhiteSpace(input.Text))
            {
                if (!list.Contains(input.Text))
                    list.Add(input.Text);

                input.Text = string.Empty;
            }
            else if (Targeting.Target is Dynel target)
            {
                if (!list.Contains(target.Name))
                    list.Add(target.Name);
            }

            File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config, Formatting.Indented));

            _config = JsonConvert.DeserializeObject<InstancesConfig>(File.ReadAllText(_configPath));

            IPCChannel.Broadcast(new UpdateJsonIPCMessage { });

            for (uint i = (uint)list.Count; i > 0; i--)
                dropdown.DeleteItem(i - 1);

            foreach (var name in list)
                dropdown.AppendItem(name);
        }

        private void RemoveEntry(string type, DropdownMenu dropdown)
        {
            var instanceId = Playfield.ModelIdentity.Instance.ToString();
            if (!_config.Instances.TryGetValue(instanceId, out var entry) || entry == null)
                return;

            List<string> list = null;
            switch (type)
            {
                case "Ignores": list = entry.Ignores; break;
                case "Switch": list = entry.Switch; break;
                case "Boss": list = entry.Boss; break;
            }

            if (list == null || list.Count == 0)
                return;

            var selectedIndex = dropdown.GetSelection();
            if (selectedIndex >= list.Count)
                return;

            list.RemoveAt((int)selectedIndex);

            File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config, Formatting.Indented));

            dropdown.DeleteItem(selectedIndex);

            _config = JsonConvert.DeserializeObject<InstancesConfig>(File.ReadAllText(_configPath));

            IPCChannel.Broadcast(new UpdateJsonIPCMessage { });
        }

        public class InstanceEntry
        {
            public List<string> Ignores { get; set; } = new List<string>();
            public List<string> Switch { get; set; } = new List<string>();
            public List<string> Boss { get; set; } = new List<string>();
        }

        public class InstancesConfig
        {
            public List<string> GlobalIgnores { get; set; } = new List<string>();
            public Dictionary<string, InstanceEntry> Instances { get; set; } = new Dictionary<string, InstanceEntry>();
        }

        #endregion
    }
}
