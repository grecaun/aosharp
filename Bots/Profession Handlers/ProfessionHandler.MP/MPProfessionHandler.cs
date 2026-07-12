using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using ProfessionHandler.Generic;
using Shared;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ProfessionHandler.Metaphysicist
{
    public class MPProfessionHandler : GenericProfessionHandler
    {
        private static string PluginDirectory;

        public static double weaponDelay;

        HealPetAction CurrentHealPetAction = new HealPetAction();

        public MPProfessionHandler(string pluginDir) : base(pluginDir)
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

                #region Add UI Buttons

                _buttonDefinitions.Add(("Weapons", (s, b) => _weaponWindow = Generic_Button_Clicked(_weaponWindow, "Weapons", $"{PluginDirectory}\\UI\\MPWeaponView.xml")));

                #endregion

                Network.N3MessageReceived += N3MessageReceived;

                Chat.WriteLine("MP Profession Handler Loaded!");
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
            var spell = Spell.List.Where(x => new[] { NanoLine.Root, NanoLine.Snare, NanoLine.Mezz }.Contains(x.Nanoline)).OrderByStackingOrder().FirstOrDefault();
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

            _mainWindow = Window.CreateFromXml("PH MP", PluginDirectory + "\\UI\\MPSettingsView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _mainWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

            if (_mainWindow.FindView("InfoView", out Button infoButton))
                infoButton.Clicked = (s, b) => _infoWindow = Generic_Button_Clicked(_infoWindow, "Info", $"{PluginDirectory}\\UI\\HandlerMainWindow\\HandlerInfoWindow.xml");

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

            if (_mainWindow.FindView("PetSyncButton", out Button SyncPetsButton))
            {
                SyncPetsButton.SetLabel(SyncPetsString);
                SyncPetsButton.Clicked = Sync_Pets_On_Off;
            }

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
                if (Game.IsZoning) { return; }


                if (CanLookupPetsAfterZone())
                {
                    AssignTargetToHealPet();
                    HandleMezzPet();
                }

                if (_settings["SummonedWeaponSelection"].AsInt32() != 0)
                {
                    if (Time.AONormalTime > weaponCheckDelay && !DynelManager.LocalPlayer.IsAttacking && !Spell.HasPendingCast && Spell.List.Any(s => s.IsReady))
                    {
                        foreach (Item weapon in Inventory.Items)
                        {
                            if (allWeaponNames.Contains(weapon.Name) || allShieldNames.Contains(weapon.Name))
                            {
                                List<EquipSlot> slot = weapon.EquipSlots;

                                if (weapon.Slot.Type != IdentityType.WeaponPage)
                                {
                                    if (Time.AONormalTime > weaponDelay + 10)
                                    {
                                        foreach (EquipSlot equipSlot in weapon.EquipSlots)
                                        {
                                            weapon.Equip(equipSlot);
                                            break;
                                        }
                                        weaponDelay = Time.AONormalTime;
                                    }
                                }
                            }
                        }
                        weaponCheckDelay = Time.AONormalTime + 10;
                    }
                }

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

        #region Misc

        enum HealPetAction { SendFollow, Following, SendHeal, Healing }

        private void AssignTargetToHealPet()
        {
            if (_settings["PetHealingOption"].AsInt32() == 0) { return; }
            if (_settings["PetHealingValue"].AsInt32() == 0) { return; }

            var healPet = DynelManager.LocalPlayer.Pets.FirstOrDefault(pet => pet.Type == PetType.Heal && pet.Character.Nano >= 1);

            if (healPet == null) { return; }

            var localPlayer = DynelManager.LocalPlayer;
            var dyingPet = localPlayer.Pets.Where(pet => pet.Character != null && pet.Character.Health > 0 && pet.Character.HealthPercent <= _settings["PetHealingValue"].AsInt32()).OrderBy(pet => pet.Character.HealthPercent).FirstOrDefault();

            switch (_settings["PetHealingOption"].AsInt32())
            {
                case 1:
                    if (localPlayer.HealthPercent <= _settings["PetHealingValue"].AsInt32())
                        HealTarget = localPlayer.Identity;
                    else if (dyingPet != null)
                        HealTarget = dyingPet.Character.Identity;
                    else
                        HealTarget = Identity.None;
                    break;
                case 2:
                    if (Team.IsInTeam)
                    {
                        var member = Team.Members.Where(c => c.Character != null && c.Character.Health > 0 && !c.Character.Buffs.Contains(SpellID.PVPEnabled) && c.Character.HealthPercent <= _settings["PetHealingValue"].AsInt32()).OrderBy(c => c.Character.HealthPercent).FirstOrDefault()?.Character;

                        if (member != null)
                        {
                            HealTarget = member.Identity;
                        }
                        else if (dyingPet != null)
                            HealTarget = dyingPet.Character.Identity;
                        else
                            HealTarget = Identity.None;
                    }
                    else if (localPlayer.HealthPercent <= _settings["PetHealingValue"].AsInt32())
                        HealTarget = localPlayer.Identity;
                    else if (dyingPet != null)
                        HealTarget = dyingPet.Character.Identity;
                    else
                        HealTarget = Identity.None;

                    break;
                case 3:
                    if (!Team.IsInTeam) { return; }
                    var leader = Team.Members.FirstOrDefault(t => t.IsLeader);
                    if (leader != null)
                        HealTarget = leader.Identity;
                    else
                        HealTarget = Identity.None;
                    break;
                case 4:

                    var player = DynelManager.Players.Where(p => p != null && p.Health > 0 && !p.Buffs.Contains(SpellID.PVPEnabled) && p.HealthPercent <= _settings["PetHealingValue"].AsInt32()).OrderBy(p => p.HealthPercent).FirstOrDefault();

                    if (player != null)
                    {
                        HealTarget = player.Identity;
                    }
                    else if (dyingPet != null)
                        HealTarget = dyingPet.Character.Identity;
                    else
                        HealTarget = Identity.None;

                    break;
            }

            if (HealTarget == Identity.None && CurrentHealPetAction != HealPetAction.Following && CurrentHealPetAction != HealPetAction.SendFollow)
            {
                CurrentHealTarget = Identity.None;
                CurrentHealPetAction = HealPetAction.SendFollow;
            }

            switch (CurrentHealPetAction)
            {
                case HealPetAction.SendFollow:
                    healPet.Follow();
                    CurrentHealPetAction = HealPetAction.Following;
                    break;
                case HealPetAction.Following:
                    if (HealTarget != Identity.None) { CurrentHealPetAction = HealPetAction.SendHeal; }
                    break;
                case HealPetAction.SendHeal:
                    healPet.Heal(HealTarget);
                    CurrentHealTarget = HealTarget;
                    CurrentHealPetAction = HealPetAction.Healing;
                    break;
                case HealPetAction.Healing:
                    if (HealTarget == Identity.None)
                    {
                        CurrentHealPetAction = HealPetAction.SendFollow;
                    }
                    if (HealTarget != CurrentHealTarget)
                    {
                        CurrentHealPetAction = HealPetAction.SendHeal;
                    }
                    break;
            }
        }

        private void HandleMezzPet()
        {
            if (_settings["PetMezzingOption"].AsInt32() != 2) { return; }
            var mezzPet = DynelManager.LocalPlayer.Pets.Where(pet => pet?.Type == PetType.Support && pet?.Character.Nano >= 1).FirstOrDefault();
            if (mezzPet == null) { return; }
            if (!CurrentPetCommand.ContainsKey(mezzPet.Identity.Instance))
            {
                CurrentPetCommand.Add(mezzPet.Identity.Instance, PetCommand.Follow);
            }

            var target = DynelManager.Characters.FirstOrDefault(c => !c.IsPlayer && !c.IsPet && c.IsAttacking && c.FightingTarget != null && !c.Buffs.Contains(NanoLine.Mezz) &&
            (c.FightingTarget?.Identity == DynelManager.LocalPlayer.Identity || Team.Members.Contains(c.FightingTarget.Identity)));

            switch (CurrentPetCommand[mezzPet.Identity.Instance])
            {
                case PetCommand.Follow:
                    if (target == null) { return; }
                    mezzPet?.Attack(target.Identity);
                    MezzTarget = target.Identity;
                    break;
                case PetCommand.Attack:
                    if (MezzTarget != Identity.None && target?.Identity == MezzTarget) { return; }
                    if (mezzPet.Character.IsAttacking && mezzPet?.Character.FightingTarget != null && !mezzPet.Character.FightingTarget.Buffs.Contains(NanoLine.Mezz)) { return; }
                    mezzPet?.Follow();
                    MezzTarget = Identity.None;
                    break;
            }
        }

        #endregion
    }
}
