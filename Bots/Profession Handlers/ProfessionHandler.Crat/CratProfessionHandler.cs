using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using ProfessionHandler.Generic;
using Shared;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ProfessionHandler.Bureaucrat
{
    public class CratProfessionHandler : GenericProfessionHandler
    {
        private static string PluginDirectory;

        public CratProfessionHandler(string pluginDir) : base(pluginDir)
        {
            try
            {
                PluginDirectory = pluginDir;

                #region IPC Register Call back

                IPCChannel.RegisterCallback((int)IPCOpcode.RemainingNCU, OnRemainingNCUMessage);
                IPCChannel.RegisterCallback((int)IPCOpcode.SettingsUpdate, Received_Settings_Message);
                IPCChannel.RegisterCallback((int)IPCOpcode.ClearBuffs, OnClearBuffs);

                IPCChannel.RegisterCallback((int)IPCOpcode.AOE, Recieved_AOE_Message);
                IPCChannel.RegisterCallback((int)IPCOpcode.Holds, Recieved_Holds_Message);
                IPCChannel.RegisterCallback((int)IPCOpcode.RootSnare, Recieved_RootSnare_Message);
                IPCChannel.RegisterCallback((int)IPCOpcode.Specials, Recieved_Specials_Message);

                IPCChannel.RegisterCallback(2344, OnPetAttack);
                IPCChannel.RegisterCallback(2345, OnPetWait);
                IPCChannel.RegisterCallback(2346, OnPetWarp);
                IPCChannel.RegisterCallback(2347, OnPetFollow);
                IPCChannel.RegisterCallback(2348, SyncPetsMessageReceived);

                #endregion

                #region Register Chat Command

                Chat.RegisterCommand("handler", HandlerCommand);
               

                #endregion

                Network.N3MessageReceived += N3MessageReceived;

                Chat.WriteLine("Crat Profession Handler Loaded!");
                Chat.WriteLine("/handler to open or close UI");
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        #region IPC Message Received

        public void OnRemainingNCUMessage(int sender, IPCMessage msg)
        {
            if (!(msg is RemainingNCUMessage remainingNCUMessage)) return;
            RemainingNCU[remainingNCUMessage.Character] = remainingNCUMessage.RemainingNCU;
        }

        public void Received_Settings_Message(int arg1, IPCMessage message)
        {
            if (!(message is SettingsUpdateMessage SettingMessage)) return;

            if (_settings["Buffing"].AsBool() != SettingMessage.Buffing)
                _settings["Buffing"] = SettingMessage.Buffing;

            if (_settings["Composites"].AsBool() != SettingMessage.Comps)
                _settings["Composites"] = SettingMessage.Comps;

            if (_settings["WaitForRez"].AsBool() != SettingMessage.Wait_For_Rez)
                _settings["WaitForRez"] = SettingMessage.Wait_For_Rez;

            Save();
        }

        public void OnClearBuffs(int sender, IPCMessage msg)
        {
            switch (((ClearBuffsMessage)msg).Type)
            {
                case 0:
                    CancelLocalBuffs();
                    break;
                case 1:
                    CancelAllBuffs();
                    break;
            }
        }

        private void Recieved_AOE_Message(int arg1, IPCMessage message)
        {
            if (!(message is AOEMessage AOEMessage)) return;

            _settings["AOE"] = AOEMessage.AOEBool;

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("BroadcastAOE", out Button aoeButton))
            {
                aoeButton.SetStateColors();
                aoeButton.SetLabelColor(_settings["AOE"].AsBool() ? Color.Green : Color.BrightRed);
                aoeButton.SetLabel(_settings["AOE"].AsBool() ? "AOE Disable" : "AOE Enable");
            }
        }

        private void Recieved_Holds_Message(int arg1, IPCMessage message)
        {
            if (!(message is HoldsMessage holdsessage)) return;

            _settings["Roots/Snares/Stuns"] = holdsessage.HoldsBool;

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("BroadcastHold", out Button holdButton))
            {
                holdButton.SetStateColors();
                holdButton.SetLabelColor(_settings["Roots/Snares/Stuns"].AsBool() ? Color.Green : Color.BrightRed);
                holdButton.SetLabel(_settings["Roots/Snares/Stuns"].AsBool() ? "Holds Disable" : "Holds Enable");
            }
        }

        private void Recieved_RootSnare_Message(int arg1, IPCMessage message)
        {
            if (!(message is RootSnareMessage rootSnareMessage)) return;
            if (rootSnareMessage.Target == null) return;
            var target = DynelManager.Characters.Where(c => c.Identity == rootSnareMessage.Target).FirstOrDefault();
            if (target == null) return;
            var spell = Spell.List.Where(x => new[] { NanoLine.Root, NanoLine.Snare }.Contains(x.Nanoline)).OrderByStackingOrder().FirstOrDefault();
            if (spell == null) return;
            spell.Cast(target, true);
        }

        private void Recieved_Specials_Message(int arg1, IPCMessage message)
        {
            if (!(message is SpecialsMessage specialsMessage)) return;

            _settings["Specials"] = specialsMessage.Specials;

            if (_mainWindow?.IsValid == true && _mainWindow.FindView("BroadcastSpecials", out Button specialsButton))
            {
                specialsButton.SetStateColors();
                specialsButton.SetLabel(_settings["Specials"].AsBool() ? "Specials Disable" : "Specials Enable");
                specialsButton.SetLabelColor(_settings["Specials"].AsBool() ? Color.Green : Color.BrightRed);
            }
        }

        private void SyncPetsMessageReceived(int sender, IPCMessage msg)
        {
            Chat.WriteLine("SyncPetsMessageReceived");
            if (!(msg is PetSync_On_Off_Message petSync)) return;
            if (petSync.Sender == DynelManager.LocalPlayer.Identity) return;
            _settings["SyncPets"] = petSync.Sync_On_Off;
            Sync_Pets_Toggle();
        }

        public void OnPetAttack(int sender, IPCMessage msg)
        {
            PetAttackMessage attackMsg = (PetAttackMessage)msg;
            DynelManager.LocalPlayer.Pets.Attack(attackMsg.Target);
        }

        private void OnPetWait(int sender, IPCMessage msg)
        {
            Send_Pets_Wait();
        }

        private void OnPetWarp(int sender, IPCMessage msg)
        {
            Warp_Pets();
        }

        private void OnPetFollow(int sender, IPCMessage msg)
        {
            Send_Pets_Follow();
        }

        #endregion

        #region Chat Commands

        private void HandlerCommand(string arg1, string[] arg2, ChatWindow window)
        {
            if (_mainWindow?.IsValid == true)
            {
                Window_Closed_helper();

                _mainWindow.Close();
                _mainWindow = null;
                return;
            }

            _mainWindow = Window.CreateFromXml("PH Crat", PluginDirectory + "\\UI\\BureaucratSettingsView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

            if (_mainWindow.FindView("InfoView", out Button infoButton))
                infoButton.Clicked = (s, b) => _infoWindow = Generic_Button_Clicked(_infoWindow, "Info", $"{PluginDirectory}\\UI\\HandlerMainWindow\\HandlerInfoWindow.xml");

            if (_mainWindow.FindView("PetSyncButton", out Button SyncPetsButton))
            {
                SyncPetsButton.SetLabel(SyncPetsString);
                SyncPetsButton.Clicked = Sync_Pets_On_Off;
            }

            if (_mainWindow.FindView("BroadcastAOE", out Button aoeButton))
            {
                aoeButton.SetStateColors();
                aoeButton.SetLabel(_settings["AOE"].AsBool() ? "AOE Disable" : "AOE Enable");
                aoeButton.SetLabelColor(_settings["AOE"].AsBool() ? Color.Green : Color.BrightRed);
                aoeButton.Clicked = AOE_Button_Clicked;
            }

            if (_mainWindow.FindView("BroadcastHold", out Button holdButton))
            {
                holdButton.SetStateColors();
                holdButton.SetLabel(_settings["Roots/Snares/Stuns"].AsBool() ? "Holds Disable" : "Holds Enable");
                holdButton.SetLabelColor(_settings["Roots/Snares/Stuns"].AsBool() ? Color.Green : Color.BrightRed);
                holdButton.Clicked = Hold_Button_Clicked;
            }

            if (_mainWindow.FindView("BroadcastRoot", out Button rootButton))
                rootButton.Clicked = Send_Target_Clicked;

            if (_mainWindow.FindView("BroadcastSpecials", out Button specialsButton))
            {
                specialsButton.SetStateColors();
                specialsButton.SetLabel(_settings["Specials"].AsBool() ? "Specials Disable" : "Specials Enable");
                specialsButton.SetLabelColor(_settings["Specials"].AsBool() ? Color.Green : Color.BrightRed);
                specialsButton.Clicked = Specials_Button_Clicked;
            }

            if (_mainWindow.FindView("ReserveNCU", out TextInputView ncus))
                ncus.Text = _settings["Reserve NCU"].AsInt32().ToString();

            if (_mainWindow.FindView("SaveNCUs", out Button saveNcus))
                saveNcus.Clicked = Save_Ncus_Button_Clicked;

            if (_mainWindow.FindView("MAX_CONCURRENT_PERKS", out TextInputView perk))
                perk.Text = _settings["MAX_CONCURRENT_PERKS"].AsInt32().ToString();

            if (_mainWindow.FindView("SavePerks", out Button savePerks))
                savePerks.Clicked = Save_Perks_Button_Clicked;

            if (_mainWindow.FindView("BroadcastGlobal", out Button broadcastButton))
                broadcastButton.Clicked = Broadcast_Button_Clicked;

            if (_mainWindow.FindView("DynamicButtons", out View dynamicButtonsView))
                BuildDynamicButtons(dynamicButtonsView);

            if (_mainWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);

            if (_mainWindow.FindView("VersionNumber", out TextView version))
                version.Text = $"Version {_settings["Version_Number"].AsFloat()}";

            _mainWindow.Show(true);
        }

        #endregion

        #region Events

        protected override void OnUpdate(float deltaTime)
        {
            try
            {
                if (Game.IsZoning) return;

                base.OnUpdate(deltaTime);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void N3MessageReceived(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var characterAction = (CharacterActionMessage)e;
            var lp = DynelManager.LocalPlayer;
            if (characterAction.Identity != lp.Identity) return;

            RemainingNCU[lp.Identity] = lp.RemainingNCU;

            IPCChannel.Broadcast(new RemainingNCUMessage()
            {
                Character = lp.Identity,
                RemainingNCU = lp.RemainingNCU - _settings["Reserve NCU"].AsInt32()
            });
        }

        #endregion

    }
}
