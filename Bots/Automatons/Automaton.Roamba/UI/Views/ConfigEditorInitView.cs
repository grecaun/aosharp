using AOSharp.Core.UI;
using System;
using System.IO;
using Path = System.IO.Path;

namespace AutomatonRoamba
{
    public class ConfigEditorInitView : RoambaView
    {
        private ComboBox _pathName;
        private ConfigEditorConfig _configEditorConfig;
        private string _path;
        public Action<string, bool> OnConfigChange;

        public ConfigEditorInitView(ConfigEditorConfig config, string path) : base(FilePath.TemplateEditorView)
        {
            _configEditorConfig = config;
            _path = path;

            try
            {
                if (Root.FindChild("AllFiles", out _pathName))
                {
                    if (!string.IsNullOrEmpty(_path))
                        _pathName.SetText(Path.GetFileNameWithoutExtension(_path));

                    var allFiles = Directory.GetFiles(FilePath.ConfigFolderPath);

                    for (int i = 0; i < allFiles.Length; i++)
                        _pathName.AppendItem(i + 1, Path.GetFileNameWithoutExtension(allFiles[i]));
                }

                if (Root.FindChild("Title", out TextView title))
                {
                    title.Text = "Roamba Config Loader";
                }

                if (Root.FindChild("CreateNew", out Button createNew))
                {
                    createNew.Clicked = OnCreateNewClick;
                }

                if (Root.FindChild("EditFile", out Button editFile))
                {
                    editFile.Clicked = OnEditFileClick;
                }

                if (Root.FindChild("LoadFile", out Button loadFile))
                {
                    loadFile.Clicked = OnLoadFileClick;
                }
            }
            catch (Exception ex)
            {
                AutomatonRoamba.Log.Warning(ex.Message);
            }
        }

        private void OnLoadFileClick(object sender, ButtonBase e)
        {
            if (Load(out _))
            {
                AutomatonRoamba.ConfigEditorWindow.Close();
            }
        }

        private bool Load(out string fullPath)
        {
            fullPath = string.Empty;

            if (_configEditorConfig == null)
            {
                AutomatonRoamba.Log.Information("No config selected");
                return false;
            }

            if (_path == string.Empty)
            {
                AutomatonRoamba.Log.Information("No config selected");
                return false;
            }

            fullPath = FilePath.ConfigFolderPath + "\\" + _pathName.GetText() + ".json";

            if (!File.Exists(fullPath))
            {
                AutomatonRoamba.Log.Information($"File not found '{fullPath}'");
                return false;
            }

            OnConfigChange?.Invoke(_path, false);

            AutomatonRoamba.MainWindow.UpdateActiveConfig(_pathName.GetText());
            return true;
        }

        private void OnEditFileClick(object sender, ButtonBase e)
        {
            if (Load(out string fullPath))
            {
                OnConfigChange?.Invoke(fullPath, true);
                AutomatonRoamba.ConfigEditorWindow.LoadMainView();
                AutomatonRoamba.ConfigEditorWindow.Window.MoveToCenter();
                AutomatonRoamba.ConfigEditorWindow.MainView.SetData(ConfigEditorConfig.Load(fullPath), fullPath);
            }
        }

        private void OnCreateNewClick(object sender, ButtonBase e)
        {
            var fullPath = string.Empty;
            OnConfigChange?.Invoke(fullPath, false);
            AutomatonRoamba.ConfigEditorWindow.LoadMainView();
            AutomatonRoamba.ConfigEditorWindow.Window.MoveToCenter();
            AutomatonRoamba.ConfigEditorWindow.MainView.SetData(new ConfigEditorConfig(), fullPath);
        }
    }
}