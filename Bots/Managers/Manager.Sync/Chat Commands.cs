using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using ManagerSync.IPCMessages;
using static ManagerSync.ManagerSync;

namespace ManagerSync
{
    internal static class Chat_Commands
    {
        #region Chat Commands
        public static void ManagerCommand(string arg1, string[] arg2, ChatWindow window)
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

                settingsWindow = Window.CreateFromXml(PluginName, PluginDir + "\\UI\\ManagerSyncSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                settingsWindow.Show(true);

                if (settingsWindow.FindView("ManagerSyncInfoView", out Button infoView))
                    infoView.Clicked = HandleInfoViewClick;

                if (settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                {
                    enableButton.SetLabel(EnableString);
                    enableButton.Clicked = Enable_Disable_Button_Clicked;
                }

                if (settingsWindow.FindView("OutsideTeamInvites", out Button invitesButton))
                {
                    invitesButton.SetLabel($"Outside invites is {(_settings["OutsideTeamInvites"].AsBool() ? "on" : "off")}");
                    invitesButton.Clicked = Outside_Team_Invites;
                }

                if (settingsWindow.FindView("LogAllOut", out Button logOut))
                    logOut.Clicked = HandleLogAllOutClicked;

                if (settingsWindow.FindView("TeamButton", out Button teamButton))
                {
                    teamButton.SetLabel(TeamButtonState);
                    teamButton.Clicked = TeamButtonClicked;
                }

                if (settingsWindow.FindView("BroadcastSettingsView", out Button settingsButton))
                    settingsButton.Clicked = UISettingsButtonClicked;

                if (settingsWindow.FindView("SpreadOut", out Button SpreadButton))
                    SpreadButton.Clicked = HandleSpreadButtonClicked;

                if (settingsWindow.FindView("Errors", out View errorView))
                    PopulateErrorView(errorView);

                if (settingsWindow.FindView("VersionNumber", out TextView version))
                    version.Text = Version_Number;
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public static void ChannelCommand(string arg1, string[] arg2, ChatWindow window)
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
                _IPCChannel.SetChannelId(Convert.ToByte(_settings["IPCChannel"].AsInt32()));
                Chat.WriteLine($"IPC Channel set to: {_settings["IPCChannel"].AsInt32()}");
                Save();
            }
            else
            {
                Chat.WriteLine("Invalid input. Please enter a number between 1 and 255.");
            }
        }
        public static void ManagerSyncCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (param.Length < 1)
            {
                Helper_Enable();
            }
        }

        public static void Send_Single_Logout(string arg1, string[] arg2, ChatWindow window)
        {
            if (arg2 == null || arg2.Length == 0)
            {
                BroadcastAndLogOut();
                return;
            }

            var name = arg2[0];
            var target = DynelManager.Players.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (target == null)
            {
                Chat.WriteLine($"Player \"{name}\" not found.");
                return;
            }

            _IPCChannel.Broadcast(new LogOutIPCMessage
            {
                Sender = DynelManager.LocalPlayer.Identity,
                Target = target.Identity,
            });

            Chat.WriteLine($"Logout signal sent to {target.Name}.");
        }

        public static void FormCommand(string command, string[] param, ChatWindow chatWindow)
        {
            ToggleTeam();
        }

        internal static void FormRaidCommand(string arg1, string[] arg2, ChatWindow window)
        {
            TeamSender = DynelManager.LocalPlayer.Identity;

            _IPCChannel.Broadcast(new LocalTeamMessage
            {
                Sender = TeamSender,
                TeamAction = 5,
                Receiver = Identity.None,
            });
        }

