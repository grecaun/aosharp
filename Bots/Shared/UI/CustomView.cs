using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;

namespace Buddy.Shared.UI
{
    public abstract class CustomView
    {
        public View Root;
        public string XmlPath;

        public CustomView(string path)
        {
            Root = View.CreateFromXml(path);
            XmlPath = path;
        }
    }
}