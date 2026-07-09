using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using AutomatonCity.IPCMessages;

namespace AutomatonCity
{
    public class NavGenState : IState
    {
        
        public void OnStateEnter()
        {
            NavGen();

            Chat.WriteLine("NavGen");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State;
            if (!Playfield.IsDungeon) return AutomatonCity.Idle_State;

            if (AutomatonCity._navMeshes != null) return null;

            if (DynelManager.LocalPlayer.Identity == AutomatonCity.Leader)
            {
                AutomatonCity.IPCChannel.Broadcast(new EnterMessage());
                return AutomatonCity.Path_State;
            }
            else
                return AutomatonCity.Path_State;
        }

        public void Tick()
        {
           
        }

        public void NavGen()
        {
            AutomatonCity.navMeshFactory.GenerateNavMeshAsync().ContinueWith(navMesh =>
            {
                if (navMesh.Result == null)
                {
                    Chat.WriteLine("NavGen failed");
                    return;
                }

                AutomatonCity._navMeshes = navMesh.Result;
                Chat.WriteLine("NavGen Finished");
                SMovementController.LoadNavmesh(AutomatonCity._navMeshes[AutomatonCity._currentAbsFloor], true);
            });
        }

        public void OnStateExit()
        {

        }
    }
}
