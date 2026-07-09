using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using AOSharp.Core;
using System;
using System.Collections.Generic;
using AOSharp.Common.GameData.UI;

namespace ManagerInventory
{
    public static class SettingsController
    {
        private static List<Settings> settingsToSave = new List<Settings>();
        public static Dictionary<string, string> settingsWindows = new Dictionary<string, string>();
        private static bool IsCommandRegistered;

        public static MultiListView searchList;

        public static string NameValue = string.Empty;
        public static int ItemIdValue = 0;
        public static int MinQlValue = 0;
        public static int MaxQlValue = 0;

        public static Window settingsWindow;

        public static void RegisterCharacters(Settings settings)
        {
            RegisterChatCommandIfNotRegistered();
            settingsToSave.Add(settings);
        }

        public static void RegisterSettingsWindow(string settingsName, string settingsWindowPath, Settings settings)
        {
            RegisterChatCommandIfNotRegistered();
            settingsWindows[settingsName] = settingsWindowPath;
            settingsToSave.Add(settings);
        }

        public static void RegisterSettings(Settings settings)
        {
            RegisterChatCommandIfNotRegistered();
            settingsToSave.Add(settings);
        }

        public static void CleanUp()
        {
            settingsToSave.ForEach(settings => settings.Save());
        }

        private static void RegisterChatCommandIfNotRegistered()
        {
            if (!IsCommandRegistered)
            {
                Chat.RegisterCommand("ManagerInventory", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    try
                    {
                        settingsWindow = Window.Create(new Rect(50, 50, 300, 300), "Inventory Manager", "Settings", WindowStyle.Default, WindowFlags.AutoScale);

                        if (settingsWindow != null && !settingsWindow.IsVisible)
                        {
                            AppendSettingsTab("Inventory Manager", settingsWindow);

                            settingsWindow.FindView("ScrollListRoot", out MultiListView mlv);
                            settingsWindow.FindView("tivminql", out TextInputView tivminql);
                            settingsWindow.FindView("tivmaxql", out TextInputView tivmaxql);
                            tivminql.Text = "1";
                            tivmaxql.Text = "500";
                            mlv.DeleteAllChildren();
                            int iEntry = 0;
                            foreach (Rule r in ManagerInventory.Rules)
                            {
                                View entry = View.CreateFromXml(ManagerInventory.PluginDir + "\\UI\\ItemEntry.xml");
                                entry.FindChild("ItemName", out TextView tx);

                                tx.Text = (iEntry + 1).ToString() + " - " + " - [" + r.Lql.PadLeft(3, ' ') + "-" + r.Hql.PadLeft(3, ' ') + "  ] - " + r.Name;

                                mlv.AddChild(entry, false);
                                iEntry++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Chat.WriteLine(e);
                    }
                });

                IsCommandRegistered = true;
            }
        }

        public static void AppendSettingsTab(String settingsName, Window testWindow)
        {
            String settingsWindowXmlPath = settingsWindows[settingsName];
            View settingsView = View.CreateFromXml(settingsWindowXmlPath);

            if (settingsView != null)
            {
                testWindow.AppendTab(settingsName, settingsView);
                testWindow.Show(true);
            }
            else
            {
                Chat.WriteLine($"{settingsWindows[settingsName]}");
                Chat.WriteLine("Failed to load settings schema from " + settingsWindowXmlPath);
            }
        }
    }
}
