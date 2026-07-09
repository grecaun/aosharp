using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Movement;
using AOSharp.Core.UI;
using System;
using System.Linq;

namespace AutomatonCity
{

    public class EnterState : IState
    {
        private State currentState = State.PathingToEntrance;
        private double forwardStartTime;

        public void OnStateEnter()
        {
            Chat.WriteLine("Entering ship");
        }

        public IState GetNextState()
        {
            if (Game.IsZoning) return null;

            if (!AutomatonCity._settings["Enable"].AsBool()) return AutomatonCity.Idle_State; ;

            if (!Playfield.IsDungeon) return null;

            if (DynelManager.LocalPlayer.Room.Name != "AI_entrance") return null;

            return AutomatonCity.Nav_Gen_State;
        }

        public void Tick()
        {
            try
            {
                if (Game.IsZoning) { return; }
                var shipEntrance = DynelManager.AllDynels.FirstOrDefault(c => c.Identity.Type == IdentityType.ACGEntrance);
                //var shipEntrance = DynelManager.AllDynels.FirstOrDefault(c => c.Name == "Door");

                if (shipEntrance == null) return;
                
                    float distanceToDoor = DynelManager.LocalPlayer.Position.DistanceFrom(shipEntrance.Position);

                switch (currentState)
                {
                    case State.PathingToEntrance:
                        if (distanceToDoor > 1)
                            MovementController.Instance.SetDestination(shipEntrance.Position);
                        else
                        {
                            MovementController.Instance.Halt();
                            currentState = State.ArrivedAtEntrance;
                        }
                        break;
                    case State.ArrivedAtEntrance:
                        MovementController.Instance.SetMovement(MovementAction.ForwardStart);
                        forwardStartTime = Time.AONormalTime + 1;
                        currentState = State.MovingForward;
                        break;
                    case State.MovingForward:
                        if (Time.AONormalTime < forwardStartTime) return;
                        MovementController.Instance.SetMovement(MovementAction.ForwardStop);
                        currentState = State.PathingToEntrance;
                        break;
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
        public enum State
        {
            PathingToEntrance,
            ArrivedAtEntrance,
            MovingForward,
        }

        public void OnStateExit()
        {
            
        }
    }
}