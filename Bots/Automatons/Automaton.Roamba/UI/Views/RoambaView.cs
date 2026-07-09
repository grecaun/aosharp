using AOSharp.Common.GameData.UI;
using AOSharp.Core.UI;
using System;

namespace AutomatonRoamba
{
    public class RoambaView
    {
        public View Root;

        public RoambaView(string viewPath)
        {
            Root = View.CreateFromXml(viewPath);
        }
    }
}