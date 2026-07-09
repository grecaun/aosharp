using AOSharp.Core;
using AOSharp.Core.UI;
using System;

namespace ProfessionHandler.Trader
{
    public class Main : AOPluginEntry
    {
        public override void Run()
        {
            try
            {
                if (Game.IsNewEngine)
                {
                    Chat.WriteLine("Does not work on this engine!");
                    return;
                }

                Generic.Combat.ProfessionHandler.Set(new TraderProfessionHandler(PluginDirectory));
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }
    }
}
