using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Common.GameData;
using System.Linq;

namespace AutomatonDB1
{
    public class EnterState : IState
    {
        double Delay = 0;

        public void OnStateEnter()
        {
            Chat.WriteLine("Entering DB1");
            
            Delay = Time.AONormalTime;
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.DB1Id:
                    if (Time.AONormalTime < Delay + 2f) return null;
                    if (Team.Members.Any(c => c.Character == null)) return null;
                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._atDoor) < 20f)
                        return AutomatonDB1.Start_State;
                    break;
                case Constants.PWId:
                    if (Extensions.CanProceed()) return null;
                    return AutomatonDB1.Idle;
            }
            
            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) { return; }

            if (Playfield.ModelIdentity.Instance != Constants.PWId) return;
            if (Time.AONormalTime < Delay + 2f) return;

            if (!DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.ThriceBlessedbytheAncients) ||
                DynelManager.LocalPlayer.Buffs.Find(AutomatonDB1.Nanos.ThriceBlessedbytheAncients, out Buff buff) && buff.RemainingTime > AutomatonDB1._settings["LastRunTime"].AsFloat())
            {
                AutomatonDB1.NavMeshMovementController.SetDestination(Constants._entrance);
                AutomatonDB1.NavMeshMovementController.AppendDestination(new Vector3(2119.3f, 3.2f, 2762.1f));
            }
        }

        public void OnStateExit()
        {
            Delay = 0;

            AutomatonDB1.StartStamp = Time.AONormalTime;
        }
    }
}
