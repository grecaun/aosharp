using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AutomatonKiteHill.IPCMessages;

namespace AutomatonKiteHill
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("Idle");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (Playfield.ModelIdentity.Instance != Constants.ElyID)
            {
                if (AutomatonKiteHill._settings["Enable"].AsBool())
                {
                    AutomatonKiteHill.IPCChannel.Broadcast(new StartStopIPCMessage() { IsStarting = false });
                    AutomatonKiteHill._settings["Enable"] = false;
                }
                return null;
            }

            if (AutomatonKiteHill.Tank == Identity.None || AutomatonKiteHill.Nt == Identity.None) return null;

            if (!AutomatonKiteHill.CanProceed()) return null;

            switch (DynelManager.LocalPlayer.Profession)
            {
                case Profession.NanoTechnician:
                    if (!Team.IsInTeam) return null;
                    break;
                case Profession.Enforcer:
                    var nt = DynelManager.Players.FirstOrDefault(tech => tech.Identity == AutomatonKiteHill.Nt);
                    if (nt == null) return null;
                    if (!nt.IsInTeam()) return null;
                    break;
            }

            switch (AutomatonKiteHill._settings["SideSelection"].AsInt32())
            {
                case 0://Beach
                    if (Time.AONormalTime < AutomatonKiteHill._beachTimer) return null;
                    return AutomatonKiteHill.Pull_Beach;
                case 1://East
                    if (Time.AONormalTime < AutomatonKiteHill._eastTimer) return null;
                    return AutomatonKiteHill.Pull_East;
                case 2://West
                    if ( Time.AONormalTime < AutomatonKiteHill._westTimer) return null;
                    return AutomatonKiteHill.Pull_West;
            }

            return null;
        }

        public void Tick()
        {
        }

        public void OnStateExit()
        {
        }
    }
}
