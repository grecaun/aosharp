using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System;
using System.Linq;

namespace AutomatonCity
{
    public class CityAttackState : IState
    {
        private double Delay = 0;
        private Vector3 StartPos = Vector3.Zero;

        public void OnStateEnter()
        {
            Delay = Time.AONormalTime + .25;

            switch (Playfield.ModelIdentity.Instance)
            {
                case 5001://Playadel Desierto
                    StartPos = AutomatonCity._playadelGaurdPos;
                    break;
                case 5002://Montroyal City
                    StartPos = AutomatonCity._montroyalGaurdPos;
                    break;
                case 6010:// Serenity Islands
                    StartPos = AutomatonCity._serenityGaurdPos;
                    break;
            }

            MoveToStart();

            Chat.WriteLine("City state");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;
            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State;
            if (!AutomatonCity.validPlayfields.Contains(Playfield.ModelIdentity.Instance)) return AutomatonCity.Idle_State;

            if (DynelManager.LocalPlayer.Identity == AutomatonCity.Leader && !DynelManager.NPCs.Any(c => c.Health > 0) && !AutomatonCity.CityUnderAttack
                    && (CityController.CloakState == CloakStatus.Unknown || CityController.CanToggleCloak()))
                return AutomatonCity.City_Controller_State;

            var _target = DynelManager.NPCs.Where(c => c.Health > 0 && !c.IsPet && c.DistanceFrom(DynelManager.LocalPlayer) < 40f).FirstOrDefault();

            if (_target != null) return null;

            var _bossCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("General") && !AutomatonCity.Bosses.Contains(c.Identity));

            if (_bossCorpse != null)
                AutomatonCity.CityUnderAttack = false;

            if (AutomatonCity.CityUnderAttack) return null;

            var shipentrance = DynelManager.AllDynels.FirstOrDefault(c => c.Identity.Type == IdentityType.ACGEntrance);// c.Name == "Door"

            if (shipentrance == null) return null;

            return AutomatonCity.Boss_Loot_State;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning) return;
                if (Time.AONormalTime < Delay) return;

                Delay = Time.AONormalTime + 0.2;

                var _bossCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name.Contains("General") && !AutomatonCity.Bosses.Contains(c.Identity));

                if (_bossCorpse != null) return;

                var lp = DynelManager.LocalPlayer;

                var attacking = DynelManager.LocalPlayer.IsAttacking || DynelManager.LocalPlayer.IsAttackPending ;


                if (AutomatonCity._settings["Looter"].AsBool())
                {
                    var corpse = DynelManager.Corpses.FirstOrDefault();

                    if (corpse != null)
                    {
                        if (!MovementController.Instance.IsNavigating)
                        {
                            if (DynelManager.LocalPlayer.Position.Distance2DFrom(corpse.Position) > 5)
                                MovementController.Instance.SetDestination(corpse.Position);
                        }

                        return;
                    }
                }

                if (lp.Identity != AutomatonCity.Leader)
                {
                    var leader = DynelManager.Players.FirstOrDefault(l => l != null && l.Health > 0 && l.Identity == AutomatonCity.Leader);

                    if (leader == null)
                    {
                        MoveToStart();
                        return;
                    }
                    else if (leader.IsAttacking)
                    {
                        if (!attacking)
                        {
                            if (leader.FightingTarget != null)
                                lp.Attack(leader.FightingTarget, false);
                        }
                    }
                    else if (!MovementController.Instance.IsNavigating)
                    {
                        if (lp.Position.DistanceFrom(leader.Position) > 1.6f)
                            MovementController.Instance.SetDestination(leader.Position);
                    }
                }
                else
                {
                    var _target = DynelManager.NPCs.Where(c => c.Health > 0 && !c.IsPet && c.DistanceFrom(DynelManager.LocalPlayer) < 40f).OrderByDescending(c => c.Name.Contains("Hacker")).FirstOrDefault();

                    if (_target != null)
                    {
                        if (_target.Position.DistanceFrom(DynelManager.LocalPlayer.Position) > 5f)
                            MovementController.Instance.SetDestination(_target.Position);
                        else if (!attacking)
                            DynelManager.LocalPlayer.Attack(_target, false);
                    }
                    else
                        MoveToStart();
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

        private void MoveToStart()
        {
            if (!MovementController.Instance.IsNavigating)
            {
                if (DynelManager.LocalPlayer.Position.Distance2DFrom(StartPos) > 10)
                    MovementController.Instance.SetDestination(StartPos);
            }
            return;
        }

        public void OnStateExit()
        {
            Delay = 0;
            StartPos = Vector3.Zero;
        }
    }
}
