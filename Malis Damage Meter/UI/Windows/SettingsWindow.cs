using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;
using static MalisDamageMeter.MainWindow;

namespace MalisDamageMeter
{
    public class SettingsWindow: AOSharpWindow
    {
        private readonly Views _views;

        public SettingsWindow(string name, string path, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade) : base(name, path, windowStyle, flags)
        {
            _views = new Views();
        }

        protected override void OnWindowCreating()
        {
            try
            {
                if (Window.FindView("Background", out _views.Background))
                {
                    _views.Background.SetBitmap(Textures.SettingsBackground);
                }

                if (Window.FindView("TotalPerMinute", out _views.TotalPerMinute))
                {
                    _views.TotalPerMinute.SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.TotalPerMinute));
                    _views.TotalPerMinute.Clicked = TotalPerMinuteClick;
                }

                if (Window.FindView("ActivePerMinute", out _views.ActivePerMinute))
                {
                    _views.ActivePerMinute.SetAllGfx(SetEnabledTexture(!DamageMeter.Window.ViewSettings.TotalPerMinute));
                    _views.ActivePerMinute.Clicked = ActivePerMinuteClick;
                }

                if (Window.FindView("AutoTimer", out _views.AutoTimer))
                {
                    _views.AutoTimer.SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.AutoToggleTimer));
                    _views.AutoTimer.Clicked = AutoStartTimerClick;
                }

                if (Window.FindView("LogMobs", out _views.LogMobs))
                {
                    _views.LogMobs.SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.LogMobs));
                    _views.LogMobs.Clicked = LogMobsClick;
                }

                if (Window.FindView("TotalValues", out _views.TotalValues))
                {
                    _views.TotalValues.SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.TotalValues));
                    _views.TotalValues.Clicked = TotalValuesClick;
                }

                if (Window.FindView("Help", out _views.Help))
                {
                    _views.Help.SetAllGfx(Textures.HelpButton);
                    _views.Help.Clicked = HelpClick;
                }

                if (Window.FindView("Close", out _views.Close))
                {
                    _views.Close.SetAllGfx(Textures.CloseButton);
                    _views.Close.Clicked = CloseClick;
                }
            }
            catch (Exception e)
            {
                Chat.WriteLine(e);
            }
        }

        private void ActivePerMinuteClick(object sender, ButtonBase e)
        {
            DamageMeter.Window.ViewSettings.TotalPerMinute = false;
            ((Button)e).SetAllGfx(SetEnabledTexture(true));
            _views.TotalPerMinute.SetAllGfx(SetEnabledTexture(false));
            DamageMeter.Settings.Save();
            Midi.Play("Click");
        }

        private void TotalPerMinuteClick(object sender, ButtonBase e)
        {
            DamageMeter.Window.ViewSettings.TotalPerMinute = true;
            ((Button)e).SetAllGfx(SetEnabledTexture(true));
            _views.ActivePerMinute.SetAllGfx(SetEnabledTexture(false));
            DamageMeter.Settings.Save();
            Midi.Play("Click");
        }

        private void HelpClick(object sender, ButtonBase e)
        {
            if (DamageMeter.Window.HelpWindow != null && DamageMeter.Window.HelpWindow.Window.IsValid && DamageMeter.Window.HelpWindow.Window.IsVisible)
                return;

            DamageMeter.Window.HelpWindow = new HelpWindow();
            DamageMeter.Window.HelpWindow.Window.Show(true);
            Midi.Play("Click");
        }


        private void CloseClick(object sender, ButtonBase e)
        {
            DamageMeter.Window.SettingsWindow = null;
            Window.Close();
            Midi.Play("Click");
        }

        private void AutoStartTimerClick(object sender, ButtonBase e)
        {
            DamageMeter.Window.ViewSettings.AutoToggleTimer = !DamageMeter.Window.ViewSettings.AutoToggleTimer;
            DamageMeter.Settings.Save();
            ((Button)e).SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.AutoToggleTimer));
            Midi.Play("Click");
        }

        private void LogMobsClick(object sender, ButtonBase e)
        {
            DamageMeter.Window.ViewSettings.LogMobs = !DamageMeter.Window.ViewSettings.LogMobs;
            DamageMeter.Settings.Save();
            ((Button)e).SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.LogMobs));
            Midi.Play("Click");
        }

        private void TotalValuesClick(object sender, ButtonBase e)
        {
            DamageMeter.Window.ViewSettings.TotalValues = !DamageMeter.Window.ViewSettings.TotalValues;
            DamageMeter.Window.ViewCache.TotalDisplayView.Hide();
            DamageMeter.Settings.Save();
            ((Button)e).SetAllGfx(SetEnabledTexture(DamageMeter.Window.ViewSettings.TotalValues));
            Midi.Play("Click");
        }

        private int SetEnabledTexture(bool enabled) => enabled ? Textures.GreenCircleButton : Textures.RedCircleButton;

        internal class Views
        {
            public Button Help;
            public Button Close;
            public Button AutoTimer;
            public Button ActivePerMinute;
            public Button TotalPerMinute;
            public Button LogMobs;
            public Button TotalValues;
            public BitmapView Background;
        }   
    }
}