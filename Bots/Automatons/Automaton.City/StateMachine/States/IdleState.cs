using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System.Linq;

namespace AutomatonCity
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("Idle");

            //if (DynelManager.LocalPlayer.Identity != AutomatonCity.Leader) return;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return null;
            if (Playfield.ModelIdentity.Instance == 152) return null;

            if (Playfield.IsDungeon)
            {
                if (DynelManager.LocalPlayer.Room.Name == "AI_bossroom")
                    return AutomatonCity.Boss_Room_State;
                else
                    return AutomatonCity.Path_State;
            }

            if (!AutomatonCity.validPlayfields.Contains(Playfield.ModelIdentity.Instance)) return null;
            //{
            //var shipentrance = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Door");
            var shipentrance = DynelManager.AllDynels.FirstOrDefault(c => c.Identity.Type == IdentityType.ACGEntrance);// c.Name == "Door"

            if (AutomatonCity._settings["Ship"].AsBool() && shipentrance != null)
                return AutomatonCity.Wait_For_Ship_State;
            else
            {
                if (DynelManager.LocalPlayer.Identity == AutomatonCity.Leader //&& !DynelManager.NPCs.Any(c => c.Health > 0) && !AutomatonCity.CityUnderAttack
                                                                              // && (CityController.CloakState == CloakStatus.Unknown || CityController.CanToggleCloak())
                 )
                    return AutomatonCity.City_Controller_State;
                else
                    return AutomatonCity.City_Attack_State;
            }
            //}

            //return null;
        }

        public void Tick()
        {

        }

        public void OnStateExit()
        {

        }
    }
}
