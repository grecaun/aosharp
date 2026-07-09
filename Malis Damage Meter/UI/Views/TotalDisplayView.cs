using AOSharp.Common.GameData;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static MalisDamageMeter.MainWindow;

namespace MalisDamageMeter
{
    public class TotalDisplayView
    {
        private bool _active;
        private View _mainRoot;
        public View Root;
        public TextView TotalValue;

        public TotalDisplayView(View mainRootView)
        {
            Root = View.CreateFromXml($"{DamageMeter.PluginDir}\\UI\\Views\\TotalDisplayView.xml");
            _mainRoot = mainRootView;
            if (Root.FindChild("TotalValue", out TotalValue)) { }
            if (Root.FindChild("Background", out BitmapView background)) { background.SetBitmap(Textures.Background); }
        }

        public void Hide()
        {
            if (!_active)
                return;

            _mainRoot.RemoveChild(Root);
            _mainRoot.FitToContents();
            _active = false;
        }

        public void Show()
        {
            if (_active)
                return;

            _mainRoot.AddChild(Root, true);
            _mainRoot.FitToContents();
            _active = true;
        }
    }
}