using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using ManagerFollow.IPCMessages;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerFollow
{
    public class ManagerFollow : AOPluginEntry
    {
        const string PluginName = "ManagerFollow";
        private string Version_Number = "2.0.0";

        private static IPCChannel IPCChannel;

        Dynel CharacterToFollow = null;
        string FollowTargetName = null;

        Window settingsWindow;
        Window _infoWindow;
        double anti_spame_delay;

        private string previousErrorMessage = string.Empty;

        protected Settings _settings;
        private static List<Settings> _settingsToSave = new List<Settings>();
        public static string EnableString;

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

                EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

                _settings.AddVariable("IPCChannel", 4);
                _settings["IPCChannel"] = 4;
                _settings.AddVariable("NavFollowDistanceSlider", 10);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                _settingsToSave.Add(_settings);

                IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

                IPCChannel.RegisterCallback((int)IPCOpcode.Follow, OnFollowMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.StartStop, OnStartStopMessage);

                Chat.RegisterCommand(PluginName, ManagerCommand);
                Chat.RegisterCommand("ManagerFollowchannel", ChannelCommand);
                Chat.RegisterCommand("toggle", HelpManagerCommand);

                Game.OnUpdate += OnUpdate;
                UIController.WindowDeleted += Windowclosed;
                Network.N3MessageSent += N3MessageSent;

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

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            _settings["Enable"] = false;
            EnableString = "Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;

            Save();
            
            return;
        }

        private void OnUpdate(object s, float deltaTime)
        {
            try
            {
                if (settingsWindow?.IsValid == true)
                {
                    settingsWindow.FindView("NavFollowDistanceSlider", out SliderView navFollowSlider);
                    if (settingsWindow.FindView("NavFollowDistanceValue", out TextView navFollowValue))
                        navFollowValue.Text = ((int)navFollowSlider.Value).ToString();
                }

                if (!_settings["Enable"].AsBool()) return;

                var localPlyer = DynelManager.LocalPlayer;

                if (CharacterToFollow == null) return;
                if (localPlyer.Position.Distance2DFrom(CharacterToFollow.Position) > _settings["NavFollowDistanceSlider"].AsInt32())
                {
                    if (Time.AONormalTime < anti_spame_delay) return;
                    MovementController.Instance.SetDestination(CharacterToFollow.Position);
                    MovementController.Instance.SetMovement(MovementAction.Update);
                    anti_spame_delay = Time.AONormalTime + 0.1;
                }
                else
                    MovementController.Instance.SetMovement(MovementAction.FullStop);

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

        private void OnFollowMessage(int sender, IPCMessage msg)
        {
            FollowMessage followMessage = (FollowMessage)msg;
            if (followMessage.Sender == DynelManager.LocalPlayer.Identity) return;
            CharacterToFollow = DynelManager.AllDynels.FirstOrDefault(d => d.Identity == followMessage.Target);
            FollowTargetName = CharacterToFollow.Name;

            _settings["NavFollowDistanceSlider"] = followMessage.Distance;

            if (settingsWindow.FindView("NavFollowDistanceSlider", out SliderView navFollowSlider))
                navFollowSlider.Value = _settings["NavFollowDistanceSlider"].AsInt32();

            if (settingsWindow.FindView("FollowCharacterName", out TextInputView followBox))
                if (FollowTargetName != null)
                    followBox.Text = $"{FollowTargetName}";

        }
        private void OnStartStopMessage(int sender, IPCMessage msg)
        {
            var startStopMessage = (StartStopIPCMessage)msg;

            if (startStopMessage.Sender == DynelManager.LocalPlayer.Identity) { return; }

            Helper_Enable();
        }

        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;

            var lineMatch = Regex.Match(ex.StackTrace ?? "", @":line (\d+)$", RegexOptions.Multiline);

            if (lineMatch.Success)
                lineNumber = int.Parse(lineMatch.Groups[1].Value);

            return lineNumber;
        }

        public static void Save()
        {
            _settingsToSave.ForEach(settings => settings.Save());
        }

        #region Chat Commands

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
                else
                {
                   
                    settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerFollowSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                    settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                    settingsWindow.Show(true);

                    if (settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                    {
                        enableButton.SetLabel(EnableString);
                        enableButton.Clicked = Enable_Disable_Button_Clicked;
                    }

                    if (settingsWindow.FindView("ManagerFollowInfoView", out Button infoView))
                        infoView.Clicked = HandleInfoViewClick;

                    if (settingsWindow.FindView("FollowCharacterName", out TextView followName))
                    {
                        if (FollowTargetName != null)
                            followName.Text = $"{FollowTargetName}";
                        else
                            followName.Text = $"None";
                    }

                    if (settingsWindow.FindView("SendButton", out Button sendButton))
                        sendButton.Clicked = Send_Button_Clicked;

                    if (settingsWindow.FindView("NavFollowDistanceSlider", out SliderView navFollowSlider))
                        navFollowSlider.Value = _settings["NavFollowDistanceSlider"].AsInt32();

                    if (settingsWindow.FindView("SaveButton", out Button saveButton))
                        saveButton.Clicked = Save_Button_Clicked;

                    if (settingsWindow.FindView("VersionNumber", out TextView version))
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

        private void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }
            else
            {
                _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerFollowInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade );

                _infoWindow.Show(true);
            }
        }
        private void Send_Button_Clicked(object sender, ButtonBase e)
        {
            if (Targeting.Target != null)
            {
                settingsWindow.FindView("NavFollowDistanceSlider", out SliderView navFollowSlider);
                IPCChannel.Broadcast(new FollowMessage() { Target = Targeting.Target.Identity, Distance = (int)navFollowSlider.Value, Sender = DynelManager.LocalPlayer.Identity });
            }
            else if (CharacterToFollow != null)
                IPCChannel.Broadcast(new FollowMessage() { Target = CharacterToFollow.Identity, Distance = _settings["NavFollowDistanceSlider"].AsInt32(), Sender = DynelManager.LocalPlayer.Identity });
            else
            {
                Chat.WriteLine("Target a character first!");
            }
        }

        private void Save_Button_Clicked(object sender, ButtonBase e)
        {
            var target = Targeting.Target;

            if (target == null)
                Chat.WriteLine("Target a character first.");
            else
            {
                if (target.Identity != DynelManager.LocalPlayer.Identity)
                {
                    if (settingsWindow.FindView("NavFollowDistanceSlider", out SliderView navFollowSlider))
                        _settings["NavFollowDistanceSlider"] = (int)navFollowSlider.Value;

                    settingsWindow.FindView("FollowCharacterName", out TextView followName);

                    CharacterToFollow = Targeting.Target;
                    FollowTargetName = Targeting.Target.Name;
                    followName.Text = $"{Targeting.Target.Name}";
                }
                else
                    Chat.WriteLine("You can not follow yourself, select another target.");
            }

            Save();
        }

        private void Enable_Disable_Button_Clicked(object sender, ButtonBase e)
        {
            IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = _settings["Enable"].AsBool() });
            Helper_Enable();
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
        private void HelpManagerCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length < 1)
            {
                IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = _settings["Enable"].AsBool() });
                Helper_Enable();
            }
        }

        private void Helper_Enable()
        {
            _settings["Enable"] = !_settings["Enable"].AsBool();
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Save();
        }

        #endregion
    }
}
