using System;
using System.Collections.Generic;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using ManagerPet.IPCMessages;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerPet
{
    public class ManagerPet : AOPluginEntry
    {
        const string PluginName = "ManagerPet";
        private string Version_Number = "2.0.0";

        protected Settings _settings;
        private static IPCChannel IPCChannel;

        Window settingsWindow;
        public static Window _infoWindow;

        private static List<Settings> _settingsToSave = new List<Settings>();
        public static string SyncPetsString;

        public override void Run()
        {
            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            _settings = new Settings(PluginName);

            _settings.AddVariable("SyncPets", true);
            SyncPetsString = _settings["SyncPets"].AsBool() ? "SyncPets Disable" : "SyncPets Enable";

            _settings.AddVariable("IPCChannel", 0);
            _settings["IPCChannel"] = 0;

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            _settingsToSave.Add(_settings);

            IPCChannel = new IPCChannel(Convert.ToByte(_settings["IPCChannel"].AsInt32()));

            Chat.RegisterCommand(PluginName, ManagerCommand);
            Chat.RegisterCommand("ManagerPetchannel", ChannelCommand);

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

            UIController.WindowDeleted -= Windowclosed;
            Network.N3MessageSent -= N3MessageSent;

            Save();

            return;
        }

        #region Chat Commands

        private void ManagerCommand(string arg1, string[] arg2, ChatWindow window)
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

                settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\ManagerPetSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                settingsWindow.Show(true);

                if (settingsWindow.FindView("ManagerPetInfoView", out Button infoView))
                    infoView.Clicked = HandleInfoViewClick;

                if (settingsWindow.FindView("SyncPets_Disable_Button", out Button SyncPetsButton))
                {
                    SyncPetsButton.SetLabel(SyncPetsString);
                    SyncPetsButton.Clicked = Sync_Pets_On_Off;
                }

                if (settingsWindow.FindView("PetAttack", out Button PetAttack))
                    PetAttack.Clicked = PetAttackClicked;

                if (settingsWindow.FindView("PetWait", out Button PetWait))
                    PetWait.Clicked = PetWaitClicked;

                if (settingsWindow.FindView("PetFollow", out Button PetFollow))
                    PetFollow.Clicked = PetFollowClicked;

                if (settingsWindow.FindView("PetWarp", out Button PetWarp))
                    PetWarp.Clicked = PetWarpClicked;

                if (settingsWindow.FindView("VersionNumber", out TextView version))
                     version.Text = $"Version {Version_Number}";

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
                Chat.WriteLine($"IPC Channel set to: {_settings["IPCChannel"].AsInt32()}");
                Save();
            }
            else
            {
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
            }
        }

        #endregion

        #region Buttion clicked

        private void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }
            else
            {
                _infoWindow = Window.CreateFromXml("Info", PluginDirectory + "\\UI\\ManagerPetInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade );
                _infoWindow.Show(true);
            }
        }

        private void Sync_Pets_On_Off(object sender, ButtonBase e)
        {
            _settings["SyncPets"] = !_settings["SyncPets"].AsBool();
            SyncPetsString = _settings["SyncPets"].AsBool() ? "SyncPets Disable" : "SyncPets Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("SyncPets_Disable_Button", out Button SyncPetsButton))
                SyncPetsButton.SetLabel(SyncPetsString);

            if (_settings["SyncPets"].AsBool())
                Chat.WriteLine("SyncPets enabled.");
            else
                Chat.WriteLine("SyncPets disabled");

            IPCChannel.Broadcast(new PetSync_On_Off_Message() { Sync_On_Off = _settings["SyncPets"].AsBool(), Sender = DynelManager.LocalPlayer.Identity });

            Save();
        }

        private void PetAttackClicked(object s, ButtonBase button)
        {
            if (Targeting.Target == null) return;
            IPCChannel.Broadcast(new PetAttackMessage() { Target = (Identity)Targeting.Target?.Identity });
        }

        private void PetWaitClicked(object s, ButtonBase button)
        {
            IPCChannel.Broadcast(new PetWaitMessage());
        }

        private void PetWarpClicked(object s, ButtonBase button)
        {
            IPCChannel.Broadcast(new PetWarpMessage());
        }

        private void PetFollowClicked(object s, ButtonBase button)
        {
            IPCChannel.Broadcast(new PetFollowMessage());
        }

        #endregion

        #region Helpers

        private void Save()
        {
            _settingsToSave.ForEach(settings => settings.Save());
        }

        #endregion
    }
}
