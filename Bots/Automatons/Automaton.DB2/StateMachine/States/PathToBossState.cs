using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonDB2
{
    public class PathToBossState : IState
    {
        public void OnStateEnter()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
            Chat.WriteLine("Path to fight position");
        }
        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.PWId:
                    return AutomatonDB2.Idle;
                case Constants.DB2Id:
                    if (DynelManager.LocalPlayer.IsFalling && DynelManager.LocalPlayer.Position.DistanceFrom(Constants.first) < 75)
                        return AutomatonDB2.Fell;

                    if (AutomatonDB2._taggedNotum || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.MachineShockwave))
                        return AutomatonDB2.Notum;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._startPosition) < 1)
                        return AutomatonDB2.Fight;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._startPosition) < 5)
                        return AutomatonDB2.Idle;

                    var _aune = DynelManager.NPCs.FirstOrDefault((c => c.Health > 0 && c.Name == "Ground Chief Aune"));

                    if (_aune == null) return null;

                    var _redTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact")  && c.Buffs.Contains(274119));

                    var _blueTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && !c.Buffs.Contains(274119));

                    if (_redTower != null || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                        return AutomatonDB2.FightTower;

                    if (_blueTower != null || _aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients))
                    {
                        if (!DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                            return AutomatonDB2.FightTower;
                    }
                    break;
            }

            return null;
        }

        public void Tick()
        {
            if (Game.IsZoning) return;
            if (Playfield.ModelIdentity.Instance != Constants.DB2Id) return;
            if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPosition) < 5f) return;
            if (SMovementController.IsNavigating()) return;

            SMovementController.SetNavDestination(Constants._startPosition);
        }

        public void OnStateExit()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}