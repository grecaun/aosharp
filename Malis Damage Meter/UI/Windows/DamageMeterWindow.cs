using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MalisDamageMeter
{
    public class DamageMeterWindow : MainWindow
    {
        private DisplayConfig _displayConfig;
        private int _elapsedTimeIndex = 0;

        public DamageMeterWindow(string name, string path, int textureStartId, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade) : base(name, path, windowStyle, flags)
        {
            Utils.LoadCustomTextures($"{DamageMeter.PluginDir}\\UI\\Textures\\", textureStartId);

            if (DamageMeter.Settings.ShowTutorial)
            {
                HelpWindow = new HelpWindow();
                HelpWindow.Window.MoveToCenter();
                HelpWindow.Window.Show(true);
            }
        }

        protected override void OnWindowCreating()
        {
            try
            {
                LoadSettings();

                if (Window.FindView("Background", out ViewCache.Background))
                {
                    ViewCache.Background.SetBitmap(Textures.Background);
                }

                if (Window.FindView("Icon", out ViewCache.Icon))
                {
                    ViewCache.Icon.SetBitmap(Textures.Header);
                }

                if (Window.FindView("ResumePause", out ViewCache.ResumePauseButton))
                {
                    ViewCache.ResumePauseButton.SetAllGfx(Textures.StartButton);
                    ViewCache.ResumePauseButton.Clicked = PauseClick;
                }

                if (Window.FindView("Reset", out ViewCache.ResetButton))
                {
                    ViewCache.ResetButton.SetAllGfx(Textures.ResetButton);
                    ViewCache.ResetButton.Clicked = ResetClick;
                }

                if (Window.FindView("Mode", out ViewCache.ModeButton))
                {
                    ViewCache.ModeButton.SetAllGfx(Textures.ModeButton);
                    ViewCache.ModeButton.Clicked = ModeClick;
                }

                if (Window.FindView("Log", out ViewCache.LogButton))
                {
                    ViewCache.LogButton.SetAllGfx(Textures.LogButton);
                    ViewCache.LogButton.Clicked = LogClick;
                }


                if (Window.FindView("Scope", out ViewCache.ScopeButton))
                {
                    int modeGfx = ViewSettings.Scope.SetIcon();
                    ViewCache.ScopeButton.SetAllGfx(modeGfx);
                    ViewCache.ScopeButton.Clicked = ScopeClick;
                }

                if (Window.FindView("Settings", out ViewCache.SettingsButton))
                {
                    ViewCache.SettingsButton.SetAllGfx(Textures.SettingsButton);
                    ViewCache.SettingsButton.Clicked = SettingsClick;
                }

                if (Window.FindView("ModeText", out ViewCache.ModeText))
                {
                    ViewCache.ModeText.Text = "Damage";
                }

                if (Window.FindView("TotalDisplay", out View totalDisplayRoot))
                {
                    ViewCache.TotalDisplayView = new TotalDisplayView(totalDisplayRoot);
                }

                if (Window.FindView("Elapsed", out ViewCache.Elapsed)) { }

                if (Window.FindView("Meters", out ViewCache.MetersRoot)) { }

                SetMeterDefaults();
            }
            catch (Exception e)
            {
                Chat.WriteLine(e);
            }
        }

        private void LogClick(object sender, ButtonBase e)
        {
            SaveAllDumps();
        }

        public void Update(object sender, float deltaTime)
        {
            if (!Window.IsValid)
                return;

            if (DamageMeter.Settings.AutoToggleTimer)
            {
                if (ViewSettings.IsPaused && ViewSettings.Scope.IsInCombat || !ViewSettings.IsPaused && ViewSettings.Scope.IsNotInCombat)
                {
                    PauseAction();
                    UpdateMainWindow();
                }
            }

            if (!ViewSettings.IsPaused)
            {
                if (ViewSettings.ElapsedTime.Count == _elapsedTimeIndex)
                    ViewSettings.ElapsedTime.Add(new DateTimeRange { StartTime = DateTime.Now, EndTime = DateTime.Now });
                else
                    ViewSettings.ElapsedTime[_elapsedTimeIndex].EndTime = DateTime.Now;

                if (ViewSettings.MeterUpdateTick.Elapsed)
                    UpdateMainWindow();
            }
        }

        private void UpdateMainWindow()
        {
            ViewCache.Elapsed.Text = Format.Time(ViewSettings.GetElapsedTimeSeconds());
            UpdateMeterViews();
        }

        public void UpdateMeterCount()
        {
            MeterView meterView = new MeterView();
            ViewCache.MetersRoot.AddChild(meterView.Root, true);
            MeterViews.Add(meterView);
            ViewCache.MetersRoot.FitToContents();
        }

        public void RefreshMeterDataActive(float highestValue)
        {
            for (int i = 0; i < MeterViews.Count; i++)
            {
                var simpleCharMeterData = _displayConfig.SimpleCharMeterData[i];
                var particTime = Format.GetParticipationTime(simpleCharMeterData.SimpleCharData.DamageStartTime, simpleCharMeterData.SimpleCharData.DamageEndTime, ViewSettings.ElapsedTime);
                MeterViews[i].InitMeterData(simpleCharMeterData, highestValue);
                MeterViews[i].LeftTextView.Text = $"{i + 1}. {simpleCharMeterData.SimpleCharData.Name}";
                MeterViews[i].RightTextView.Text = $"{Format.TotalDmgFormat(simpleCharMeterData.Total)} " +
                $"({Format.DpmFormat(simpleCharMeterData.Total, particTime)}, " +
                $" {Format.PercentFormat((float)simpleCharMeterData.Total / _displayConfig.TotalAmount)})";
            }
        }

        public void RefreshMeterDataTotal(float highestValue, float totalElapsedTime)
        {
            for (int i = 0; i < MeterViews.Count; i++)
            {
                var simpleCharMeterData = _displayConfig.SimpleCharMeterData[i];
                MeterViews[i].InitMeterData(simpleCharMeterData, highestValue);
                MeterViews[i].LeftTextView.Text = $"{i + 1}. {simpleCharMeterData.SimpleCharData.Name}";
                MeterViews[i].RightTextView.Text = $"{Format.TotalDmgFormat(simpleCharMeterData.Total)} " +
                $"({Format.DpmFormat(simpleCharMeterData.Total, totalElapsedTime)}, " +
                $" {Format.PercentFormat((float)simpleCharMeterData.Total / _displayConfig.TotalAmount)})";
            }
        }

        public void UpdateMeterViews()
        {
            if (HitRegisters.Characters.Count == 0)
                return;

            _displayConfig = HitRegisters.GetDisplayConfig(ViewSettings.Mode);

            if (MeterViews.Count != _displayConfig.SimpleCharMeterData.Count)
                MeterViews.Redraw(ViewCache.MetersRoot, _displayConfig.SimpleCharMeterData.Count);

            if (_displayConfig.SimpleCharMeterData.Count == 0)
                return;

            float highestValue = _displayConfig.SimpleCharMeterData[0].Total;

            if (!ViewSettings.TotalPerMinute)
            {
                RefreshMeterDataActive(highestValue);
            }
            else
            {
                RefreshMeterDataTotal(highestValue, ViewSettings.GetElapsedTimeSeconds());
            }


            if (ViewSettings.TotalValues && MeterViews.Count > 1)
            {
                ViewCache.TotalDisplayView.Show();
                ViewCache.TotalDisplayView.TotalValue.Text = $"{Format.TotalDmgFormat(_displayConfig.TotalAmount)} " +
                $"({Format.DpmFormat(_displayConfig.TotalAmount, ViewSettings.GetElapsedTimeSeconds())})";
            }
        }

        private void PauseClick(object sender, ButtonBase e)
        {
            UpdateMeterViews();
            PauseAction();
            Midi.Play("Click");
        }

        private void ResetClick(object sender, ButtonBase e)
        {
            SetMeterDefaults();
            Midi.Play("Click");
        }

        private void ScopeClick(object sender, ButtonBase e)
        {
            ScopeAction();
            Midi.Play("Click");
        }

        private void ModeClick(object sender, ButtonBase e)
        {
            var modeView = ViewSettings.Mode.GetNext();
            ViewSettings.Mode.Current = modeView.Key;
            ViewCache.ModeText.Text = modeView.Value;
            ViewCache.TotalDisplayView.Hide();

            UpdateMeterViews();

            foreach (MeterView s in MeterViews)
                s.ResetMeter();

            Midi.Play("Click");
        }

        private void SettingsClick(object sender, ButtonBase e)
        {
            if (SettingsWindow != null && SettingsWindow.Window.IsVisible)
                return;

            SettingsWindow = new SettingsWindow("MdmSettings", $"{DamageMeter.PluginDir}\\UI\\Windows\\SettingsWindow.xml");
            SettingsWindow.Show();
            Midi.Play("Click");
        }

        private void PauseAction()
        {
            int texId = ViewSettings.IsPaused ? Textures.PauseButton : Textures.StartButton;
            ViewCache.ResumePauseButton.SetAllGfx(texId);
            ViewSettings.IsPaused = !ViewSettings.IsPaused;
            _elapsedTimeIndex = ViewSettings.ElapsedTime.Count;
            UpdateMainWindow();
        }

        private void SetMeterDefaults()
        {
            HitRegisters.Characters.Clear();
            HitRegisters.Pets.Clear();

            ViewCache.Elapsed.Text = "0:00:00:0";
            ViewCache.TotalDisplayView.Hide();

            ViewSettings.ElapsedTime = new List<DateTimeRange>();
            _elapsedTimeIndex = 0;

            foreach (var views in MeterViews)
            {
                ViewCache.MetersRoot.RemoveChild(views.Root);
                views.Root.Dispose();
            }

            MeterViews.Clear();
            ViewCache.MetersRoot.FitToContents();
        }

        private void ScopeAction()
        {
            ViewSettings.Scope.Next();
            ViewCache.ScopeButton.SetAllGfx(ViewSettings.Scope.SetIcon());
            DamageMeter.Settings.Scope = ViewSettings.Scope.Current;
            DamageMeter.Settings.Save();
            Chat.WriteLine($"Current Scope set to: {ViewSettings.Scope.Current}");
        }

        public new void Show()
        {
            base.Show();
            var screenSize = Window.GetScreenSize();

            if ((DamageMeter.Settings.Frame.X <= 0 || DamageMeter.Settings.Frame.Y <= 0) ||
                (DamageMeter.Settings.Frame.X > screenSize.X || DamageMeter.Settings.Frame.Y > screenSize.Y))
                Window.MoveToCenter();
            else
                Window.MoveTo(DamageMeter.Settings.Frame.X, DamageMeter.Settings.Frame.Y);
        }
        private void SaveAllDumps()
        {
            if (HitRegisters.Characters.Count == 0)
            {
                Chat.WriteLine("Error: No registered characters. Log not saved.", ChatColor.Red);
                return;
            }

            if (_displayConfig == null)
                return;

            Chat.WriteLine("Preview Logs:");

            var basicDmgDump = Format.DumpDmgFormatBasic(ViewSettings.ElapsedTime, ViewSettings.GetElapsedTimeSeconds());

            Chat.WriteLine(basicDmgDump);
            File.WriteAllText($"{Utils.FindScriptFolder()}\\mdmg", basicDmgDump);

            var basicHealDump = Format.DumpHealingFormatBasic(ViewSettings.ElapsedTime, ViewSettings.GetElapsedTimeSeconds());

            Chat.WriteLine(basicHealDump);
            File.WriteAllText($"{Utils.FindScriptFolder()}\\mheal", basicHealDump);

            foreach (var simpleCharData in HitRegisters.Characters.Values.OrderBy(x => x.Name))
            {
                if (simpleCharData.DamageSources.Total == 0 && simpleCharData.HealSource.Total == 0)
                    continue;

                var advDump = Format.DumpDmgFormatAdvanced(simpleCharData, ViewSettings.ElapsedTime, ViewSettings.GetElapsedTimeSeconds());
                Chat.WriteLine(advDump);
                File.WriteAllText($"{Utils.FindScriptFolder()}\\mdmg_{simpleCharData.Name.ToLower()}", advDump);
            }

            Chat.WriteLine("Logs saved to script folder.", ChatColor.Green);
        }

        private void LoadSettings()
        {
            ViewSettings.IsPaused = true;
            ViewSettings.Mode.Current = ModeEnum.Damage;
            ViewSettings.Scope.Current = DamageMeter.Settings.Scope;
            ViewSettings.LogMobs = DamageMeter.Settings.LogMobs;
            ViewSettings.TotalValues = DamageMeter.Settings.TotalValues;
            ViewSettings.TotalPerMinute = DamageMeter.Settings.TotalPerMinute;
            ViewSettings.AutoToggleTimer = DamageMeter.Settings.AutoToggleTimer;
            ViewSettings.Scope.Current = DamageMeter.Settings.Scope;
        }
    }
}