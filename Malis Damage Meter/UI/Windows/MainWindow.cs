using AOSharp.Common.GameData.UI;
using AOSharp.Core.Misc;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MalisDamageMeter
{
    public class MainWindow : AOSharpWindow
    {
        public List<MeterView> MeterViews = new List<MeterView>();
        public Views ViewCache = new Views();
        public SettingsWindow SettingsWindow;
        public HelpWindow HelpWindow;
        public Config ViewSettings = new Config();
        public MainWindow(string name, string path, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade) : base(name, path, windowStyle, flags) { }

        protected override void OnWindowCreating() { }

        public class Views
        {
            public Button ResumePauseButton;
            public Button ResetButton;
            public Button ModeButton;
            public Button LogButton;
            public Button ScopeButton;
            public Button SettingsButton;
            public BitmapView Background;
            public BitmapView Icon;
            public TextView Elapsed;
            public TextView ModeText;
            public View MetersRoot;
            public TotalDisplayView TotalDisplayView;
        }

        public class Config
        {
            public Scope Scope = new Scope();
            public Mode Mode = new Mode();
            public AutoResetInterval MeterUpdateTick = new AutoResetInterval(125);

            public bool AutoToggleTimer;
            public bool TotalPerMinute;
            public bool LogMobs;
            public bool TotalValues;
            public bool IsPaused;
            public List<DateTimeRange> ElapsedTime;

            public float GetElapsedTimeSeconds()
            {
                double totalSeconds = 0;

                foreach (var range in ElapsedTime)
                {
                    totalSeconds += (range.EndTime - range.StartTime).TotalSeconds;
                }

                return (float)totalSeconds;
            }
        }

        public class DateTimeRange
        {
            public DateTime StartTime;
            public DateTime EndTime;
        }

        internal static class Textures
        {
            public const int PauseButton = 1430035;
            public const int StartButton = 1430036;
            public const int ResetButton = 1430037;
            public const int ModeButton = 1430038;
            public const int SettingsButton = 1430039;
            public const int Background = 1430040;
            public const int PowerbarBackground = 1430041;
            public const int PowerbarForeground = 1430042;
            public const int SoloScopeButton = 1430043;
            public const int TeamScopeButton = 1430044;
            public const int AllScopeButton = 1430045;
            public const int GreenCircleButton = 1430046;
            public const int RedCircleButton = 1430047;
            public const int Header = 1430048;
            public const int CloseButton = 1430049;
            public const int RedMinusButton = 1430050;
            public const int GreenPlusButton = 1430051;
            public const int SettingsBackground = 1430052;
            public const int HelpBackground = 1430053;
            public const int HelpButton = 1430054;
            public const int LogButton = 1430055;
        }
    }
}