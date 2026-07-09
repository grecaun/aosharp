using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using System;
using System.Linq;

namespace AutomatonCity
{
    public class BossRoomState : IState
    {
        private double Delay = 0;

        public void OnStateEnter()
        {
            Delay = Time.AONormalTime + 0.2;
            Chat.WriteLine("Boss room state.");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State;
            if (!Playfield.IsDungeon) return AutomatonCity.Idle_State;
            if (DynelManager.LocalPlayer.Room.Name != "AI_bossroom") return AutomatonCity.Idle_State; ;

            var bossCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("Fleet Admiral") || c.Name.Contains("Recruitment Director") || c.Name.Contains("General"));

            if (bossCorpse == null) return null;

            return AutomatonCity.Boss_Loot_State;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning || !Team.IsInTeam) return;
                if (Time.AONormalTime < Delay) return;
                if (Team.Members.Any(c => c.Character == null)) return;
                Delay = Time.AONormalTime + 0.15;

                AutomatonCity.ChangedFloors();

                var _downButton = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Button (down)");

                if (_downButton != null && !AutomatonCity._downButtonLocation.Contains(_downButton.Position))
                    AutomatonCity._downButtonLocation.Add(_downButton.Position);

                var _pilot = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && (c.Name.Contains("Fighter Pilot") || c.Name.Contains("Alien Reproduction Technician")));

                var _target = DynelManager.NPCs.Where(c => c.Health > 0 && !c.IsPet && c.IsInLineOfSight && !AutomatonCity._ignores.Contains(c.Name) && c.Identity != _pilot?.Identity)
                    .OrderBy(c => c.Position.DistanceFrom(DynelManager.LocalPlayer.Position)).ThenBy(c => c.HealthPercent).FirstOrDefault();

                var _boss = DynelManager.NPCs.FirstOrDefault(c => c.Health > 0 && (c.Name.Contains("Fleet Admiral") || c.Name.Contains("Recruitment Director")));

                var lp = DynelManager.LocalPlayer;

                var attacking =  (DynelManager.LocalPlayer.IsAttacking && DynelManager.LocalPlayer.FightingTarget != null) || DynelManager.LocalPlayer.IsAttackPending;

                if (lp.Identity == AutomatonCity.Leader)
                {
                    var target = _pilot ?? _target ?? _boss;

                    if (target != null)
                        HandleAttack(target, attacking);
                    else if (DynelManager.LocalPlayer.Position.DistanceFrom(DynelManager.LocalPlayer.Room.Position) > 5)
                            SMovementController.SetNavDestination(DynelManager.LocalPlayer.Room.Position);
                }
                else
                {
                    var leader = DynelManager.Players.FirstOrDefault(l => l.Health > 0 && l.Identity == AutomatonCity.Leader);

                    if (leader != null)
                    {
                        if (leader.IsAttacking)
                            HandleAttack(leader.FightingTarget, attacking);
                        else if (lp.Position.DistanceFrom(leader.Position)> 3)
                            SMovementController.SetNavDestination(leader.Position);
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

        private void HandleAttack(SimpleChar target, bool attacking)
        {
            if (target.IsInAttackRange(true) && target.IsInLineOfSight)
            {
                if (SMovementController.IsNavigating())
                    SMovementController.Halt();

                if (!DynelManager.LocalPlayer.IsAttacking && DynelManager.LocalPlayer.FightingTarget == null && !DynelManager.LocalPlayer.IsAttackPending)
                    DynelManager.LocalPlayer.Attack(target, false);
            }
            else if (!SMovementController.IsNavigating())
                SMovementController.SetNavDestination(target.Position);
            return;
        }

        public void OnStateExit()
        {
            Delay = 0;
            SMovementController.Halt();
        }
    }
}
