using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.Misc;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using AutomatonRoamba.IPCMessages;
using Buddy.Shared.UI;
using System;
using System.Runtime;

namespace AutomatonRoamba
{
    public class MainWindow : BuddyBaseWindow
    {
        public override string CoreViewRootName => "CoreViewRoot";
        public override View InfoView => View.CreateFromXml(FilePath.InfoView);

        private AutoResetInterval _uiUpdateTick;
        private TextView _activeConfigTextView;
        private TextView _activePathTextView;
        private bool IsWindowValid => Window != null && Window.IsValid && Window.IsVisible;

        private AutomatonRoambaSettings _settings;
        
        public MainWindow(string windowName, string baseWindowPath, string infoWindowPath, string coreViewPath, AutomatonRoambaSettings settings, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade) : base(windowName, baseWindowPath, infoWindowPath, coreViewPath, windowStyle, flags)
        {
            _uiUpdateTick = new AutoResetInterval(100);
            _settings = settings;
        }

        protected override void OnWindowCreating()
        {
            //Make sure to call the base first in order to init all the core views
            base.OnWindowCreating();

            CoreSettingsView.TitleTextView.Text = "Roamba";

            CoreSettingsView.SetData(_settings.CoreConfig);

            if (Window.FindView("PathEditor", out Button pathEditorButton))
            {
                pathEditorButton.Clicked += OnPathEditorClick;
            }

            if (Window.FindView("ConfigEditor", out Button configEditorButton))
            {
                configEditorButton.Clicked += OnConfigEditorClick;
            }

            if (Window.FindView("ActiveConfig", out _activeConfigTextView))
            {
                var configPath = _settings.ConfigEditorPath;
                _activeConfigTextView.Text = string.IsNullOrEmpty(configPath) ? "None" : System.IO.Path.GetFileNameWithoutExtension(configPath);
            }

            if (Window.FindView("ActivePath", out _activePathTextView))
            {
                var pathPath = _settings.PathEditorPath;
                _activePathTextView.Text = string.IsNullOrEmpty(pathPath) ? "None" : System.IO.Path.GetFileNameWithoutExtension(pathPath);
            }

            if (Window.FindView("ConfigSync", out Checkbox configSync))
            {
                configSync.SetValue(_settings.SyncConfig);
                configSync.Toggled += OnConfigSyncToggle;
            }

            if (Window.FindView("PathSync", out Checkbox pathSync))
            {
                pathSync.SetValue(_settings.SyncPath);
                pathSync.Toggled += OnPathSyncToggle;
            }

            var screenSize = Window.GetScreenSize();

            if (_settings.WindowCoords.X > screenSize.X || _settings.WindowCoords.Y > screenSize.Y)
                Window.MoveToCenter();
            else if (_settings.WindowCoords.X != 0 && _settings.WindowCoords.Y != 0)
                Window.MoveTo(_settings.WindowCoords.X, _settings.WindowCoords.Y);
            CoreSettingsView.EnabledButton.Clicked += OnEnable;
            Game.OnUpdate += MainWindowUpdate;

            var onInjectEnable = _settings.CoreConfig.OnInjectEnable && _settings.PathEditorConfig?.SPath?.PlayfieldId == Playfield.ModelIdentity.Instance;

            CoreSettingsView.EnabledButton.SetLabel(onInjectEnable ? "Stop" : "Start");

            if (onInjectEnable)
            {
                LoopTitleText(true);
                CoreSettingsView.IsButtonEnabled = true;
            }
        }

        private void OnConfigSyncToggle(object sender, bool e)
        {
            _settings.SyncConfig = e;
            _settings.Save();
        }

        private void OnPathSyncToggle(object sender, bool e)
        {
            _settings.SyncPath = e;
            _settings.Save();
        }

        public void UpdateActivePath(string fileName)
        {
            if (IsWindowValid)
                _activePathTextView.Text = fileName;
        }

        public void UpdateActiveConfig(string fileName)
        {
            if (IsWindowValid)
                _activeConfigTextView.Text = fileName;
        }

        private void OnEnable(object sender, ButtonBase e)
        {
            try
            {
                Enable();
            }
            catch (Exception ex)
            {
                AutomatonRoamba.Log.Information(ex.Message);
            }
        }

