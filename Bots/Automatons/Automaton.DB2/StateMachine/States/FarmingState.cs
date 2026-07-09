using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonDB2
{
    public class FarmingState : IState
    {
        private bool _atCorpse = false;
        private double _timeToLeave;

        public void OnStateEnter()
        {
            _atCorpse = false;
            _timeToLeave = 0;

            Chat.WriteLine("Pillage me treasures!");

            if (SMovementController.IsNavigating())
                SMovementController.Halt();
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) { return null; }

            switch (Playfield.ModelIdentity.Instance)
            {
                case Constants.PWId:
                    return AutomatonDB2.Reform;
                case Constants.DB2Id:
                    if (DynelManager.LocalPlayer.IsFalling && DynelManager.LocalPlayer.Position.DistanceFrom(Constants.first) < 75)
                        return AutomatonDB2.Fell;
                    break;
            }

            return null;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning) return;
                if (!Extensions.CanProceed()) return;
                if (Playfield.ModelIdentity.Instance != Constants.DB2Id) return;

                if (Extensions.Debuffed() || DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB2.Nanos.XanBlessingoftheEnemy))
                {
                    if (DynelManager.LocalPlayer.IsAttacking)
                        DynelManager.LocalPlayer.StopAttack(false);

                    if (SMovementController.IsNavigating())
                        SMovementController.Halt();
                }

                var _auneCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Ground Chief Aune");

                if (_auneCorpse != null)
                {
                    if (DynelManager.LocalPlayer.Buffs.Contains(NanoLine.Stun)) return;

                    if (SMovementController.IsNavigating()) return;

                    if (!_atCorpse)
                    {
                        if (AtPosition(_auneCorpse.Position, 2))
                        {
                            Chat.WriteLine("Pause for looting, 30 sec");
                            _timeToLeave = Time.AONormalTime + 30;
                            _atCorpse = true;
                        }
                    }
                    else if (Time.AONormalTime < _timeToLeave) return;
                    else
                        Disband();
                }
                else
                    Disband();
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

        bool AtPosition(Vector3 position, int distance)
        {
            HandlePathing(position, distance);
            return SMovementController.IsNavigating() == false;
        }

        void HandlePathing(Vector3 position, int distance)
        {
            if (DynelManager.LocalPlayer.Position.DistanceFrom(position) > distance)
            {
                if (!SMovementController.IsNavigating())
                    SMovementController.SetNavDestination(position);
            }
            else
            {
                if (SMovementController.IsNavigating())
                    SMovementController.Halt();
            }
        }

        private void Disband()
        {
            if (!Team.IsInTeam) return;

            foreach (var member in Team.Members)
            {
                if (AutomatonDB2._teamCache.Contains(member.Identity)) continue;
                AutomatonDB2._teamCache.Add(member.Identity);

                if (DynelManager.LocalPlayer.Identity != AutomatonDB2.Leader) continue;
                if (member.Identity == AutomatonDB2.Leader) continue;
                Team.Kick(member.Identity);
            }
        }

        public void OnStateExit()
        {
            _atCorpse = false;
            _timeToLeave = 0;
        }
    }
}