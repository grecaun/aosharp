using AOSharp.Core;
using AOSharp.Pathfinding;
using System.Linq;

namespace AutomatonDB2
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            var _leader = Team.Members.FirstOrDefault(c => c.Character?.Health > 0 && c.Identity == AutomatonDB2.Leader)?.Character;

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.PWId:

                    if (DynelManager.NPCs.Any(m => m.IsAttacking && Team.Members.Any(t => t.Identity == m.FightingTarget.Identity))) return AutomatonDB2.Clear;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) > 30f) return null;

                    if (DynelManager.LocalPlayer.Identity == AutomatonDB2.Leader)
                    {
                        if (!Team.IsInTeam)
                            return AutomatonDB2.Reform;
                        else if (!Extensions.CanProceed()) return null;
                        else
                            return AutomatonDB2.Enter;
                    }
                    else
                    {
                        if (!Team.IsInTeam) return null;

                        if (_leader != null) return null;

                        if (!Extensions.CanProceed()) return null;

                        return AutomatonDB2.Enter;
                    }
                case Constants.DB2Id:

                    if (!Team.IsInTeam) return null;

                    if (DynelManager.LocalPlayer.IsFalling && DynelManager.LocalPlayer.Position.DistanceFrom(Constants.first) < 75)
                        return AutomatonDB2.Fell;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPosition) > 50f)
                        return AutomatonDB2.PathToBoss;

                    if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._centerPosition) < 50f)
                        return AutomatonDB2.Fight;

                    if (AutomatonDB2._taggedNotum || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.MachineShockwave))
                        return AutomatonDB2.Notum;

                    var _aune = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Ground Chief Aune");

                    if (_aune != null)
                    {
                        var _redTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && c.Buffs.Contains(274119));
                       
                        if (_redTower != null || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                            return AutomatonDB2.FightTower;

                        var _blueTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && !c.Buffs.Contains(274119));

                        if (_blueTower != null || _aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients))
                        {
                            if (!DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                                return AutomatonDB2.FightTower;
                        }
                    }

                    var _auneCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Ground Chief Aune");
                    var _exitBeacon = DynelManager.AllDynels.FirstOrDefault(c => c.Name.Contains("Dust Brigade Exit Beacon"));

                    if (_auneCorpse == null || _exitBeacon == null) return null;

                    if (!AutomatonDB2._settings["Farming"].AsBool()) return null;
                    return AutomatonDB2.Loot;

            }

            return null;
        }

        public void Tick()
        {

        }

        public void OnStateExit()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}
