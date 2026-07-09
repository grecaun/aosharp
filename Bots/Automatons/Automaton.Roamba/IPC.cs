using AOSharp.Core;
using AOSharp.Core.IPC;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using AutomatonRoamba.IPCMessages;
using System.IO;
using System.Runtime;
using Path = System.IO.Path;

namespace AutomatonRoamba
{
    public class IPC : IPCChannelBase
    {
        protected override int _localDynelId => DynelManager.LocalPlayer.Identity.Instance;
        internal byte ChannelId;
        private PathEditorConfig _ipcPath;
        private AutomatonRoambaSettings _settings;

        public IPC(byte channelId, AutomatonRoambaSettings settings) : base(channelId)
        {
            _settings = settings;
            ChannelId = channelId;
            RegisterCallback((int)IPCOpcode.Enabled, OnEnabledMessage);
        }

        public new void SetChannelId(byte channelId)
        {
            ChannelId = channelId;
            base.SetChannelId(channelId);
        }

        private void OnEnabledMessage(int arg1, IPCMessage message)
        {
            EnabledIPCMessage enabledIpc = (EnabledIPCMessage)message;
            AutomatonRoamba.MainWindow.Show();

            _ipcPath?.SPath?.Delete();
            AutomatonRoamba.StateMachine?.Context?.PathEditorConfig?.SPath?.Delete();
            _settings.PathEditorConfig?.SPath?.Delete();

            if (enabledIpc.SetEnabled)
            {
                if (enabledIpc.SyncConfig && enabledIpc.ConfigFolderPath != string.Empty)
                {
                    AutomatonRoamba.Log.Information($"Loaded config via IPC: {Path.GetFileNameWithoutExtension(enabledIpc.ConfigFolderPath)}");
                    AutomatonRoamba.StateMachine.Context.UpdateConfig(ConfigEditorConfig.Load(enabledIpc.ConfigFolderPath));
                }
                else
                {
                    if (File.Exists(_settings.ConfigEditorPath))
                        AutomatonRoamba.StateMachine.Context.UpdateConfig(ConfigEditorConfig.Load(_settings.ConfigEditorPath));
                }

                if (enabledIpc.SyncPath && enabledIpc.PathFolderPath != string.Empty)
                {
                    AutomatonRoamba.StateMachine?.Context?.PathEditorConfig?.SPath?.Delete();
                    _settings.PathEditorConfig?.SPath?.Delete();
                    var pathConfig = PathEditorConfig.Load(enabledIpc.PathFolderPath);
                    _ipcPath = pathConfig;
                    AutomatonRoamba.Log.Information($"Loaded path via IPC: {Path.GetFileNameWithoutExtension(enabledIpc.PathFolderPath)}");
                    AutomatonRoamba.StateMachine.Context.UpdateConfig(pathConfig);
                }
                else
                {
                    if (File.Exists(_settings.PathEditorPath))
                        AutomatonRoamba.StateMachine.Context.UpdateConfig(PathEditorConfig.Load(_settings.PathEditorPath));
                }

                if (AutomatonRoamba.StateMachine.Context.PathEditorConfig == null)
                {
                    AutomatonRoamba.Log.Information("No path loaded");
                    return;
                }

                if (AutomatonRoamba.StateMachine.Context.ConfigEditorConfig == null)
                {
                    AutomatonRoamba.Log.Information("No config found");
                    return;
                }
            }
            else
            {
                var pathConfig = PathEditorConfig.Load(_settings.PathEditorPath);
                _settings.PathEditorConfig = pathConfig;
                AutomatonRoamba.StateMachine.Context.UpdateConfig(pathConfig);
                AutomatonRoamba.StateMachine.Context.UpdateConfig(ConfigEditorConfig.Load(_settings.ConfigEditorPath));
            }

            AutomatonRoamba.MainWindow.OnEnabledPress(enabledIpc.SetEnabled);
        }
    }
}
