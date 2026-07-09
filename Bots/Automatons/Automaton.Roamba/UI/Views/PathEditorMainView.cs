using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonRoamba
{
    public class PathEditorMainView : RoambaView
    {
        private TextView _pointsText;
        private TextView _isLoopingText;
        private TextView _isReversedText;
        private TextView _isLockedText;
        private TextView _playfieldText;
        private TextView _priorityNames;
        private TextView _ignoredNames;
        private TextInputView _fileName;
        private PathEditorConfig _config;
        public Action<string, bool> OnSave;
        private string _oldPath;

        public PathEditorMainView() : base(FilePath.PathEditorMainView)
        {
            try
            {
                if (Root.FindChild("CoreInfo", out Button coreInfo))
                {
                    SetGfx(coreInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    coreInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("\n" +
                        "Add Point - Adds a point at players location\n" +
                        "Remove Point - Remove closest point (green circle)\n" +
                        "Reverse Path - Reverses the path\n" +
                        "Split Path - Splits the path at closest intersection with player\n" +
                        "Test / Stop - Test run / stop navigating\n\n");
                    };
                }

                if (Root.FindChild("EditingInfo", out Button editingInfo))
                {
                    SetGfx(editingInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    editingInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("\n" +
                        "Pickup Point - Picks up closest point (green circle)\n" +
                        "Place Point - Places picked up point");
                    };
                }

                if (Root.FindChild("MiscInfo", out Button miscInfo))
                {
                    SetGfx(miscInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    miscInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("\n" +
                        "Toggle Lock - Stops the last point from following the player\n" +
                        "Toggle Loop - Makes the path loop\n" +
                        "Clear Path - Clears entire path");
                    };
                }

                if (Root.FindChild("InfoInfo", out Button infoInfo))
                {
                    SetGfx(infoInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    infoInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("Shows basic path info");
                    };
                }

                if (Root.FindChild("IgnoredNamesInfo", out Button ignoredNamesInfo))
                {
                    SetGfx(ignoredNamesInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    ignoredNamesInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("Names to ignore when picking combat targets");
                    };
                }

                if (Root.FindChild("PriorityNamesInfo", out Button priorityNamesInfo))
                {
                    SetGfx(priorityNamesInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    priorityNamesInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("Names to prioritize when picking combat targets");
                    };
                }

                if (Root.FindChild("ExportSaveInfo", out Button exportSaveInfo))
                {
                    SetGfx(exportSaveInfo, "GFX_GUI_CHECKLIST_MARKER_UNKNOWN");
                    exportSaveInfo.Clicked += (sender, e) =>
                    {
                        Chat.WriteLine("Save or export your path. Paths can be accessed across multiple characters.");
                    };
                }

                if (Root.FindChild("AddPoint", out Button addPoint)) { addPoint.Clicked = AddPointClick; }
                if (Root.FindChild("RemovePoint", out Button removePoint)) { removePoint.Clicked = RemovePointClick; }
                if (Root.FindChild("ReversePath", out Button reversePath)) { reversePath.Clicked = ReversePathClick; }
                if (Root.FindChild("RunPath", out Button runPath)) { runPath.Clicked = RunPathClicked; }
                if (Root.FindChild("PickupPoint", out Button pickupPoint)) { pickupPoint.Clicked = PickupPointClick; }
                if (Root.FindChild("PlacePoint", out Button placePoint)) { placePoint.Clicked = PlacePointClick; }
                if (Root.FindChild("SplitPath", out Button splitPath)) { splitPath.Clicked = SplitPointClick; }
                if (Root.FindChild("ToggleLock", out Button toggleLock)) { toggleLock.Clicked = ToggleLockClick; }
                if (Root.FindChild("ToggleLoop", out Button toggleLoop)) { toggleLoop.Clicked = ToggleLoopClick; }
                if (Root.FindChild("ClearPath", out Button clearPath)) { clearPath.Clicked = ClearClick; }
                if (Root.FindChild("Points", out _pointsText)) { }
                if (Root.FindChild("IsLooping", out _isLoopingText)) { }
                if (Root.FindChild("IsReversed", out _isReversedText)) { }
                if (Root.FindChild("IsLocked", out _isLockedText)) { }
                if (Root.FindChild("Playfield", out _playfieldText)) { }
                if (Root.FindChild("IgnoredNames", out _ignoredNames)) { }
                if (Root.FindChild("PriorityNames", out _priorityNames)) { }
                if (Root.FindChild("Save", out Button save)) { save.Clicked += SaveClick; }
                if (Root.FindChild("Close", out Button close)) { close.Clicked += CloseClick; }
                if (Root.FindChild("FileName", out _fileName)) { }
            }
            catch (Exception ex)
            {
                AutomatonRoamba.Log.Warning(ex.Message);
            }
        }
        private void SetGfx(Button button, string gfx)
        {
            button.SetGfx(AOSharp.Common.GameData.UI.ButtonState.Raised, gfx);
            button.SetGfx(AOSharp.Common.GameData.UI.ButtonState.Hover, gfx);
            button.SetGfx(AOSharp.Common.GameData.UI.ButtonState.Pressed, gfx);
        }

        private void CloseClick(object sender, ButtonBase e)
        {
            var currentPath = $"{FilePath.PathFolderPath}\\{_fileName.Text}.json";
            _config.SPath?.Delete();
            _config.SPath = null;
            var path = _oldPath != currentPath ? _oldPath : currentPath;
            OnSave?.Invoke(path, true);
            AutomatonRoamba.PathEditorWindow.Close();
        }

        public string GetIgnoredText()
        {
            return _ignoredNames.Text;
        }

        public string GetPriorityText()
        {
            return _priorityNames.Text;
        }

        private void SaveClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath?.Delete();

            if (_config.SPath?.Waypoints.Count == 0)
            {
                AutomatonRoamba.Log.Warning("Cannot save path with 0 waypoints");
                return;
            }

            if (string.IsNullOrEmpty(_fileName.Text))
            {
                AutomatonRoamba.Log.Warning("Please provide a file name");
                return;
            }

            var savePath = $"{FilePath.PathFolderPath}\\{_fileName.Text}.json";

            GetData().Save(savePath);
            OnSave?.Invoke(savePath, true);
            AutomatonRoamba.MainWindow.UpdateActivePath(_fileName.Text);
            AutomatonRoamba.PathEditorWindow.Close();
        }

        public PathEditorConfig GetData()
        {
            if (!string.IsNullOrEmpty(_ignoredNames.Text))
                _config.Rules.IgnoredNames = _ignoredNames.Text.Split('\n').Select(x => x.Trim()).ToList();
         
            if (!string.IsNullOrEmpty(_priorityNames.Text))
                _config.Rules.PriorityNames = _priorityNames.Text.Split('\n').Select(x => x.Trim()).ToList();

            return _config;
        }

        public void SetData(PathEditorConfig config, string newPath, string oldPath)
        {
            _oldPath = oldPath;
            _config = config;

            _pointsText.Text = config.SPath?.Waypoints.Count.ToString();
            _isLoopingText.Text = config.SPath?.IsLooping.ToString();
            _isReversedText.Text = config.SPath?.IsReversed.ToString();
            _isLockedText.Text = config.SPath?.IsLocked.ToString();
            _ignoredNames.Text = string.Join("\n", config.Rules.IgnoredNames);
            _priorityNames.Text = string.Join("\n", config.Rules.PriorityNames);
            _playfieldText.Text = Playfield.ModelIdentity.Instance.ToString();

            SetPlayfieldTextColor(config.SPath?.PlayfieldId == Playfield.ModelIdentity.Instance);

            if (string.IsNullOrEmpty(newPath) || config.SPath?.PlayfieldId != Playfield.ModelIdentity.Instance || config.SPath?.Waypoints.Count() == 0)
                _fileName.Text = "";
            else
                _fileName.Text = System.IO.Path.GetFileNameWithoutExtension(newPath);
        }

        private void SetPlayfieldTextColor(bool isSameAsPlayer)
        {
            uint color = isSameAsPlayer ? (uint)0x0022FF22 : (uint)0x00FF2222;
            _playfieldText.SetColor(color);
        }

        private bool PathPlayfieldCheck()
        {
            var pfCheck = _config.SPath.PlayfieldId == Playfield.ModelIdentity.Instance;

            SetPlayfieldTextColor(pfCheck);

            if (!pfCheck)
                AutomatonRoamba.Log.Information("Cannot edit path created for a different playfield.");

            return pfCheck;
        }

        private void AddPointClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            if (_config.SPath.Waypoints.Count < 2 && _config.SPath.IsLooping)
            {
                _config.SPath.ToggleLoop();
                _isLoopingText.Text = _config.SPath.IsLooping.ToString();
            }

            _config.SPath.AddPoint();
            _pointsText.Text = _config.SPath.Waypoints.Count.ToString();
        }

        private void RemovePointClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath.RemovePoint(); 
            _pointsText.Text = _config.SPath.Waypoints.Count.ToString();
        }

        private void PickupPointClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath.PickupPoint();
        }

        private void PlacePointClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath.PlacePoint();
        }

        private void RunPathClicked(object sender, ButtonBase e)
        {
            if (_config.SPath == null)
                return;

            if (!PathPlayfieldCheck())
                return;

            if (SMovementController.IsNavigating())
            {
                SMovementController.Halt();
                return;
            }

            if (!SMovementController.IsLoaded())
                SMovementController.Set();

            AutomatonRoamba.SetPath(_config.SPath);
        }

        private void SplitPointClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _pointsText.Text = _config.SPath.Waypoints.Count.ToString();

            Vector3 playerPos = DynelManager.LocalPlayer.Position;
            int closestIndex = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < _config.SPath.Waypoints.Count - 1; i++)
            {
                Vector3 wp1 = _config.SPath.Waypoints[i];
                Vector3 wp2 = _config.SPath.Waypoints[i + 1];

                float distance = DistanceToLineSegment(playerPos, wp1, wp2);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                _config.SPath.SelectPoint(closestIndex);
                _config.SPath.SelectPoint(closestIndex + 1);
                _config.SPath.Split();
                _config.SPath.UnselectPoints();
            }
        }

        private float DistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            float lineLength = line.Magnitude;

            if (lineLength == 0)
                return Vector3.Distance(point, lineStart);

            float t = Math.Max(0, Math.Min(1, Vector3.Dot(point - lineStart, line) / (lineLength * lineLength)));
            Vector3 projection = lineStart + line * t;

            return Vector3.Distance(point, projection);
        }
        private void ToggleLockClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath.ToggleLock();
            _isLockedText.Text = _config.SPath.IsLocked.ToString();
        }

        private void ToggleLoopClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            if (_config.SPath.Waypoints.Count < 2)
                return;

            _config.SPath.ToggleLoop();
            _isLoopingText.Text = _config.SPath.IsLooping.ToString();
        }

        private void ClearClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath.Clear();
            _config.SPath.IsLocked = false;
        }

        private void ReversePathClick(object sender, ButtonBase e)
        {
            if (!PathPlayfieldCheck())
                return;

            _config.SPath.Reverse();
            _isReversedText.Text = _config.SPath.IsReversed.ToString();
        }
    }
}