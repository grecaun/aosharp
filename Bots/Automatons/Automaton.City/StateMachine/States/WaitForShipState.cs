using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System.Linq;

namespace AutomatonCity
{
    public class WaitForShipState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine("Waiting for ship.");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State;;

            var shipentrance = DynelManager.AllDynels.FirstOrDefault(c => c.Identity.Type == IdentityType.ACGEntrance);
            //var shipentrance = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Door");

            if (shipentrance == null) return null;
            
            if (DynelManager.LocalPlayer.Identity == AutomatonCity.Leader)
                return AutomatonCity.Enter_State;

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
