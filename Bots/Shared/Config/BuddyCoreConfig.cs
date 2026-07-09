using AOSharp.Common.GameData;
using Newtonsoft.Json;

namespace Shared
{
    public class BuddyCoreConfig
    {
        public byte ChannelId;
        public bool OnInjectEnable;

        public BuddyCoreConfig()
        {
            ChannelId = 69;
            OnInjectEnable = false;
        }
    }
}