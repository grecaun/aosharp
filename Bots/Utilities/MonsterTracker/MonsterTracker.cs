using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace MonsterTracker
{
    public class MonsterTracker : AOPluginEntry
    {
        private const string PluginName = "MonsterTracker";
        private string Version_Number = "0.0.1";
        private List<string> ErrorMessages = new List<string>();
        private Settings _settings;
        private string EnableString;

        Window settingsWindow;

        private List<Settings> _settingsToSave = new List<Settings>();

        private class MonsterData
        {
            public string Identity = "";
            public string Name = "";
            public string LVL = "";
            public string PF = "";
            public string Area = "";


            public MonsterData() { }

            public MonsterData(string id, string name, string lvl, string pf, string area)
            {
                this.Identity = id;
                this.Name = name;
                this.LVL = lvl;
                this.PF = pf;
                this.Area = area;
            }
        }

        private List<MonsterData> Monsters = new List<MonsterData>();

        private static string BasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AOSharp", PluginName);

        private Identity Monster = Identity.None;

        public override void Run()
        {
            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            Directory.CreateDirectory(BasePath);

            _settings = new Settings(PluginName);

            _settings.AddVariable("Enable", false);
            _settings["Enable"] = false;
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            _settings.AddVariable("follow", false);
            _settings["follow"] = false;

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            _settingsToSave.Add(_settings);

            Chat.RegisterCommand(PluginName, ManagerCommand);

            UIController.WindowDeleted += Windowclosed;
            Network.N3MessageSent += N3MessageSent;

            Chat.WriteLine($"{PluginName} Loaded!");
            Chat.WriteLine($"/{PluginName} for UI.");
            MainUI();
        }

        #region Chat Command

        private void ManagerCommand(string arg1, string[] arg2, ChatWindow window)
        {
            MainUI();
        }

        private void MainUI()
        {
            try
            {
                if (settingsWindow?.IsValid == true)
                {
                    Window_Closed_helper();

                    settingsWindow.Close();
                    settingsWindow = null;
                    return;
                }

                settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + $"\\UI\\MonsterTrackerSettingsWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
                settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

                if (settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                {
                    enableButton.SetLabel(EnableString);
                    enableButton.Clicked = Enable_Disable_Button_Clicked;
                }

                if (settingsWindow.FindView("FollowButton", out Button followButton))
                {
                    followButton.SetLabel("Follow Target");

                    followButton.Clicked = (s, e) =>
                    {
                        if (_settings["follow"].AsBool())
                        {
                            followButton.SetLabel("Follow Target");
                            MovementController.Instance.Halt();
                            DynelManager.LocalPlayer.MovementState = MovementState.Run;
                            _settings["follow"] = false;
                        }
                        else if (Targeting.Target == null)
                        {
                            Chat.WriteLine("Select target first");
                        }
                        else
                        {
                            followButton.SetLabel("Stop Follow");
                            Monster = Targeting.Target.Identity;
                            _settings["follow"] = true;
                        }
                    };
                }

                //if (settingsWindow.FindView("ScrollListView", out MultiListView _multiListView))
                //{
                //    _multiListView.DeleteAllChildren();

                //    if (Monsters != null && Monsters.Count > 0)
                //    {
                //        var row = View.CreateFromXml(PluginDirectory + "\\UI\\MonsterRow.xml");

                //        foreach (var monster in Monsters)
                //        {
                //            if (row.FindChild("MonsterRow", out TextView index))
                //                index.Text = monster.ToString();
                //            _multiListView.AddChild(row, false);
                //        }
                //    }
                //}

                if (settingsWindow.FindView("OpenFolder", out Button openButton))
                    openButton.Clicked = (s, e) => { Process.Start("explorer.exe", BasePath); };

                if (settingsWindow.FindView("Errors", out View errorView))
                    PopulateErrorView(errorView);

                if (settingsWindow.FindView("VersionNumber", out TextView version))
                    version.Text = $"Version {Version_Number}";

                settingsWindow.Show(true);

            }
            catch (Exception ex)
            {
                var output = ex.Message + Environment.NewLine + "   at " +
                             ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

                if (!ErrorMessages.Contains(output))
                    ErrorMessages.Add(output);

            }
        }

        #endregion

        #region Button Clicked

        private void Enable_Disable_Button_Clicked(object sender, ButtonBase e)
        {
            Helper_Enable();
        }

        #endregion

        #region Events

        public override void Teardown()
        {
            Save();
            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            Network.N3MessageReceived -= N3MessageReceived;
        }

        private void Windowclosed(object sender, Window e)
        {
            switch (e.Name)
            {
                case PluginName:
                    Window_Closed_helper();
                    break;
            }
        }

        private void N3MessageSent(object sender, N3Message e)
        {
            if (e.N3MessageType != N3MessageType.CharacterAction) return;
            var charAction = (CharacterActionMessage)e;
            if (charAction.Action != CharacterActionType.Logout) return;

            _settings["Enable"] = false;

            Save();

            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
            Network.N3MessageReceived -= N3MessageReceived;

            return;
        }

        private void N3MessageReceived(object sender, N3Message e)
        {
            switch (e.N3MessageType)
            {
                case N3MessageType.FollowTarget:
                    var followTargetMessage = (FollowTargetMessage)e;
                    if (!_settings["follow"].AsBool()) return;
                    if (Monster != followTargetMessage.Identity) return;
                    var targetid = DynelManager.Characters.FirstOrDefault(x => x.Identity == followTargetMessage.Identity);
                    if (followTargetMessage.Type != FollowTargetType.NpcPath) return;

                    if (!(followTargetMessage.Info is FollowTargetMessage.PathInfo pathInfo)) return;

                    var monsterData = Monsters.FirstOrDefault(m => m.Identity == Monster.ToString());

                    if (monsterData == null) return;

                    string playfieldFolder = System.IO.Path.Combine(BasePath, Playfield.Name);
                    string safeIdentity = string.Concat(monsterData.Identity.Where(c => !System.IO.Path.GetInvalidFileNameChars().Contains(c)));
                    string safeName = string.Concat(monsterData.Name.Where(c => !System.IO.Path.GetInvalidFileNameChars().Contains(c)));
                    string fileName = $"{safeIdentity} {safeName}.json";
                    string filePath = System.IO.Path.Combine(playfieldFolder, fileName);

                    if (!File.Exists(filePath)) return;

                    string json = File.ReadAllText(filePath);
                    JObject parsed = JObject.Parse(json);
                    var waypoints = parsed["Waypoints"]?.ToObject<List<Vector3>>() ?? new List<Vector3>();
                    bool waypointRepeated = false;

                    foreach (var waypoint in pathInfo.Waypoints)
                    {
                        if (waypoints.Contains(waypoint))
                        {
                            waypointRepeated = true;
                            break;
                        }

                        waypoints.Add(waypoint);
                        MovementController.Instance.SetDestination(waypoint);
                        if (DynelManager.LocalPlayer.Position.DistanceFrom(targetid.Position) < 3)
                        if (DynelManager.LocalPlayer.MovementState != targetid.MovementState)
                            DynelManager.LocalPlayer.MovementState = targetid.MovementState;
                    }

                    parsed["Waypoints"] = JToken.FromObject(waypoints);
                    parsed["Done"] = waypointRepeated;

                    if (waypointRepeated)
                    {
                        string newFileName = $"Done {fileName}";
                        string newFilePath = System.IO.Path.Combine(playfieldFolder, newFileName);

                        File.Move(filePath, newFilePath);
                        File.WriteAllText(newFilePath, parsed.ToString(Formatting.Indented));

                        DynelManager.LocalPlayer.MovementState = MovementState.Run;
                        MovementController.Instance.Halt();
                        _settings["follow"] = false;
                        Monsters.Remove(monsterData);
                        Monster = Identity.None;

                        if (settingsWindow != null && settingsWindow.IsVisible)
                        {
                            if (settingsWindow.FindView("ScrollListView", out MultiListView _multiListView))
                            {
                                _multiListView.DeleteAllChildren();

                                if (Monsters != null && Monsters.Count > 0)
                                {
                                    var row = View.CreateFromXml(PluginDirectory + "\\UI\\MonsterRow.xml");

                                    foreach (var monster in Monsters)
                                    {
                                        if (row.FindChild("MonsterRow", out TextView index))
                                            index.Text = $"{monster.Identity.ToString()}, {monster.Name}, {monster.LVL.ToString()}, {Playfield.ModelIdentity.Instance.ToString()}, {Playfield.Name}";
                                        _multiListView.AddChild(row, false);
                                    }
                                }
                            }

                            if (settingsWindow.FindView("FollowButton", out Button followButton))
                                followButton.SetLabel("Follow Target");
                        }
                    }
                    else
                    {
                        File.WriteAllText(filePath, parsed.ToString(Formatting.Indented));
                    }

                    break;
            }
        }
        private void OnUpdate(object sender, float e)
        {
            try
            {
                foreach (var monster in DynelManager.NPCs.Where(n => n.IsMoving && !n.IsPet))
                {
                    string safeIdentity = string.Concat(monster.Identity.ToString().Where(c => !System.IO.Path.GetInvalidFileNameChars().Contains(c)));
                    string safeName = string.Concat(monster.Name.Where(c => !System.IO.Path.GetInvalidFileNameChars().Contains(c)));
                    string doneFileName = $"Done {safeIdentity} {safeName}.json";
                    string doneFilePath = System.IO.Path.Combine(System.IO.Path.Combine(BasePath, Playfield.Name), doneFileName);

                    if (Monsters.Any(m => m.Identity == monster.Identity.ToString()) || File.Exists(doneFilePath)) continue;

                    string playfieldFolder = System.IO.Path.Combine(BasePath, Playfield.Name);
                    Directory.CreateDirectory(System.IO.Path.Combine(BasePath, Playfield.Name));

                    string fileName = $"{safeIdentity} {safeName}.json";
                    string filePath = System.IO.Path.Combine(playfieldFolder, fileName);

                    if (!File.Exists(filePath))
                    {
                        var data = new
                        {
                            Identity = monster.Identity.ToString(),
                            Name = monster.Name,
                            LVL = monster.Level.ToString(),
                            PF = Playfield.ModelIdentity.Instance.ToString(),
                            Area = Playfield.Name,
                            Waypoints = new List<Vector3>()
                        };

                        File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
                    }

                    Monsters.Add(new MonsterData(
                        monster.Identity.ToString(),
                        monster.Name,
                        monster.Level.ToString(),
                        Playfield.ModelIdentity.Instance.ToString(),
                        Playfield.Name));

                    if (settingsWindow != null && settingsWindow.IsVisible)
                    {
                        if (settingsWindow.FindView("ScrollListView", out MultiListView _multiListView))
                        {
                            var row = View.CreateFromXml(PluginDirectory + "\\UI\\MonsterRow.xml");

                            if (row.FindChild("MonsterRow", out TextView index))
                                index.Text = $"{monster.Identity.ToString()}, {monster.Name}, {monster.Level.ToString()}, {Playfield.ModelIdentity.Instance.ToString()}, {Playfield.Name}";
                            _multiListView.AddChild(row, false);
                        }
                    }
                }

                if (Monster == Identity.None)
                {
                    Targeting.SetTarget(DynelManager.NPCs.FirstOrDefault(n => n != null && Monsters.Any(m => m.Identity == n.Identity.ToString())));
                    AOSharp.Core.Debug.DrawLine(DynelManager.LocalPlayer.Position, Targeting.Target.Position, DebuggingColor.White);
                } 
            }
            catch (Exception ex)
            {
                var output = ex.Message + Environment.NewLine + "   at " +
                             ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

                if (!ErrorMessages.Contains(output))
                    ErrorMessages.Add(output);
            }
        }

        #endregion

        #region Helpers

        private void Save()
        {
            _settingsToSave.ForEach(settings => settings.Save());
        }

        private void Helper_Enable()
        {
            _settings["Enable"] = !_settings["Enable"].AsBool();
            EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

            if (settingsWindow?.IsValid == true && settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            if (_settings["Enable"].AsBool())
            {
                Chat.WriteLine($"{PluginName} enabled");
                Game.OnUpdate += OnUpdate;
                Network.N3MessageReceived += N3MessageReceived;
            }
            else
            {
                Chat.WriteLine($"{PluginName} disabled");
                Network.N3MessageReceived -= N3MessageReceived;
                Game.OnUpdate -= OnUpdate;
            }

            Save();
        }

        private void Window_Closed_helper()
        {
            if (settingsWindow?.IsValid == true)
            {
                Rect frame = settingsWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        public void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { " at " }, StringSplitOptions.None);

                    View xmlRoot = View.CreateFromXml($"{PluginDirectory}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                    xmlRoot.FindChild("TextLabel", out TextView labelView);
                    labelView.Text = parts[0];
                    labelView.SetColor(Color.Red);
                    errorView.AddChild(xmlRoot, true);

                    if (parts.Length > 1)
                    {
                        View methodRoot = View.CreateFromXml($"{PluginDirectory}\\UI\\HandlerMainWindow\\ErrorRow.xml");
                        methodRoot.FindChild("TextLabel", out TextView methodLabel);
                        methodLabel.Text = "at " + parts[1];
                        errorView.AddChild(methodRoot, true);
                    }
                }
            }
        }

        #endregion
    }
}
