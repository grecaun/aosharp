using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.Messages;
using SmokeLounge.AOtomation.Messaging.Messages.ChatMessages;

namespace Teams
{
    public partial class Main : AOPluginEntry
    {

        public override void Run()
        {
            Network.ChatMessageReceived += ChatMessageReceived;

            Chat.WriteLine("Teams Loaded!");
            Chat.WriteLine("Say '#teamup' in a group chat or 'teamup' in a tell to get others to whisper you to join your team.");
        }

        private void ChatMessageReceived(object sender, ChatMessageBody e)
        {
            if (e is GroupMsgMessage msg)
            {
                if (msg.Text.Equals("#teamup", System.StringComparison.OrdinalIgnoreCase) && DynelManager.LocalPlayer.Identity.Instance != msg.SenderId)
                {
                    Chat.SendPrivateMessage(msg.SenderId, "invite me");
                }
            }
            else if (e is PrivateMsgMessage priv)
            {
                if (priv.Text.Equals("teamup", System.StringComparison.OrdinalIgnoreCase))
                {
                    Chat.SendPrivateMessage(priv.Sender, "invite me");
                }
                else if (priv.Text.Equals("invite me", System.StringComparison.OrdinalIgnoreCase) && DynelManager.LocalPlayer.Identity.Instance != priv.Sender)
                {
                    Team.Invite(new Identity((int)priv.Sender));
                }
            }
        }
    }
}