        private void Enable()
        {
            SaveSettings(false);

            if (_settings.PathEditorPath == string.Empty)
            {
                AutomatonRoamba.Log.Information("No path loaded");
                return;
            }

            if (_settings.ConfigEditorPath == string.Empty)
            {
                AutomatonRoamba.Log.Information("No config loaded");
                return;
            }

            _settings.PathEditorConfig?.SPath?.Delete();

            var configEditorConfig = ConfigEditorConfig.Load(_settings.ConfigEditorPath);
            var pathEditorConfig = PathEditorConfig.Load(_settings.PathEditorPath);

            if (pathEditorConfig.SPath?.PlayfieldId != Playfield.ModelIdentity.Instance)
            {
                CoreSettingsView.IsButtonEnabled = false;
                CoreSettingsView.SetButtonState(false);
                AutomatonRoamba.Log.Information("Cannot start a path saved for a different playfield.");
                return;
            }

            if (CoreSettingsView.IsButtonEnabled && pathEditorConfig.SPath.Waypoints.Count == 0)
            {
                CoreSettingsView.IsButtonEnabled = false;
                CoreSettingsView.SetButtonState(false);
                AutomatonRoamba.Log.Information("Path needs to have at least one waypoint");
                return;
            }

            AutomatonRoamba.Ipc.Broadcast(new EnabledIPCMessage
            {
                SetEnabled = CoreSettingsView.IsButtonEnabled,
                PathFolderPath = _settings.PathEditorPath,
                ConfigFolderPath = _settings.ConfigEditorPath,
                SyncConfig = _settings.SyncConfig,
                SyncPath = _settings.SyncPath
            });

            AutomatonRoamba.StateMachine.Context.UpdateConfig(configEditorConfig);
            AutomatonRoamba.StateMachine.Context.UpdateConfig(pathEditorConfig);

            OnEnabledPress(CoreSettingsView.IsButtonEnabled);
        }

        public void OnEnabledPress(bool result)
        {
            CoreSettingsView.SetButtonState(result);

            AutomatonRoamba.StateMachine.SetStatus(result);

            if (SMovementController.IsNavigating())
                SMovementController.Halt();

            AutomatonRoamba.Log.Information(result ? "Starting" : "Stopping");
            LoopTitleText(result);
        }

        public void LoopTitleText(bool state)
        {
            if (state)
            {
                _rainbowTimer = 0;
                _loopColor = true;
                _animateText = false;
                _playTitleAnim = true;
            }
            else
            {
                _loopColor = false;
            }
        }

        private void SaveSettings(bool displayMsg = true)
        {
            _settings.CoreConfig = CoreSettingsView.GetData();
            _settings.WindowCoords = new Vector2(Window.GetFrame().MinX, Window.GetFrame().MinY);
            _settings.Save();

            if (AutomatonRoamba.Ipc.ChannelId != _settings.CoreConfig.ChannelId)
                AutomatonRoamba.Ipc.SetChannelId(_settings.CoreConfig.ChannelId);

            if (displayMsg)
                AutomatonRoamba.Log.Information("Config saved! (use the Path Editor to save / export your path)", ChatColor.Green);
        }


        private void OnConfigEditorChange(string fullPath, bool modifySettings)
        {
            if (modifySettings)
            {
                _settings.ConfigEditorPath = fullPath;
                _settings.Save();
            }

            if (AutomatonRoamba.StateMachine.IsRunning)
            {
                AutomatonRoamba.Ipc.Broadcast(new EnabledIPCMessage
                {
                    SetEnabled = false,
                    SyncConfig = false,
                    SyncPath = false,
                    PathFolderPath = string.Empty,
                    ConfigFolderPath = string.Empty
                });

                CoreSettingsView.IsButtonEnabled = false;
                OnEnabledPress(false);
                AutomatonRoamba.StateMachine.Context.UpdateConfig(_settings.ConfigEditorConfig);
            }
        }

        private void OnPathEditorChange(string fullPath, bool modifySettings)
        {
            if (modifySettings)
            {
                _settings.PathEditorPath = fullPath;
                _settings.Save();
            }

            if (_settings.PathEditorConfig != null && _settings.PathEditorConfig.SPath != null)
                _settings.PathEditorConfig.SPath.IsLocked = true;

            if (AutomatonRoamba.StateMachine.IsRunning)
            {
                AutomatonRoamba.Ipc.Broadcast(new EnabledIPCMessage
                {
                    SetEnabled = false,
                    SyncConfig = false,
                    SyncPath = false,
                    PathFolderPath = string.Empty,
                    ConfigFolderPath = string.Empty
                });

                CoreSettingsView.IsButtonEnabled = false;
                OnEnabledPress(false);
                AutomatonRoamba.StateMachine.Context.UpdateConfig(_settings.PathEditorConfig);
            }
        }

        private void OnConfigEditorClick(object sender, ButtonBase e)
        {
            if (AutomatonRoamba.ConfigEditorWindow != null && AutomatonRoamba.ConfigEditorWindow.Window != null && AutomatonRoamba.ConfigEditorWindow.Window.IsValid)
                return;

            AutomatonRoamba.ConfigEditorWindow = new ConfigEditorWindow(
                  "AutomatonRoamba.ConfigEditor",
                  new ConfigEditorInitView(_settings.ConfigEditorConfig, _settings.ConfigEditorPath),
                  new ConfigEditorMainView());

            AutomatonRoamba.ConfigEditorWindow.InitView.OnConfigChange += OnConfigEditorChange;
            AutomatonRoamba.ConfigEditorWindow.MainView.OnSave += OnConfigEditorChange;
            AutomatonRoamba.ConfigEditorWindow.Show();
            AutomatonRoamba.ConfigEditorWindow.Window.MoveToCenter();
        }

