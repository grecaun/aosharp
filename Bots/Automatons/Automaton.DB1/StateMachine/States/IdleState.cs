using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using System.Linq;

namespace AutomatonDB1
{
    public class IdleState : IState
    {
        public void OnStateEnter()
        {
            AutomatonDB1._stateTimeOut = 0;

            if (AutomatonDB1.NavMeshMovementController.IsNavigating)
                AutomatonDB1.NavMeshMovementController.Halt();

            Chat.WriteLine("Idle");
        }

        public IState GetNextState()
        {
            try
            {
                if (Game.IsZoning) return null;
                if (!AutomatonDB1._settings["Enable"].AsBool()) return null;
                if (!Team.IsInTeam) return null;
                if (!Extensions.CanProceed()) return null;

                switch (Playfield.ModelIdentity.Instance)
                {
                    case Constants.PWId:
                        if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._entrance) > 30f) return null;

                        if (DynelManager.NPCs.Any(m => m.IsAttacking && Team.Members.Any(t => t.Identity == m.FightingTarget.Identity))) return AutomatonDB1.Clear;

                        if (DynelManager.LocalPlayer.Identity == AutomatonDB1.Leader)
                        {
                            if (!Team.Members.Any(t => t.Character == null))
                                return AutomatonDB1.Enter;
                        }
                        else
                        {
                            var _leader = DynelManager.Players.FirstOrDefault(c => c.Health > 0 && c.Identity == AutomatonDB1.Leader);

                            if (_leader == null)
                            {
                                if (AutomatonDB1._stateTimeOut == 0)
                                    AutomatonDB1._stateTimeOut = Time.AONormalTime + AutomatonDB1.Rand(0.5f, 3.5f);

                                if (Time.AONormalTime > AutomatonDB1._stateTimeOut)
                                {
                                    AutomatonDB1._stateTimeOut = 0;
                                    return AutomatonDB1.Enter;
                                }

                                return null;
                            }
                        }
                        break;
                    case Constants.DB1Id:

                        if (!Team.Members.Any(c => c.Character == null) && DynelManager.LocalPlayer.Position.DistanceFrom(Constants._atDoor) < 20f)
                            return AutomatonDB1.Start_State;

                        if (DynelManager.LocalPlayer.Position.DistanceFrom(Constants._atDoor) > 40f)
                        {
                            if (!DynelManager.LocalPlayer.Buffs.Contains(AutomatonDB1.Nanos.ThriceBlessedbytheAncients))
                                return AutomatonDB1.BuffState;
                            else
                                return AutomatonDB1.Fight;
                        }

                        var _mikkelsenCorpse = DynelManager.Corpses.FirstOrDefault(c => c.Name == "Remains of Ground Chief Mikkelsen");

                        if (AutomatonDB1._settings["Farming"].AsBool() && _mikkelsenCorpse != null)
                            return AutomatonDB1.Loot;

                        break;
                }

                return null;

            }
            catch (Exception ex)
            {
                AutomatonDB1.ErrorCatch(ex);
                return null;
            }
        }

        public void Tick()
        {
        }

        public void OnStateExit()
        {
            AutomatonDB1._stateTimeOut = 0;
        }
    }
}
