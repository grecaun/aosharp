using System.IO;

namespace AutomatonRoamba
{
    public class FilePath
    {
        public static string ViewsRootDir;
        public static string WindowsRootDir;

        public static string MainWindow => WindowsRootDir + "\\MainWindow.xml";
        public static string TemplateWindow => WindowsRootDir + "\\TemplateWindow.xml";

        public static string BuddyCoreView => ViewsRootDir + "\\BuddyCoreView.xml";
        public static string ConfigEditorMainView => ViewsRootDir + "\\ConfigEditorMainView.xml";
        public static string TemplateEditorView => ViewsRootDir + "\\TemplateEditorView.xml";
        public static string PathEditorMainView => ViewsRootDir + "\\PathEditorMainView.xml";
        public static string InfoView => ViewsRootDir + "\\InfoView.xml";
        public static string ConfigFolderPath => $"{CommonParameters.PluginDataPath}\\Config";
        public static string PathFolderPath => $"{CommonParameters.PluginDataPath}\\RoamPath";
        public static void Set(string pluginDir)
        {
            ViewsRootDir = $"{pluginDir}\\UI\\Views";
            WindowsRootDir = $"{pluginDir}\\UI\\Windows";
        }
    }
}
