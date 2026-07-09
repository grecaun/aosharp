using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonDB2
{
    public class FightState : IState
    {
        public void OnStateEnter()
        {
            Chat.WriteLine($"Smash Aune!!");

            if (DynelManager.LocalPlayer.IsAttacking)
                DynelManager.LocalPlayer.StopAttack(false);

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.PWId:
                    return AutomatonDB2.Idle;
                case Constants.DB2Id:
                    if (DynelManager.LocalPlayer.IsFalling && DynelManager.LocalPlayer.Position.DistanceFrom(Constants.first) < 75)
                        return AutomatonDB2.Fell;

                    if (AutomatonDB2._taggedNotum || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.MachineShockwave))
                        return AutomatonDB2.Notum;

                    var _aune = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Ground Chief Aune");

                    if (_aune != null && !DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.SeismicActivity))
                    {
                        var _redTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && c.Buffs.Contains(274119));

                        if (AutomatonDB2._redTowerBool || _redTower != null || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                        {
                            if (DynelManager.LocalPlayer.IsAttacking)
                                DynelManager.LocalPlayer.StopAttack(false);
                            else
                                return AutomatonDB2.FightTower;
                        }
                        var _blueTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && !c.Buffs.Contains(274119));

                        if (AutomatonDB2._blueTowerBool || _blueTower != null || _aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients))
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
            try
            {
                if (Game.IsZoning) { return; }

                if (Extensions.Debuffed() || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                {
                    if (DynelManager.LocalPlayer.IsAttacking)
                        DynelManager.LocalPlayer.StopAttack(false);

                    if (SMovementController.IsNavigating())
                        SMovementController.Halt();
                }
                else
                {
                    var _aune = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Ground Chief Aune");
                    if (_aune == null) return;

                    if (_aune.IsInLineOfSight && _aune.IsInAttackRange(true)
                        && DynelManager.LocalPlayer.Position.DistanceFrom(_aune.Position) < 8)
                    {
                        if (SMovementController.IsNavigating())
                            SMovementController.Halt();
                        else
                        {
                            if (DynelManager.LocalPlayer.FightingTarget == _aune)
                                DynelManager.LocalPlayer.StopAttack(false);

                            if (DynelManager.LocalPlayer.FightingTarget == null
                              && !DynelManager.LocalPlayer.IsAttackPending
                              && !_aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients))
                                DynelManager.LocalPlayer.Attack(_aune, false);
                        }
                    }
                    else if (!SMovementController.IsNavigating())
                        SMovementController.SetNavDestination(_aune.Position);
                }

            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred on line " + AutomatonDB2.GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != AutomatonDB2.previousErrorMessage)
                {
                    Chat.WriteLine(errorMessage);
                    Chat.WriteLine("Stack Trace: " + ex.StackTrace);
                    AutomatonDB2.previousErrorMessage = errorMessage;
                }
            }
        }

        public void OnStateExit()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}