using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Common.GameData.UI;
using AOSharp.Core;
using AOSharp.Core.UI;
using Newtonsoft.Json;

namespace Boss_Logger
{
    public class BossLogger : AOPluginEntry
    {
        Settings _settings;
        private readonly List<Settings> settingsToSave = new List<Settings>();
        const string PluginName = "BossLogger";
        Window Main;

        string path;

        Dictionary<string, List<Info>> Bosses = new Dictionary<string, List<Info>>();

        public class Info
        {
            public string Name = "";
            public string HP = "";
        }

        public override void Run()
        {

            if (Game.IsNewEngine)
            {
                Chat.WriteLine("Does not work on this engine!");
                return;
            }

            _settings = new Settings(PluginName);

            path = Path.Combine(PluginDataDirectory.FullName, "Bosses.json");

            if (File.Exists(path))
                Bosses = JsonConvert.DeserializeObject<Dictionary<string, List<Info>>>(File.ReadAllText(path));
            else
                File.WriteAllText(path, JsonConvert.SerializeObject(Bosses));

            _settings.AddVariable("MainWindowTopLeftX", 50f);
            _settings.AddVariable("MainWindowTopLeftY", 50f);

            settingsToSave.Add(_settings);

            UIController.WindowDeleted += Windowclosed;

            Chat.RegisterCommand("logger", Main_Window_Chat_Command_Received);

            Chat.WriteLine($"{PluginName} Loaded, /logger to reopen ui");
            Main_Window();
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

        private void Main_Window_Chat_Command_Received(string arg1, string[] arg2, ChatWindow window)
        {
            Main_Window();
        }

        private void Main_Window()
        {
            if (Main?.IsValid == true)
            {
                Window_Closed_helper();
                Main.Close();
                Main = null;
                return;
            }

            Main = Window.CreateFromXml(PluginName, PluginDirectory + "\\BossLoggerMainWindow.xml", windowStyle: WindowStyle.Default, windowFlags: WindowFlags.AutoScale | WindowFlags.NoFade);
            Main.MoveTo(_settings["MainWindowTopLeftX"].AsFloat(), _settings["MainWindowTopLeftY"].AsFloat());

            if (Main.FindView("Add", out Button addButton))
                addButton.Clicked = Add_Button_Clicked;

            if (Main.FindView("Remove", out Button removeButton))
                removeButton.Clicked = Remove_Button_Clicked;

            if (Main.FindView("Open", out Button openButton))
                openButton.Clicked = Open_Button_Clicked;

            Main.Show(true);
        }

        private void Add_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                Dynel dynel = Targeting.Target;
                if (dynel == null)
                    return;

                SimpleChar target = new SimpleChar(dynel);

                string pf = Playfield.ModelIdentity.Instance.ToString();

                if (!Bosses.ContainsKey(pf))
                    Bosses[pf] = new List<Info>();

                if (Bosses[pf].Any(x => x.Name == target.Name))
                    return;

                Bosses[pf].Add(new Info
                {
                    Name = target.Name,
                    HP = target.MaxHealth.ToString()
                });

                Chat.WriteLine($"Added: Name {target.Name}, HP {target.MaxHealth} to {pf}");

                File.WriteAllText(path, JsonConvert.SerializeObject(Bosses));
            }
            catch (Exception ex) { Chat.WriteLine(ex); }
        }

        private void Remove_Button_Clicked(object sender, ButtonBase e)
        {
            try
            {
                Dynel dynel = Targeting.Target;
                if (dynel == null)
                    return;

                SimpleChar target = new SimpleChar(dynel);

                string pf = Playfield.ModelIdentity.Instance.ToString();

                if (!Bosses.ContainsKey(pf)) return;

                var info = Bosses[pf].FirstOrDefault(x => x.Name == target.Name);

                if (info == null) return;

                Bosses[pf].Remove(info);

                Chat.WriteLine($"Removed: Name {target.Name}, HP {target.MaxHealth} from {pf}");

                File.WriteAllText(path, JsonConvert.SerializeObject(Bosses));
            }
            catch (Exception ex) { Chat.WriteLine(ex); }
        }

        private void Open_Button_Clicked(object sender, ButtonBase e)
        {
            Process.Start("explorer.exe", PluginDataDirectory.FullName);
        }

        private void Window_Closed_helper()
        {
            if (Main?.IsValid == true)
            {
                Rect frame = Main.GetFrame();
                _settings["MainWindowTopLeftX"] = frame.MinX;
                _settings["MainWindowTopLeftY"] = frame.MinY;
                Save();
            }
        }

        public void Save()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }
    }
}
