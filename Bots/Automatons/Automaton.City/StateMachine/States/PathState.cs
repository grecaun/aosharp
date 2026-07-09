using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;

namespace AutomatonCity
{
    public class PathState : IState
    {
        //private Dictionary<Room, IEnumerable<Door>> Room_Door_Map;

        private double Delay = 0;

        public void OnStateEnter()
        {
            Delay = Time.AONormalTime + 0.2;
            Chat.WriteLine("PathState");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State;
            if (!Playfield.IsDungeon) return AutomatonCity.Idle_State;

            if (DynelManager.LocalPlayer.Room.Name != "AI_bossroom") return null;

            return AutomatonCity.Boss_Room_State;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning || !Team.IsInTeam) { return; }
                if (Time.AONormalTime < Delay) return;
                Delay = Time.AONormalTime + 0.15;

                AutomatonCity.ChangedFloors();

                var exitDoorPos = Playfield.Doors.FirstOrDefault(d => (d.RoomLink1 != null && d.RoomLink2 == null) || (d.RoomLink1 == null && d.RoomLink2 != null))?.Position;

                if (exitDoorPos != null)
                    if (AutomatonCity._exitDoorLocation != exitDoorPos)
                        AutomatonCity._exitDoorLocation = (Vector3)exitDoorPos;
                
                var lp = DynelManager.LocalPlayer;

                var _downButton = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Button (down)");

                if (_downButton != null && !AutomatonCity._downButtonLocation.Contains(_downButton.Position))
                    AutomatonCity._downButtonLocation.Add(_downButton.Position);
                
                var _upButton = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Button (up)" || c.Name == "Button (boss)");

                var attacking = (DynelManager.LocalPlayer.IsAttacking && DynelManager.LocalPlayer.FightingTarget != null) ||  DynelManager.LocalPlayer.IsAttackPending;

                AutomatonCity.LockedDoors();

                if (lp.Identity == AutomatonCity.Leader)
                {
                    var AttackingTarget = DynelManager.NPCs.FirstOrDefault(t => t.Health > 0 && !t.IsPet && t.IsAttacking );
                    if (attacking) return;
                    
                    if (AttackingTarget != null)
                        AutomatonCity.HandleAttack(AttackingTarget, attacking);
                    else if (Team.Members.Any(c => c.Character == null)) return;
                    else if (_upButton != null)
                        AutomatonCity.HandleButtonUsage(_upButton);
                    else
                    {
                        var target = DynelManager.NPCs.FirstOrDefault(t => t.Health > 0 && !t.IsPet);
                        if (target != null)
                        {
                            AutomatonCity.HandleAttack(target, attacking);
                            return;
                        }    
                    }
                }
                else
                {
                    var leader = DynelManager.Players.FirstOrDefault(l => l.Health > 0 && l.Identity == AutomatonCity.Leader);

                    if (leader != null)
                    {
                        if (leader.IsAttacking)
                        {
                            if (DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttacking && !DynelManager.LocalPlayer.IsAttackPending)
                            {
                                if (leader.FightingTarget != null)
                                {
                                    AutomatonCity.HandleAttack(leader.FightingTarget, attacking);
                                    return;
                                }    
                            }
                        }
                        else if (!SMovementController.IsNavigating() && lp.Position.DistanceFrom(leader.Position) > 2)
                        {
                            SMovementController.SetNavDestination(leader.Position);
                            return;
                        }
                    }
                    else if (_upButton != null)
                    {
                        AutomatonCity.HandleButtonUsage(_upButton);
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

        //private void Search()
        //{
        //    Chat.WriteLine("Searching");

        //    if (SMovementController.IsNavigating())
        //        return;

        //    Room firstTargetRoom = null;

        //    foreach (var room in Playfield.Rooms)
        //    {
        //        if (Room_Door_Map.ContainsKey(room))
        //            continue;

        //        var anyDoorExists = Room_Door_Map.Values
        //            .SelectMany(doors => doors)
        //            .Any(door => room.Doors.Contains(door));

        //        if (!anyDoorExists)
        //        {
        //            firstTargetRoom = room;
        //            break;
        //        }

        //        if (firstTargetRoom == null)
        //            firstTargetRoom = room;
        //    }

        //    if (firstTargetRoom != null)
        //    {
        //        var door = firstTargetRoom.Doors.FirstOrDefault();

        //        if (door != null)
        //            SMovementController.SetNavDestination(door.Position);
        //    }
        //}

        public void OnStateExit()
        {
            Delay = 0;
            SMovementController.Halt();
        }
    }
}
