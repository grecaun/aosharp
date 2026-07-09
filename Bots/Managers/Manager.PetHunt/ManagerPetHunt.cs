using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using ManagerPetHunt.IPCMessages;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerPetHunt
{
    public class ManagerPetHunt : AOPluginEntry
    {
        const string PluginName = "ManagerPetHunt";
        private string Version_Number = "2.0.0";

        public static StateMachine _stateMachine;
        public static IPCChannel IPCChannel { get; private set; }

        public static Settings _settings;
        private List<Settings> settingsToSave = new List<Settings>();

        Window _settingsWindow;
        Window _infoWindow;

        public static string EnableString = "Enable";

        public static string previousErrorMessage = string.Empty;

        public static List<SimpleChar> _mob = new List<SimpleChar>();
        public static List<SimpleChar> _bossMob = new List<SimpleChar>();
        public static List<SimpleChar> _switchMob = new List<SimpleChar>();

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                _settings = new Settings(PluginName);

                _settings.AddVariable("Enable", false);
                _settings["Enable"] = false;

                _settings.AddVariable("HuntRange", 60);

                _settings.AddVariable("IPCChannel", 48);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                settingsToSave.Add(_settings);

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, OnStartStopMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.RangeInfo, OnRangeInfoMessage);

                Chat.RegisterCommand("enable", EnableCommand);
                Chat.RegisterCommand(PluginName, BuddyCommand);
                Chat.RegisterCommand("buddychannel", ChannelCommand);

                _stateMachine = new StateMachine(new IdleState());

                Network.N3MessageSent += N3MessageSent;
                UIController.WindowDeleted += Windowclosed;

                Chat.WriteLine($"{PluginName} Loaded!");
                Chat.WriteLine($"/{PluginName} for UI.");
                Chat.WriteLine($"/macro {PluginName} /{PluginName}");
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

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            UIController.WindowDeleted -= Windowclosed;
            _settingsWindow?.Close();
            _infoWindow?.Close();
        }

        private void OnStartStopMessage(int sender, IPCMessage msg)
        {
            if (!(msg is StartStopIPCMessage startStopMessage)) return;
            if (startStopMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            _settings["Enable"] = startStopMessage.IsStarting;

            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (_settingsWindow?.IsValid == true && _settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            if (_settings["Enable"].AsBool())
                Start();
            else
                Stop();
        }

        private void OnRangeInfoMessage(int sender, IPCMessage msg)
        {
            if (!(msg is RangeInfoIPCMessage rangeInfoMessage)) return;
            if (rangeInfoMessage.Sender == DynelManager.LocalPlayer.Identity) return;

            _settings["HuntRange"] = rangeInfoMessage.HuntRange;

            if (_settingsWindow?.IsValid == true)
            {
                if (_settingsWindow.FindView("HuntRangeBox", out TextInputView attackRangeInput))
                    attackRangeInput.Text = _settings["HuntRange"].ToString();
            }
            Save();
        }


        private void OnUpdate(object s, float deltaTime)
        {

            if (Game.IsZoning)
            {
                _settings["Enable"] = false;
                return;
            }

            ScanningDefault();
            _stateMachine.Tick();
        }

        private void ScanningDefault()
        {
            var localPlayer = DynelManager.LocalPlayer;
            var player = DynelManager.Players;

            var pets = DynelManager.LocalPlayer.Pets;

            _bossMob = DynelManager.NPCs
                       .Where(c => c.DistanceFrom(localPlayer) <= _settings["HuntRange"].AsInt32()
                           && !Ignores._ignores.Contains(c.Name) && !c.IsPlayer
                           && c.Health > 0 && !c.IsPet
                           && !c.Buffs.Contains(253953) && !c.Buffs.Contains(205607)
                           && c.MaxHealth >= 1000000)
                       .OrderBy(c => c.Position.DistanceFrom(localPlayer.Position))
                       .OrderByDescending(c => c.Name == "Field Support  - Cha'Khaz")
                       .OrderByDescending(c => c.Name == "Ground Chief Aune")


                       .ToList();

            _switchMob = DynelManager.NPCs
               .Where(c => c.DistanceFrom(localPlayer) <= _settings["HuntRange"].AsInt32()
                   && !Ignores._ignores.Contains(c.Name) && !c.IsPlayer
                   && c.Name != "Zix" && !c.Name.Contains("sapling")
                   && c.Health > 0 && c.MaxHealth < 1000000 && !c.IsPet
                   && (c.Name == "Hand of the Colonel"
                  || c.Name == "Hacker'Uri"
                  || c.Name == "The Sacrifice"
                  || c.Name == "Drone Harvester - Jaax'Sinuh"
                  || c.Name == "Support Sentry - Ilari'Uri"
                  || c.Name == "Alien Coccoon"
                  || c.Name == "Alien Cocoon"
                  || c.Name == "Stasis Containment Field"))
               .OrderBy(c => c.Position.DistanceFrom(localPlayer.Position))
               .OrderBy(c => c.HealthPercent)
               .OrderByDescending(c => c.Name == "Drone Harvester - Jaax'Sinuh")
               .OrderByDescending(c => c.Name == "Lost Thought")
               .OrderByDescending(c => c.Name == "Support Sentry - Ilari'Uri")
               .OrderByDescending(c => c.Name == "Alien Cocoon")
               .OrderByDescending(c => c.Name == "Alien Coccoon" && c.MaxHealth < 40001)
               .ToList();

            _mob = DynelManager.Characters
                .Where(c => c.DistanceFrom(localPlayer) <= _settings["HuntRange"].AsInt32()
                    && !Ignores._ignores.Contains(c.Name) && !c.IsPlayer
                    && c.Name != "Zix" && !c.Name.Contains("sapling") && c.Health > 0
                    && c.MaxHealth < 1000000 && !c.IsPet
                    && (!c.IsPet || c.Name == "Drop Trooper - Ilari'Ra"))
                .OrderBy(c => c.Position.DistanceFrom(localPlayer.Position))
                .OrderBy(c => c.HealthPercent)
                .OrderByDescending(c => c.Name == "Drone Harvester - Jaax'Sinuh")
                .OrderByDescending(c => c.Name == "Support Sentry - Ilari'Uri")
                .OrderByDescending(c => c.Name == "Alien Cocoon")
                .OrderByDescending(c => c.Name == "Alien Coccoon" && c.MaxHealth < 40001)
                .OrderByDescending(c => c.Name == "Masked Operator")
                .OrderByDescending(c => c.Name == "Masked Technician")
                .OrderByDescending(c => c.Name == "Masked Engineer")
                .OrderByDescending(c => c.Name == "Masked Superior Commando")
                .OrderByDescending(c => c.Name == "The Sacrifice")
                .OrderByDescending(c => c.Name == "Hacker'Uri")
                .OrderByDescending(c => c.Name == "Hand of the Colonel")
                .OrderByDescending(c => c.Name == "Ground Chief Aune")
                .ToList();
        }


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

                            if (!Ignores._ignores.Contains(name))
                            {
                                Ignores._ignores.Add(name);
                                chatWindow.WriteLine($"Added \"{name}\" to ignored mob list");
                            }
                            else
                            {
                                Ignores._ignores.Remove(name);
                                chatWindow.WriteLine($"Removed \"{name}\" from ignored mob list");
                            }
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
                var errorMessage = "An error occurred on line " + GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != previousErrorMessage)
                {
                    chatWindow.WriteLine(errorMessage);
                    chatWindow.WriteLine("Stack Trace: " + ex.StackTrace);
                    previousErrorMessage = errorMessage;
                }
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
                else
                {
                    _settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerPetHuntSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                    _settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                    _settingsWindow.Show(true);

                    if (_settingsWindow.FindView("ManagerPetHuntInfoView", out Button infoView))
                        infoView.Clicked = HandleInfoViewClick;

                    if (_settingsWindow.FindView("Enable_Disable_Button", out Button enableBtn))
                    {
                        enableBtn.SetLabel(EnableString);
                        enableBtn.Clicked = Enable_Disable_Button_Clicked;
                    }

                    if (_settingsWindow.FindView("HuntRangeBox", out TextInputView attackRange))
                        attackRange.Text = _settings["HuntRange"].AsInt32().ToString();

                    if (_settingsWindow.FindView("SetRange", out Button setRangeBtn))
                        setRangeBtn.Clicked = SaveRangeButtonClicked;

                    if (_settingsWindow.FindView("VersionNumber", out TextView version))
                         version.Text = $"Version {Version_Number}";
                }
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

        #region buttons

        internal void Enable_Disable_Button_Clicked(object s, ButtonBase button)
        {
            Handle_Enable();
        }

        internal void SaveRangeButtonClicked(object sender, ButtonBase e)
        {
            if (!_settingsWindow.FindView("HuntRangeBox", out TextInputView HuntRangeBoxInput))
                return;

            if (int.TryParse(HuntRangeBoxInput.Text, out int huntValue) && _settings["HuntRange"].AsInt32() != huntValue)
                _settings["HuntRange"] = huntValue;

            Save();

            IPCChannel.Broadcast(new RangeInfoIPCMessage()
            {
                Sender = DynelManager.LocalPlayer.Identity,
                HuntRange = _settings["HuntRange"].AsInt32(),

            });
        }

        internal void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }
            else
            {
                _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerPetHuntInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade );
                _infoWindow.Show(true);
            }
        }

        #endregion

        #region Helpers

        void Handle_Enable()
        {
            _settings["Enable"] = !_settings["Enable"].AsBool();
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (_settingsWindow?.IsValid == true && _settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = _settings["Enable"].AsBool(), Sender = DynelManager.LocalPlayer.Identity });

            if (_settings["Enable"].AsBool())
                Start();
            else
                Stop();

            return;
        }

        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;

            var lineMatch = Regex.Match(ex.StackTrace ?? "", @":line (\d+)$", RegexOptions.Multiline);

            if (lineMatch.Success)
                lineNumber = int.Parse(lineMatch.Groups[1].Value);

            return lineNumber;
        }
        public void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
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

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            _settings["Enable"] = false;
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

        private void Start()
        {

            Chat.WriteLine("ManagerPetHunt enabled.");

            Game.OnUpdate += OnUpdate;
        }

        private void Stop()
        {

            Chat.WriteLine("ManagerPetHunt disabled.");
            Game.OnUpdate -= OnUpdate;

            if (!(_stateMachine.CurrentState is IdleState))
                _stateMachine.SetState(new IdleState());

        }

        #endregion
    }
}
