using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.IO;
using Path = System.IO.Path;

namespace AutomatonRoamba
{
    public class PathEditorInitView : RoambaView
    {
        private ComboBox _pathName;
        private PathEditorConfig _config;
        private string _path;
        public Action<string, bool> OnConfigChange;

        public PathEditorInitView(PathEditorConfig config, string pathEditorPath) : base(FilePath.TemplateEditorView)
        {
            _config = config;
            _path = pathEditorPath;

            try
            {
                if (Root.FindChild("AllFiles", out _pathName))
                {
                    if (!string.IsNullOrEmpty(_path))
                        _pathName.SetText(Path.GetFileNameWithoutExtension(_path));

                    var allFiles = Directory.GetFiles(FilePath.PathFolderPath);

                    for (int i = 0; i < allFiles.Length; i++)
                        _pathName.AppendItem(i + 1, Path.GetFileNameWithoutExtension(allFiles[i]));
                }

                if (Root.FindChild("Title", out TextView title))
                {
                    title.Text = "Roamba Path Loader";
                }

                if (Root.FindChild("CreateNew", out Button createNewPath))
                {
                    createNewPath.Clicked = OnCreateNewPathClick;
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
            if (Load(out _, out _))
            {
                AutomatonRoamba.PathEditorWindow.Close();
            }
        }

        private void OnEditFileClick(object sender, ButtonBase e)
        {
            if (Load(out string fullPath, out PathEditorConfig pathConfig))
            {
                AutomatonRoamba.PathEditorWindow.LoadMainView();
                AutomatonRoamba.PathEditorWindow.Window.MoveToCenter();
                AutomatonRoamba.PathEditorWindow.MainView.SetData(pathConfig, fullPath, fullPath);
            }
        }

        private bool Load(out string fullPath, out PathEditorConfig pathConfig)
        {
            fullPath = FilePath.PathFolderPath + "\\" + _pathName.GetText() + ".json";
            pathConfig = null;

            if (!File.Exists(fullPath))
            {
                AutomatonRoamba.Log.Information($"File not found '{fullPath}'");
                return false;
            }

            OnConfigChange?.Invoke(fullPath, true);

            pathConfig = PathEditorConfig.Load(fullPath);
            var pf = pathConfig.SPath.PlayfieldId;
            pathConfig.SPath.Delete();

            if (pf != Playfield.ModelIdentity.Instance)
            {
                AutomatonRoamba.Log.Information($"Cannot edit path saved for a different playfield.");
                return false;
            }

            AutomatonRoamba.MainWindow.UpdateActivePath(_pathName.GetText());
            return true;
        }

        private void OnCreateNewPathClick(object sender, ButtonBase e)
        {
            var fullPath = string.Empty;
            _config?.SPath?.Delete();
            AutomatonRoamba.PathEditorWindow.LoadMainView();
            AutomatonRoamba.PathEditorWindow.Window.MoveToCenter();
            var config = new PathEditorConfig();
            config.SPath = SPath.Create();
            config.Rules = new TargetingRules
            {
                IgnoredNames = { "IgnoredLeet1", "IgnoredLeet2", "IgnoredLeet3" },
                PriorityNames = { "PriorityLeet1", "PriorityLeet2", "PriorityLeet3" }
            };

            OnConfigChange?.Invoke(fullPath, false);

            AutomatonRoamba.PathEditorWindow.MainView.SetData(config, fullPath, FilePath.PathFolderPath + "\\" + _pathName.GetText() + ".json");
        }
    }
}