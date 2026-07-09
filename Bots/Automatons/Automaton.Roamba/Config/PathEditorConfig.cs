using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AutomatonRoamba
{
    public class PathEditorConfig
    {
        [JsonProperty]
        public SPath SPath;
        public TargetingRules Rules = new TargetingRules();

        public static PathEditorConfig Load(string path)
        {
            PathEditorConfig config;

            try
            {
                config = JsonConvert.DeserializeObject<PathEditorConfig>(File.ReadAllText(path));
                var newPath = SPath.Create();
                newPath.Waypoints = config.SPath.Waypoints;
                newPath.Name = config.SPath.Name;
                newPath.IsLooping = config.SPath.IsLooping;
                newPath.IsReversed = config.SPath.IsReversed;
                newPath.IsLocked = true;
                newPath.ShouldDraw = config.SPath.ShouldDraw;
                newPath.PlayfieldId = config.SPath.PlayfieldId;
                newPath.Id = config.SPath.Id;
                config.SPath = newPath;
            }
            catch
            {
                config = new PathEditorConfig();
            }
            return config;
        }

        public void Save(string savePath)
        {
            try
            {
                File.WriteAllText(savePath, JsonConvert.SerializeObject(this, Formatting.Indented));
                AutomatonRoamba.Log.Information($"Path Editor Config file saved");
            }
            catch (Exception ex)
            {
                AutomatonRoamba.Log.Warning(ex.Message);
            }
        }
    }
}