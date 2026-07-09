using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonDB2
{
    public class NotumState : IState
    {
        private double _timeToTagReset;

        public void OnStateEnter()
        {
            Chat.WriteLine("Nuke!");

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
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

                    if (!AutomatonDB2._taggedNotum && !DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.MachineShockwave))
                    {
                        var _aune = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name == "Ground Chief Aune");
                        if (_aune != null)
                        {
                            var _redTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && c.Buffs.Contains(274119));
                            var _blueTower = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && c.Name.Contains("Strange Xan Artifact") && !c.Buffs.Contains(274119));

                            if (_redTower == null && _blueTower == null)
                            {
                                if (_aune != null && !_aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients)
                                && !DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                                    return AutomatonDB2.Fight;
                            }

                            if (_redTower != null || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                                return AutomatonDB2.FightTower;

                            if (_blueTower != null || (_aune != null && _aune.Buffs.Contains(AutomatonDB2.Nanos.StrengthOfTheAncients)))
                            {
                                if (!DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                                    return AutomatonDB2.FightTower;
                            }
                        }
                        else
                            return AutomatonDB2.Idle;
                    }

                    break;
            }

            return null;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning) { return; }

               var _notumIrregularity = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Notum Irregularity");

                if (Extensions.Debuffed())
                {
                    if (SMovementController.IsNavigating())
                        SMovementController.Halt();

                    if (_notumIrregularity != null)
                    {
                        if (DynelManager.LocalPlayer.Position.Distance2DFrom(_notumIrregularity.Position) > 0.5)
                        {
                            DynelManager.LocalPlayer.Position = _notumIrregularity.Position;
                            MovementController.Instance.SetMovement(MovementAction.Update);
                            MovementController.Instance.SetMovement(MovementAction.TurnLeftStart);
                            MovementController.Instance.SetMovement(MovementAction.TurnLeftStop);
                            MovementController.Instance.SetMovement(MovementAction.Update);
                        }
                    }
                }
                else
                {
                    if (_notumIrregularity != null)
                    {
                        var playerDistance = DynelManager.LocalPlayer.Position.DistanceFrom(_notumIrregularity.Position);

                        if (playerDistance > 0.5)
                        {
                            if (!SMovementController.IsNavigating())
                                SMovementController.SetNavDestination(_notumIrregularity.Position);
                        }

                        if (playerDistance < 0.6)
                        {
                            if (SMovementController.IsNavigating())
                                SMovementController.Halt();
                            else
                                if (_timeToTagReset <= 0)
                                _timeToTagReset = Time.AONormalTime + 5;
                            else if (Time.AONormalTime >= _timeToTagReset)
                            {
                                AutomatonDB2._taggedNotum = false;
                                _timeToTagReset = 0;
                            }
                        }
                    }
                    else if (!SMovementController.IsNavigating())
                        SMovementController.SetNavDestination(Constants._centerPosition);
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
            AutomatonDB2._taggedNotum = false;

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }
    }
}