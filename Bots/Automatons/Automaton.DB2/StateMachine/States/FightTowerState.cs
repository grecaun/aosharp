using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomatonDB2
{
    public class FightTowerState : IState
    {
        private Dictionary<Vector3, string> _towerPOS = new Dictionary<Vector3, string>();
        public void OnStateEnter()
        {
            if (SMovementController.IsNavigating())
                SMovementController.Halt();

            if (DynelManager.LocalPlayer.IsAttacking)
                DynelManager.LocalPlayer.StopAttack(false);

            Chat.WriteLine($"Tower!");
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

                    var _redTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && c.Buffs.Contains(274119));
                    var _blueTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && !c.Buffs.Contains(274119));

                    if (_redTower == null && _blueTower == null && _towerPOS.Count == 0)
                    {
                        var _aune = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Ground Chief Aune");

                        if (_aune != null && !_aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients)
                         && !DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                        {
                            AutomatonDB2._redTowerBool = false;
                            AutomatonDB2._blueTowerBool = false;
                            return AutomatonDB2.Fight;
                        }
                        else
                            SMovementController.SetNavDestination(Constants._startPosition);
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

                if (Extensions.Debuffed())
                {
                    if (DynelManager.LocalPlayer.IsAttacking == true)
                        DynelManager.LocalPlayer.StopAttack(false);

                    if (SMovementController.IsNavigating())
                        SMovementController.Halt();
                }
                else
                {
                    var _redTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && c.Buffs.Contains(274119));
                    var _blueTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && !c.Buffs.Contains(274119));


                    if (_redTower != null)
                    {
                        if (_redTower.IsInLineOfSight && _redTower.IsInAttackRange(true))
                        {
                            if (SMovementController.IsNavigating())
                                SMovementController.Halt();
                            else
                            {
                                if (DynelManager.LocalPlayer.FightingTarget == _redTower)
                                    DynelManager.LocalPlayer.StopAttack(false);

                                if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                                {
                                    if (_towerPOS.ContainsKey(_redTower.Position))
                                        _towerPOS.Remove(_redTower.Position);

                                    DynelManager.LocalPlayer.Attack(_redTower, false);
                                }
                            }
                        }
                        else //if (!AutomatonDB2._redTower.IsInLineOfSight || !AutomatonDB2._redTower.IsInAttackRange(true))
                        {
                            if (!SMovementController.IsNavigating())
                            {
                                if (!_towerPOS.ContainsKey(_redTower.Position))
                                    _towerPOS[_redTower.Position] = _redTower.Name;

                                SMovementController.SetNavDestination(_redTower.Position);
                            }
                        }
                    }
                    else if (_blueTower != null)
                    {
                        if (!DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                        {
                            if (_blueTower.IsInLineOfSight && _blueTower.IsInAttackRange(true))
                            {
                                if (SMovementController.IsNavigating())
                                    SMovementController.Halt();
                                else
                                {
                                    if (DynelManager.LocalPlayer.FightingTarget == _blueTower)
                                        DynelManager.LocalPlayer.StopAttack(false);

                                    if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                                    {
                                        if (_towerPOS.ContainsKey(_blueTower.Position))
                                            _towerPOS.Remove(_blueTower.Position);
                                        DynelManager.LocalPlayer.Attack(_blueTower, false);
                                    }
                                }
                            }
                            else //if (!AutomatonDB2._blueTower.IsInLineOfSight || !AutomatonDB2._blueTower.IsInAttackRange(true))
                            {
                                if (!SMovementController.IsNavigating())
                                {
                                    if (!_towerPOS.ContainsKey(_blueTower.Position))
                                        _towerPOS[_blueTower.Position] = _blueTower.Name;

                                    SMovementController.SetNavDestination(_blueTower.Position);
                                }
                            }

                        }
                    }
                    else if (_towerPOS.Count >= 1)
                    {
                        Vector3 towerPosition = _towerPOS.Keys.First();

                        if (DynelManager.LocalPlayer.Position.DistanceFrom(towerPosition) > 5f)
                        {
                            if (!SMovementController.IsNavigating())
                                SMovementController.SetNavDestination(towerPosition);
                        }
                        else if (_towerPOS.ContainsKey(towerPosition))
                        {
                            _towerPOS.Remove(towerPosition);
                        }
                    }
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
            if (DynelManager.LocalPlayer.IsAttacking)
                DynelManager.LocalPlayer.StopAttack(false);

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}