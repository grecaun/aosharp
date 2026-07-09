using System;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;

namespace AutomatonCity
{
    public class ButtonExitState : IState
    {
        private double Delay = 0;

        public void OnStateEnter()
        {
            Delay = Time.AONormalTime + 0.25;
            Chat.WriteLine("Enter Button Exit State");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State;
            if (!Playfield.IsDungeon) return AutomatonCity.Idle_State;

            return null;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning || !Team.IsInTeam) return;
                if (Time.AONormalTime < Delay) return;
                Delay = Time.AONormalTime + 0.15;

                AutomatonCity.ChangedFloors();

                var lp = DynelManager.LocalPlayer;

                var _downButton = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Button (down)");

                var _exitDoor = Playfield.Doors.FirstOrDefault(d => (d.RoomLink1 != null && d.RoomLink2 == null) || (d.RoomLink1 == null && d.RoomLink2 != null));

                var attacking = DynelManager.LocalPlayer.FightingTarget != null || DynelManager.LocalPlayer.IsAttacking || DynelManager.LocalPlayer.IsAttackPending;

                AutomatonCity.LockedDoors();

                if (lp.Identity == AutomatonCity.Leader)
                {
                    var target = DynelManager.NPCs.FirstOrDefault(t => t.Health > 0 && (t.IsAttacking || t.Room == lp.Room));

                    if (target != null)
                        AutomatonCity.HandleAttack(target, attacking);
                    else if (Team.Members.Any(c => c.Character == null)) return;
                    else if (_exitDoor != null)
                        MoveToExit(_exitDoor);
                    else if (_downButton != null)
                    {
                        if (AutomatonCity._downButtonLocation.Contains(_downButton.Position))
                            AutomatonCity._downButtonLocation.Remove(_downButton.Position);

                        AutomatonCity.HandleButtonUsage(_downButton);
                    }
                    else if (AutomatonCity._downButtonLocation.Count > 0)
                    {
                        if (lp.Position.DistanceFrom(AutomatonCity._downButtonLocation.Last()) > 5)
                            SMovementController.SetNavDestination(AutomatonCity._downButtonLocation.Last());
                    }
                    else if (AutomatonCity._exitDoorLocation != Vector3.Zero && lp.Room.Floor == 0)
                        SMovementController.SetNavDestination(AutomatonCity._exitDoorLocation);
                }
                else
                {
                    var leader = DynelManager.Players.FirstOrDefault(l => l.Health > 0 && l.Identity == AutomatonCity.Leader);

                    if (leader != null)
                    {
                        if (leader.IsAttacking)
                        {
                            AutomatonCity.HandleAttack(leader.FightingTarget, attacking);
                            return;
                        }
                        else if (!SMovementController.IsNavigating() && lp.Position.DistanceFrom(leader.Position) > 3)
                        {
                            SMovementController.SetNavDestination(leader.Position);
                            return;
                        }

                        return;
                    }
                    else if (_exitDoor != null)
                    {
                        MoveToExit(_exitDoor);
                        return;
                    }
                    else if (_downButton != null)
                    {
                        AutomatonCity.HandleButtonUsage(_downButton);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "An error occurred on line " + AutomatonCity.GetLineNumber(ex) + ": " + ex.Message;

                if (errorMessage != AutomatonCity.previousErrorMessage)
                {
                    Chat.WriteLine(errorMessage);
                    Chat.WriteLine("Stack Trace: " + ex.StackTrace);
                    AutomatonCity.previousErrorMessage = errorMessage;
                }
            }
        }

        private void MoveToExit(Dynel _exitDoor)
        {
            float distanceFromExit = DynelManager.LocalPlayer.Position.DistanceFrom(_exitDoor.Position);

            if (SMovementController.IsNavigating()) return;

            if (distanceFromExit > 5)
            {
                SMovementController.SetNavDestination(_exitDoor.Position);
                Chat.WriteLine("Moving to exit door");
            }
            else if (distanceFromExit < 2)
                SMovementController.SetMovement(MovementAction.ForwardStart);
        }

        public void OnStateExit()
        {
            Delay = 0;
            SMovementController.Halt();
        }
    }
}
