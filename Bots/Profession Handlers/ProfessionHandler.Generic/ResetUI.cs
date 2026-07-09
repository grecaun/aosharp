using AOSharp.Common.GameData;
using AOSharp.Core.UI;

namespace ProfessionHandler.Generic
{
    public partial class GenericProfessionHandler
    {
        private void ResetUI(string arg1, string[] arg2, ChatWindow window)
        {
            _settings["MainWindowTopLeftX"] = 50f;
            _settings["MainWindowTopLeftY"] = 50f;

            _settings["BuffsTopLeftX"] = 50f;
            _settings["BuffsTopLeftY"] = 50f;
            _settings["BuffsWidth"] = 300f;
            _settings["BuffsHeight"] = 300f;

            _settings["DebuffsTopLeftX"] = 50f;
            _settings["DebuffsTopLeftY"] = 50f;
            _settings["DebuffsWidth"] = 300f;
            _settings["DebuffsHeight"] = 300f;

            _settings["HealsTopLeftX"] = 50f;
            _settings["HealsTopLeftY"] = 50f;
            _settings["HealsWidth"] = 300f;
            _settings["HealsHeight"] = 300f;

            _settings["HoldsTopLeftX"] = 50f;
            _settings["HoldsTopLeftY"] = 50f;
            _settings["HoldsWidth"] = 300f;
            _settings["HoldsHeight"] = 300f;

            _settings["NukesTopLeftX"] = 50f;
            _settings["NukesTopLeftY"] = 50f;
            _settings["NukesWidth"] = 300f;
            _settings["NukesHeight"] = 300f;

            _settings["PerksTopLeftX"] = 50f;
            _settings["PerksTopLeftY"] = 50f;
            _settings["PerksWidth"] = 300f;
            _settings["PerksHeight"] = 300f;

            _settings["PetsTopLeftX"] = 50f;
            _settings["PetsTopLeftY"] = 50f;
            _settings["PetsWidth"] = 300f;
            _settings["PetsHeight"] = 300f;

            _settings["TauntsTopLeftX"] = 50f;
            _settings["TauntsTopLeftY"] = 50f;
            _settings["TauntsWidth"] = 300f;
            _settings["TauntsHeight"] = 300f;

            _settings["MorphsTopLeftX"] = 50f;
            _settings["MorphsTopLeftY"] = 50f;
            _settings["MorphsWidth"] = 300f;
            _settings["MorphsHeight"] = 300f;

            _settings["PetComsTopLeftX"] = 50f;
            _settings["PetComsTopLeftY"] = 50f;
            _settings["PetComsWidth"] = 300f;
            _settings["PetComsHeight"] = 300f;

            _settings["ProcsTopLeftX"] = 50f;
            _settings["ProcsTopLeftY"] = 50f;
            _settings["ProcsWidth"] = 300f;
            _settings["ProcsHeight"] = 300f;

            _settings["InfoTopLeftX"] = 50f;
            _settings["InfoTopLeftY"] = 50f;
            _settings["InfoWidth"] = 300f;
            _settings["InfoHeight"] = 300f;


            _settings.Save();

            if (_mainWindow?.IsValid == true)
                _mainWindow.MoveTo(50f, 50f);

            if (BuffWindow.CurrentWindow?.IsValid == true)
            {
                BuffWindow.CurrentWindow.MoveTo(50f, 50f);
                BuffWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (DebuffWindow.CurrentWindow?.IsValid == true)
            {
                DebuffWindow.CurrentWindow.MoveTo(50f, 50f);
                DebuffWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (HealWindow.CurrentWindow?.IsValid == true)
            {
                HealWindow.CurrentWindow.MoveTo(50f, 50f);
                HealWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (HoldWindow.CurrentWindow?.IsValid == true)
            {
                HoldWindow.CurrentWindow.MoveTo(50f, 50f);
                HoldWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (NukeWindow.CurrentWindow?.IsValid == true)
            {
                NukeWindow.CurrentWindow.MoveTo(50f, 50f);
                NukeWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (PerkWindow.CurrentWindow?.IsValid == true)
            {
                PerkWindow.CurrentWindow.MoveTo(50f, 50f);
                PerkWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (PetWindow.CurrentWindow?.IsValid == true)
            {
                PetWindow.CurrentWindow.MoveTo(50f, 50f);
                PetWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (TauntWindow.CurrentWindow?.IsValid == true)
            {
                TauntWindow.CurrentWindow.MoveTo(50f, 50f);
                TauntWindow.CurrentWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (_morphWindow?.IsValid == true)
            {
                _morphWindow.MoveTo(50f, 50f);
                _morphWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (_petCommandWindow?.IsValid == true)
            {
                _petCommandWindow.MoveTo(50f, 50f);
                _petCommandWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (_procWindow?.IsValid == true)
            {
                _procWindow.MoveTo(50f, 50f);
                _procWindow.ResizeTo(new Vector2(300f, 300f));
            }

            if (_infoWindow?.IsValid == true)
            {
                _infoWindow.MoveTo(50f, 50f);
                _infoWindow.ResizeTo(new Vector2(300f, 300f));
            }

            Chat.WriteLine("UI layout reset to defaults.");
        }
    }
}
