using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace ManagerSocialPet
{
    public class ManagerSocialPet : AOPluginEntry
    {

        protected Settings _settings;

        Window settingsWindow;

        protected int SelectedSocialPet;

        protected int SavedSocialPetID = 0;

        protected int CurrentSocialPetID = 0;

        protected bool CurrentlyBusy = false;

        protected double _lastZonedTime = Time.AONormalTime;

        protected double _timer = 0f;

        private static List<Settings> _settingsToSave = new List<Settings>();

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                Chat.WriteLine("Social Pet Manager Loaded!");
                Chat.WriteLine("/ManagerSocialPet for settings");

                _settings = new Settings("ManagerSocialPet");
                _settings.AddVariable("SelectedSocialPet", 287160);

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                _settingsToSave.Add(_settings);

                Chat.RegisterCommand("ManagerSocialPet", ManagerCommand);

                if (PetCurrentSocial() != null)
                    CurrentSocialPetID = 1; //we know that we have a pet, but we do not know which one

                Game.OnUpdate += OnUpdate;
                Game.TeleportEnded += TeleportEnded;
                UIController.WindowDeleted += Windowclosed;
                Network.N3MessageSent += N3MessageSent;

            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred: " + ex.Message;
                Chat.WriteLine(errorMessage);
                Chat.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }

        public override void Teardown()
        {
            Save();
            Game.OnUpdate -= OnUpdate;
            Game.TeleportEnded -= TeleportEnded;
            UIController.WindowDeleted -= Windowclosed;
            Network.N3MessageSent -= N3MessageSent;
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if (Game.IsZoning || Time.AONormalTime < _lastZonedTime + 2.0) { return; }

            if (Time.AONormalTime < _timer + 0.4) // 0.4 sec tick, low profile manager
                return;

            if (CurrentlyBusy)
                return;

            _timer = Time.AONormalTime;
            CastSocialPet();
        }

        private void CastSocialPet()
        {
            var pet = PetCurrentSocial();

            // user input
            SavedSocialPetID = (int)_settings["SelectedSocialPet"].AsInt32();

            // we do not have a pet
            if (pet == null)
            {
                // and we do not want a pet
                if (SavedSocialPetID == 0)
                {
                    CurrentSocialPetID = 0;
                    return;
                }
                // we want a pet
                else
                {
                    // so proceed to the casting
                }
            }
            // we have a pet already
            else
            {
                // we are content with our pet
                if (SavedSocialPetID == CurrentSocialPetID)
                {
                    return;
                }
                // we just want to get rid of it
                else if (SavedSocialPetID == 0 && CurrentSocialPetID > 0)
                {
                    _timer += 0.4;
                    CurrentSocialPetID = 0;
                    PetTerminate(pet);
                    return;
                }
                // we want a new pet
                else
                {
                    // so proceed to the casting
                }
            }

            // Find the pet spell
            if (!Spell.Find(SavedSocialPetID, out Spell newPetSpell))
                return;

            // We have a replacement pet, so let's terminate the old one
            if (pet != null && CurrentSocialPetID > 0)
            {
                _timer += 0.4;
                CurrentSocialPetID = 0;
                PetTerminate(pet);
                return;
            }

            // check if we are ready to cast the new pet
            if (!CanCast(newPetSpell))
                return;

            // cast
            _timer += 2.1;
            newPetSpell.Cast(false);
            CurrentSocialPetID = SavedSocialPetID;

        }

        public Pet PetCurrentSocial()
        {
            if (DynelManager.LocalPlayer.Pets.Count() == 0)
                return null;

            foreach (Pet pet in DynelManager.LocalPlayer.Pets)
            {
                if (pet.Type == PetType.Social)
                {
                    return pet;
                }
            }
            return null;
        }

        private void PetTerminate(Pet pet)
        {
            if (pet == null)
                return;

            Network.Send(new PetCommandMessage()
            {
                Command = PetCommand.Terminate,
                Pets = new PetBase[1] {
                    pet
                }
            });
        }

        public static bool CanCast(Spell spell)
        {
            var localPlayer = DynelManager.LocalPlayer;

            if (Playfield.ModelIdentity.Instance == 152)
                return false;

            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 0)
                return false;

            if (localPlayer.MovementState == MovementState.Fly || localPlayer.IsFalling)
                return false;

            if (!Spell.List.Any(cast => cast.IsReady) || Spell.HasPendingCast)
                return false;

            if (!localPlayer.MovementStatePermitsCasting)
                return false;

            if (DynelManager.LocalPlayer.GetStat(Stat.TemporarySkillReduction) > 1)
                return false;

            return spell.Cost < DynelManager.LocalPlayer.Nano;
        }

        private void TeleportEnded(object sender, EventArgs e)
        {
            _lastZonedTime = Time.AONormalTime;
        }

        public static Array SocialPets = new int[]{
            300453,
            253168,
            287160,
            295322,
            303127,
            294060,
            300803,
            300850,
            290645,
            300806,
            301663,
            248386,
            296513,
            302548,
            295315,
            290122,
            293828,
            290062,
            295327,
            300571,
            277450,
            300439,
            304789,
            303649,
            289971,
            288561,
            285678,
            285056,
            294056
        };

        private void Windowclosed(object sender, Window e)
        {
            switch (e.Name)
            {
                case "ManagerSocialPet":
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
            Game.OnUpdate -= OnUpdate;
            Game.TeleportEnded -= TeleportEnded;
            UIController.WindowDeleted -= Windowclosed;
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
            if (settingsWindow?.IsValid == true)
            {
                Window_Closed_helper();

                settingsWindow.Close();
                settingsWindow = null;
                return;
            }
            else
            {
                settingsWindow = Window.CreateFromXml("ManagerSocialPet", PluginDirectory + "\\UI\\ManagerSocialPetSettingWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());
                settingsWindow.Show(true);

                settingsWindow.FindView("ManagerSocialPetSettingWindow", out View SettingWindow);

                foreach (int pet in SocialPets)
                {
                    SettingWindow.FindChild(pet.ToString(), out RadioButton r);
                    if (r != null)
                    {
                        if (!Spell.Find(pet, out Spell validspell))
                            r.Enable(false);
                    }
                }
            }
        }

        public static void Save()
        {
            _settingsToSave.ForEach(settings => settings.Save());
        }
    }
}