        private void OnPathEditorClick(object sender, ButtonBase e)
        {
            if (AutomatonRoamba.PathEditorWindow != null && AutomatonRoamba.PathEditorWindow.Window != null && AutomatonRoamba.PathEditorWindow.Window.IsValid)
                return;

            AutomatonRoamba.PathEditorWindow = new PathEditorWindow(
                "AutomatonRoamba.PathEditor",
                new PathEditorInitView(_settings.PathEditorConfig, _settings.PathEditorPath),
                new PathEditorMainView());

            AutomatonRoamba.PathEditorWindow.InitView.OnConfigChange += OnPathEditorChange;
            AutomatonRoamba.PathEditorWindow.MainView.OnSave += OnPathEditorChange;

            AutomatonRoamba.PathEditorWindow.Show();
            AutomatonRoamba.PathEditorWindow.Window.MoveToCenter();
        }

        private void MainWindowUpdate(object sender, float delta)
        {
            PlayTitleAnim(delta);

            if (!_uiUpdateTick.Elapsed)
                return;

            if (AutomatonRoamba.PathEditorWindow != null && (AutomatonRoamba.PathEditorWindow.Window == null || !AutomatonRoamba.PathEditorWindow.Window.IsValid))
            {
                if (_settings.PathEditorConfig != null)
                    _settings.PathEditorConfig.SPath.IsLocked = true;

                AutomatonRoamba.PathEditorWindow = null;
            }
        }
        
        private float _rainbowTimer = 0f;
        private const float RAINBOW_DURATION = 3f;
        private bool _playTitleAnim = true;
        private const string FULL_TEXT = "Roamba";
        private bool _animateText = true; 
        private bool _animateColor = true;
        private bool _loopColor = false;  

        private void PlayTitleAnim(float delta)
        {
            if (!_playTitleAnim)
                return;

            bool animationComplete = _rainbowTimer >= RAINBOW_DURATION;

            if (!animationComplete || _loopColor)
            {
                _rainbowTimer += delta;

                if (_animateText && !animationComplete)
                {
                    int charsToShow = (int)((_rainbowTimer / RAINBOW_DURATION) * FULL_TEXT.Length);
                    charsToShow = Math.Min(charsToShow, FULL_TEXT.Length);
                    CoreSettingsView.TitleTextView.Text = FULL_TEXT.Substring(0, charsToShow);
                }
                else if (!_animateText)
                {
                    CoreSettingsView.TitleTextView.Text = FULL_TEXT;
                }

                if (_animateColor)
                {
                    float progress = (_rainbowTimer * 0.5f) % 1f;
                    uint fieryColor;
                    if (progress < 0.2f)
                        fieryColor = LerpColor(0xCC0000, 0xFF3300, progress / 0.2f);         
                    else if (progress < 0.4f)
                        fieryColor = LerpColor(0xFF3300, 0xFF6600, (progress - 0.2f) / 0.2f);
                    else if (progress < 0.6f)
                        fieryColor = LerpColor(0xFF6600, 0xFF9900, (progress - 0.4f) / 0.2f);
                    else if (progress < 0.8f)
                        fieryColor = LerpColor(0xFF9900, 0xFFFF00, (progress - 0.6f) / 0.2f);
                    else
                        fieryColor = LerpColor(0xFFFF00, 0xCC0000, (progress - 0.8f) / 0.2f);

                    CoreSettingsView.TitleTextView.SetColor(fieryColor);
                }
            }
            else
            {
                CoreSettingsView.TitleTextView.Text = FULL_TEXT;
                CoreSettingsView.TitleTextView.SetColor(0xFFFF00);
                _playTitleAnim = false;
            }
        }

        private uint LerpColor(uint color1, uint color2, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));

            byte r1 = (byte)((color1 >> 16) & 0xFF);
            byte g1 = (byte)((color1 >> 8) & 0xFF);
            byte b1 = (byte)(color1 & 0xFF);

            byte r2 = (byte)((color2 >> 16) & 0xFF);
            byte g2 = (byte)((color2 >> 8) & 0xFF);
            byte b2 = (byte)(color2 & 0xFF);

            byte r = (byte)(r1 + (r2 - r1) * t);
            byte g = (byte)(g1 + (g2 - g1) * t);
            byte b = (byte)(b1 + (b2 - b1) * t);

            return (uint)((r << 16) | (g << 8) | b);
        }
    }
}