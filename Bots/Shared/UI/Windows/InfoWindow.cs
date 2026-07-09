using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;

namespace Buddy.Shared.UI
{
    public class InfoWindow : AOSharpWindow
    {
        public View Root;
        private View _infoView;
        private View _infoRoot;

        public InfoWindow(string windowName, string path, View infoView, WindowStyle windowStyle = WindowStyle.Popup, WindowFlags flags = WindowFlags.AutoScale | WindowFlags.NoFade) : base(windowName, path, windowStyle, flags)
        {
            _infoView = infoView;
        }

        protected override void OnWindowCreating()
        {
            if (Window.FindView("InfoRoot", out  _infoRoot))
            {
                _infoRoot.AddChild(_infoView, true);
            }

            if (Window.FindView("Close", out Button closeButton))
            {
                closeButton.Clicked += OnCloseClick;
            }
            Window.MoveToCenter();
        }

        private void OnCloseClick(object sender, ButtonBase e)
        {
            _infoRoot.RemoveChild(_infoView);
            Close();
        }
    }
}