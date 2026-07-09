using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;

namespace AutomatonRoamba
{
    public class ConfigEditorWindow : EditorWindow
    {
        public new ConfigEditorInitView InitView;
        public new ConfigEditorMainView MainView;

        public ConfigEditorWindow(string windowName, ConfigEditorInitView initView, ConfigEditorMainView mainView, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = (WindowFlags)6144) : base(windowName, FilePath.TemplateWindow, initView, mainView, windowStyle, flags)
        {
            InitView = initView;
            MainView = mainView;
        }
    }

    public class PathEditorWindow : EditorWindow
    {
        public new PathEditorInitView InitView;
        public new PathEditorMainView MainView;

        public PathEditorWindow(string windowName, PathEditorInitView initView, PathEditorMainView mainView, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = (WindowFlags)6144) : base(windowName, FilePath.TemplateWindow, initView, mainView, windowStyle, flags)
        {
            InitView = initView;
            MainView = mainView;
        }
    }

    public abstract class EditorWindow: AOSharpWindow
    {
        public RoambaView InitView;
        public RoambaView MainView;
        private View _root;

        public EditorWindow(string windowName, string windowPath, RoambaView initView, RoambaView mainView, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade)
            : base(windowName, windowPath, windowStyle, flags)
        {
            InitView = initView;
            MainView = mainView;
        }

        protected override void OnWindowCreating()
        {
            try
            {
                if (Window.FindView("Root", out _root))
                    LoadInitView();
            }
            catch (Exception ex)
            {
                AutomatonRoamba.Log.Warning(ex.Message);
            }
        }

        private void LoadInitView()
        {
            AddChild(InitView.Root);
        }

        public void LoadMainView()
        {
            RemoveChild(InitView.Root);
            AddChild(MainView.Root);
        }

        private void AddChild(View view)
        {
            _root.AddChild(view, true);
        }

        public void RemoveChild(View view)
        {
            _root.RemoveChild(view);
        }
    }
}