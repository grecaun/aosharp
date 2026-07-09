using AOSharp.Common.GameData;
using Newtonsoft.Json;
using Shared;

namespace AutomatonRoamba
{
    public class AutomatonRoambaSettings : BuddyBaseConfig<AutomatonRoambaSettings>
    {
        public BuddyCoreConfig CoreConfig;

        [JsonIgnore]
        public override string FileName => "AutomatonRoambaSettings";

        [JsonIgnore]
        public override AutomatonRoambaSettings LoadDefaults => new AutomatonRoambaSettings();

        [JsonIgnore]
        public ConfigEditorConfig ConfigEditorConfig;

        [JsonIgnore]
        public PathEditorConfig PathEditorConfig;

        public bool SyncPath;

        public bool SyncConfig;

        private string _pathEditorPath = "";
        public string PathEditorPath
        {
            get => _pathEditorPath;
            set
            {
                _pathEditorPath = value;

                if (!string.IsNullOrEmpty(value))
                {
                    PathEditorConfig?.SPath?.Delete();
                    PathEditorConfig = PathEditorConfig.Load(value);
                }
            }
        }


        private string _configEditorPath = "";
        public string ConfigEditorPath
        {
            get => _configEditorPath;
            set
            {
                _configEditorPath = value;

                if (!string.IsNullOrEmpty(value))
                {
                    ConfigEditorConfig = ConfigEditorConfig.Load(value);
                }
            }
        }

        public Vector2 WindowCoords;

        public AutomatonRoambaSettings()
        {
            CoreConfig = new BuddyCoreConfig();
            PathEditorPath = "";
            ConfigEditorPath = "";
            WindowCoords = Vector2.Zero;
            SyncPath = false;
            SyncConfig = false;
        }
    }
}
