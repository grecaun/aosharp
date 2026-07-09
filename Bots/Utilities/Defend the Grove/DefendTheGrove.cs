using System;
using System.Collections.Generic;
using System.Reflection;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using Shared;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace DefendTheGrove
{
    public class DefendTheGrove : AOPluginEntry
    {

        const string PluginName = "DefendTheGrove";
        private string Version_Number = "0.0.1";

        private Settings _settings;

        Window _settingsWindow;

        private static List<Settings> _settingsToSave = new List<Settings>();
        private static List<string> ErrorMessages = new List<string>();

        public static string EnableString = "Enable";

        private int WaveCounter = 0;

        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                _settings = new Settings(PluginName);

                _settings.AddVariable("Enable", false);
                _settings["Enable"] = false;
                EnableString = _settings["Enable"].AsBool() ? "Disable" : "Enable";

                _settings.AddVariable("MainWindowTopLeftX", 50f);
                _settings.AddVariable("MainWindowTopLeftY", 50f);

                _settingsToSave.Add(_settings);


                UIController.WindowDeleted += Windowclosed;
                Network.N3MessageSent += N3MessageSent;

                MainWindow();

                Chat.WriteLine($"{PluginName} Loaded!");

            }
            catch (Exception ex)
            {
                ErrorCatch(ex);
            }
        }

        public override void Teardown()
        {
            Save();
            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;
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
            EnableString = "Enable";

            if (_settingsWindow?.IsValid == true && _settingsWindow.FindView("Enable_Disable_Button", out Button enableButton))
                enableButton.SetLabel(EnableString);

            Save();

            UIController.WindowDeleted -= Windowclosed;
            Game.OnUpdate -= OnUpdate;
            Network.N3MessageSent -= N3MessageSent;

            return;
        }

        private void Window_Closed_helper()
        {
            if (_settingsWindow?.IsValid == true)
            {
                Rect frame = _settingsWindow.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;

                Save();
            }
        }

        private void OnUpdate(object sender, float e)
        {
            if (Game.IsZoning) return;

            if (_settingsWindow?.IsValid == true)
            {
                if (_settingsWindow.FindView("WaveCounter", out TextView counter))
                {
                    if (counter.Text != WaveCounter.ToString())
                        counter.Text = $"Waves Completed = {WaveCounter}";
                }
            }
        }

        private void MainWindow()
        {
            if (_settingsWindow?.IsValid == true)
            {
                Window_Closed_helper();

                _settingsWindow.Close();
                _settingsWindow = null;
                return;
            }

            _settingsWindow = Window.CreateFromXml(PluginName, PluginDirectory + "\\UI\\DefendTheGroveMainUI.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            _settingsWindow.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

            if (_settingsWindow.FindView("WaveCounter", out TextView counter))
                counter.Text = $"Waves Completed = {WaveCounter}";

            if (_settingsWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);

            if (_settingsWindow.FindView("VersionNumber", out TextView version))
                version.Text = $"Version {Version_Number}";

            _settingsWindow.Show(true);
        }

        public void Save()
        {
            _settingsToSave.ForEach(settings => settings.Save());
        }

        private void ErrorCatch(Exception ex)
        {
            var output = ex.Message + Environment.NewLine + "   at " + ex.TargetSite?.DeclaringType?.FullName + "." + ex.TargetSite?.Name;

            if (!ErrorMessages.Contains(output))
                ErrorMessages.Add(output);

            if (_settingsWindow != null && _settingsWindow.IsValid && _settingsWindow.FindView("Errors", out View errorView))
                PopulateErrorView(errorView);
        }


        private void PopulateErrorView(View errorView)
        {
            errorView.DeleteAllChildren();

            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                foreach (var error in ErrorMessages)
                {
                    var parts = error.Split(new[] { "   at " }, StringSplitOptions.None);

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

    }
}
