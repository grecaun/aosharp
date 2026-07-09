using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using Shared;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        public void Sync_Pets_On_Off(object sender, ButtonBase e)
        {
            try
            {
                IPCChannel.Broadcast(new PetSync_On_Off_Message() { Sync_On_Off = _settings["SyncPets"].AsBool(), Sender = DynelManager.LocalPlayer.Identity });
                Sync_Pets_Toggle();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void HandlePetCommandViewClick(object s, ButtonBase button)
        {
            try
            {
                if (_petCommandWindow?.IsValid == true)
                {
                    SaveWindowBounds("PetComs", _petCommandWindow);
                    _petCommandWindow.Close();
                    _petCommandWindow = null;
                    return;
                }

                _petCommandWindow = Window.CreateFromXml("PetComs", PluginDir + "\\UI\\HandlerMainWindow\\PetCommandView.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);

                _petCommandWindow.MoveTo(_settings["PetComsTopLeftX"].AsFloat(), _settings["PetComsTopLeftY"].AsFloat());
                _petCommandWindow.ResizeTo(new Vector2(_settings["PetComsWidth"].AsFloat(), _settings["PetComsHeight"].AsFloat()));

                if (_petCommandWindow.FindView("ProfessionHandlerPetAttack", out Button PetAttack))
                    PetAttack.Clicked = PetAttackClicked;

                if (_petCommandWindow.FindView("ProfessionHandlerPetWait", out Button PetWait))
                    PetWait.Clicked = PetWaitClicked;

                if (_petCommandWindow.FindView("ProfessionHandlerPetWarp", out Button PetWarp))
                    PetWarp.Clicked = PetWarpClicked;

                if (_petCommandWindow.FindView("ProfessionHandlertPetFollow", out Button PetFollow))
                    PetFollow.Clicked = PetFollowClicked;

                _petCommandWindow.Show(true);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void PetAttackClicked(object s, ButtonBase button)
        {
            try
            {
                var target = Targeting.Target;
                if (target == null) return;
                DynelManager.LocalPlayer.Pets.Attack(target.Identity);
                IPCChannel.Broadcast(new PetAttackMessage() { Target = target.Identity });
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void PetWaitClicked(object s, ButtonBase button)
        {
            try
            {
                Send_Pets_Wait();
                IPCChannel.Broadcast(new PetWaitMessage());
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void PetWarpClicked(object s, ButtonBase button)
        {
            try
            {
                Warp_Pets();
                IPCChannel.Broadcast(new PetWarpMessage());
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        private void PetFollowClicked(object s, ButtonBase button)
        {
            try
            {
                Send_Pets_Follow();
                IPCChannel.Broadcast(new PetFollowMessage());
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void AOE_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                _settings["AOE"] = !_settings["AOE"].AsBool();
               
                if (_mainWindow?.IsValid == true && _mainWindow.FindView("BroadcastAOE", out Button aoeButton))
                {
                    aoeButton.SetStateColors();
                    aoeButton.SetLabelColor(_settings["AOE"].AsBool() ? Color.Green : Color.BrightRed);
                    aoeButton.SetLabel(_settings["AOE"].AsBool() ? "AOE Disable" : "AOE Enable");
                }

                IPCChannel.Broadcast(new AOEMessage() { AOEBool = _settings["AOE"].AsBool() });

                Save();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Hold_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                _settings["Roots/Snares/Stuns"] = !_settings["Roots/Snares/Stuns"].AsBool();
              
                if (_mainWindow?.IsValid == true && _mainWindow.FindView("BroadcastHold", out Button holdButton))
                {
                    holdButton.SetStateColors();
                    holdButton.SetLabelColor(_settings["Roots/Snares/Stuns"].AsBool() ? Color.Green : Color.BrightRed);
                    holdButton.SetLabel(_settings["Roots/Snares/Stuns"].AsBool() ? "Holds Disable" : "Holds Enable");
                }

                IPCChannel.Broadcast(new HoldsMessage() { HoldsBool = _settings["Roots/Snares/Stuns"].AsBool() });

                Save();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Send_Target_Clicked(object sender, ButtonBase e)
        {
            try
            {
                if (!Targeting.HasTarget) return;

                var target = DynelManager.Characters.Where(c => c.Identity == Targeting.Target.Identity).FirstOrDefault();
                if (target == null) return;

                IPCChannel.Broadcast(new RootSnareMessage() { Target = target.Identity });

                var spell = Spell.List.Where(x => new[] { NanoLine.Root, NanoLine.Snare }.Contains(x.Nanoline)).OrderByStackingOrder().FirstOrDefault();
                if (spell == null) return;
                spell.Cast(target, true);
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Specials_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                _settings["Specials"] = !_settings["Specials"].AsBool();
                
                if (_mainWindow?.IsValid == true && _mainWindow.FindView("BroadcastSpecials", out Button specialsButton))
                {
                    specialsButton.SetStateColors();
                    specialsButton.SetLabel(_settings["Specials"].AsBool() ? "Specials Disable" : "Specials Enable");
                    specialsButton.SetLabelColor(_settings["Specials"].AsBool() ? Color.Green : Color.BrightRed);
                }

                IPCChannel.Broadcast(new SpecialsMessage() { Specials = _settings["Specials"].AsBool() });

                Save();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Broadcast_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                IPCChannel.Broadcast(new SettingsUpdateMessage()
                {
                    Buffing = _settings["Buffing"].AsBool(),
                    Comps = _settings["Composites"].AsBool(),
                    Wait_For_Rez = _settings["WaitForRez"].AsBool()
                });

                Save();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Save_Ncus_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                if (_mainWindow.FindView("ReserveNCU", out TextInputView ncus))
                {
                    if (int.TryParse(ncus.Text, out int value))
                        _settings["Reserve NCU"] = value;
                }

                Save();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public void Save_Perks_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                if (_mainWindow.FindView("MAX_CONCURRENT_PERKS", out TextInputView perk))
                {
                    if (int.TryParse(perk.Text, out int value))
                        _settings["MAX_CONCURRENT_PERKS"] = value;
                }

                Save();
            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }
    }
}