        public static void RaidCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (Team.IsLeader)
            {
                if (!Team.IsRaid)
                    Team.ConvertToRaid();
            }
            else
            {
                Chat.WriteLine("Needs to be used from leader.");
            }
        }

        public static void ReformCommand(string command, string[] param, ChatWindow chatWindow)
        {
            if (Team.IsInTeam)
            {
                _IPCChannel.Broadcast(new LocalTeamMessage
                {
                    Sender = TeamSender,
                    TeamAction = 2,
                    Receiver = Identity.None
                });

                _IPCChannel.Broadcast(new LocalTeamMessage
                {
                    Sender = TeamSender,
                    TeamAction = 4,
                    Receiver = Identity.None
                });

                _pendingReform = true;
            }
            else
                ToggleTeam();
        }

        internal static void LFT(string arg1, string[] arg2, ChatWindow window)
        {
            string text = "";

            if (arg2 != null && arg2.Length > 0)
                text = string.Join(" ", arg2);

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);
            if (bytes.Length > 16)
                System.Array.Resize(ref bytes, 16);

            byte length = (byte)bytes.Length;
            int p0 = 0;
            int p1 = 0;
            int p2 = 0;
            int p3 = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                int seg = i / 4;
                int shift = (i % 4) * 8;

                if (seg == 0)
                    p0 |= bytes[i] << shift;
                else if (seg == 1)
                    p1 |= bytes[i] << shift;
                else if (seg == 2)
                    p2 |= bytes[i] << shift;
                else if (seg == 3)
                    p3 |= bytes[i] << shift;
            }

            _IPCChannel.Broadcast(new LFTMessage
            {
                Sender = DynelManager.LocalPlayer.Identity,
                TextLength = length,
                TextPart0 = p0,
                TextPart1 = p1,
                TextPart2 = p2,
                TextPart3 = p3
            });

            if (Team.IsInTeam)
                Team.Leave();

            _settings["OutsideTeamInvites"] = true;

            if (settingsWindow?.IsValid == true)
            {
                if (settingsWindow.FindView("OutsideTeamInvites", out Button invitesButton))
                    invitesButton.SetLabel($"Outside invites is {(_settings["OutsideTeamInvites"].AsBool() ? "on" : "off")}");
            }

            LookingForTeam.Join(text);
        }

        #endregion

        #region Buttons Clicked

        public static void Enable_Disable_Button_Clicked(object sender, ButtonBase e)
        {
            Helper_Enable();
        }

        private static void Outside_Team_Invites(object sender, ButtonBase e)
        {
            _settings["OutsideTeamInvites"] = !_settings["OutsideTeamInvites"].AsBool();

            if (settingsWindow?.IsValid == true)
            {
                if (settingsWindow.FindView("OutsideTeamInvites", out Button invitesButton))
                    invitesButton.SetLabel($"Outside invites is {(_settings["OutsideTeamInvites"].AsBool() ? "on" : "off")}");
            }

            _IPCChannel.Broadcast(new OutsideInvitesIPCMessage { Invites = _settings["OutsideTeamInvites"].AsBool() });
        }

        private static void HandleLogAllOutClicked(object sender, ButtonBase e)
        {
            BroadcastAndLogOut();
        }

        public static void TeamButtonClicked(object sender, ButtonBase e)
        {
            ToggleTeam();
        }

        public static void HandleSpreadButtonClicked(object sender, ButtonBase e)
        {
            SpreadOut();
        }

        public static void HandleInfoViewClick(object s, ButtonBase button)
        {
            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }
            else
            {
                _infoWindow = Window.CreateFromXml("Info", PluginDir + "\\UI\\ManagerSyncInfoView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale);
                _infoWindow.Show(true);
            }
        }

        public static void UISettingsButtonClicked(object s, ButtonBase button)
        {
            Broadcast_Settings();
            Save();
        }

        #endregion

        #region Helpers

        public static void BroadcastAndLogOut()
        {
            _IPCChannel.Broadcast(new LogOutIPCMessage
            {
                Sender = DynelManager.LocalPlayer.Identity,
                Target = Identity.None,
            });

            Handle_Log_Out();
        }

        public static void ToggleTeam()
        {
            int action = Team.IsInTeam ? 2 : 0;

            TeamSender = DynelManager.LocalPlayer.Identity;

            _IPCChannel.Broadcast(new LocalTeamMessage
            {
                Sender = DynelManager.LocalPlayer.Identity,
                TeamAction = action,
                Receiver = Identity.None,
            });
        }

        #endregion
    }
}
