using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;

namespace Buddy.Shared.UI
{
    public abstract class BuddyBaseWindow : AOSharpWindow
    {
        public abstract string CoreViewRootName { get; }
        public abstract View InfoView { get; }
   
        public BuddyCoreView CoreSettingsView;
        public View Root;

        private string _infoWindowPath;
        private string _coreViewPath;

        public BuddyBaseWindow(string windowName, string baseWindowPath, string infoWindowPath, string coreViewPath, WindowStyle windowStyle = WindowStyle.Default, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade) : base(windowName, baseWindowPath, windowStyle, flags)
        {
            _coreViewPath = coreViewPath;
            _infoWindowPath = infoWindowPath;
        }

        protected override void OnWindowCreating()
        {
            Window.FindView(CoreViewRootName, out View coreViewRoot);
            CoreSettingsView = new BuddyCoreView(InfoView, _coreViewPath, _infoWindowPath);
            CoreSettingsView.OnCloseClick += OnCloseClick;
            coreViewRoot.AddChild(CoreSettingsView.Root, true);
        }

        private void OnCloseClick()
        {
            Window?.Close();    
        }
    }
}