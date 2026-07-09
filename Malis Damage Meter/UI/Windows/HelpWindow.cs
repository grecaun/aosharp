using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Common.Helpers;
using AOSharp.Common.Unmanaged.Imports;
using AOSharp.Common.Unmanaged.Interfaces;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MalisDamageMeter.MainWindow;

namespace MalisDamageMeter
{
    public class HelpWindow
    {
        public Window Window;

        public HelpWindow()
        {
            Window = Window.CreateFromXml("MalisDmgMeterHelp", $"{DamageMeter.PluginDir}\\UI\\Windows\\HelpWindow.xml",
                WindowStyle.Popup, WindowFlags.AutoScale | WindowFlags.NoFade);

            if (Window.FindView("Start", out Button Start)) { Start.SetAllGfx(Textures.StartButton); }

            if (Window.FindView("Pause", out Button Pause)) { Pause.SetAllGfx(Textures.PauseButton); }

            if (Window.FindView("Reset", out Button Reset)) { Reset.SetAllGfx(Textures.ResetButton); }

            if (Window.FindView("Log", out Button Log)) { Log.SetAllGfx(Textures.LogButton); }

            if (Window.FindView("Solo", out Button Solo)) { Solo.SetAllGfx(Textures.SoloScopeButton); }

            if (Window.FindView("Group", out Button Group)) { Group.SetAllGfx(Textures.TeamScopeButton); }

            if (Window.FindView("All", out Button All)) { All.SetAllGfx(Textures.AllScopeButton); }

            if (Window.FindView("Mode", out Button Mode)) { Mode.SetAllGfx(Textures.ModeButton); }

            if (Window.FindView("Settings", out Button Settings)) { Settings.SetAllGfx(Textures.SettingsButton); }

            if (Window.FindView("MeterViewDmgRoot", out View meterViewDmgRoot))
            {
                MeterView tutDmgMeter = new MeterView();

                var barValues = new List<float> { 0.2f, 0.2f, 0.2f, 0.1f, 0.4f };
                var barColors = new List<uint>
                {
                    MeterViewColors.DamageAutoAttack,
                    MeterViewColors.DamageSpecials,
                    MeterViewColors.DamageNanobots,
                    MeterViewColors.DamagePet,
                    MeterViewColors.DamageDeflect,
                };

                float barValue = 0;

                for (int i = 0; i < barColors.Count; i++)
                {
                    barValue += barValues[i];
                    tutDmgMeter.PowerBars[i].PowerBarView.Value = barValue;
                    tutDmgMeter.PowerBars[i].PowerBarView.SetBarColor(barColors[i]);
                }

                tutDmgMeter.LeftTextView.Text = "1.) Damage";
                tutDmgMeter.RightTextView.Text = "69M (69.69K, 100%)";

                meterViewDmgRoot.AddChild(tutDmgMeter.Root, true);

            }

            if (Window.FindView("MeterViewHealRoot", out View meterViewHealRoot))
            {
                MeterView tutHealMeter = new MeterView();

                var barValues = new List<float> { 0.5f, 0.5f };
                var barColors = new List<uint>
                {
                    MeterViewColors.HealUser,
                    MeterViewColors.HealPet,
                };

                float barValue = 0;

                for (int i = 0; i < barColors.Count; i++)
                {
                    barValue += barValues[i];
                    tutHealMeter.PowerBars[i].PowerBarView.Value = barValue;
                    tutHealMeter.PowerBars[i].PowerBarView.SetBarColor(barColors[i]);
                }

                tutHealMeter.LeftTextView.Text = "1.) Healing";
                tutHealMeter.RightTextView.Text = "69M (69.69K, 100%)";

                meterViewHealRoot.AddChild(tutHealMeter.Root, true);
            }

            if (Window.FindView("Commands", out TextView cmdTextView))
            {
                cmdTextView.Text = $"\n\n " +
                  $"- /mdmg - Basic damage log\n " +
                  $"- /mheal - Basic healing log\n " +
                  $"- /mdmg_name - Advanced log\n ";
            }

            if (Window.FindView("Info", out TextView infoTextView))
            {
                infoTextView.Text = $"\n " +
                $"- Pets get automatically registered\n\n " +
                $"- Open 'Settings' to config your meter\n " +
                $"  or to reopen this window again.\n\n " +
                $"- For bugs / glitches / requests:\n " +
                $"  Discord:  Pixelmania#0349\n";
            }

            if (Window.FindView("Close", out Button _closeHelp))
            {
                _closeHelp.SetAllGfx(1430049);
                _closeHelp.Clicked = CloseClick;
            }

            if (Window.FindView("Logo", out BitmapView _logo))
            {
                _logo.SetBitmap(Textures.HelpBackground);
            }
        }

        private void CloseClick(object sender, ButtonBase e)
        {
            Midi.Play("Click");
            Window.Close();
        }
    }
}