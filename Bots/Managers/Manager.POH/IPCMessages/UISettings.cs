using AOSharp.Core.IPC;
using SmokeLounge.AOtomation.Messaging.Serialization.MappingAttributes;

namespace Manager.POH
{
    [AoContract((int)IPCOpcode.UISettings)]
    internal class UISettings : IPCMessage
    {
        [AoMember(0)] public bool BroadcastSettings { get; set; }
       
        [AoMember(1)] public bool Alters { get; set; }
    }
}
