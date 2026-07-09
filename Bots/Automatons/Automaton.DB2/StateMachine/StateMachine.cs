using AOSharp.Core.UI;
using System;

namespace AutomatonDB2
{
    public class StateMachine
    {
        public IState CurrentState { get; private set; } = null;

        public StateMachine(IState defaultState)
        {
            SetState(defaultState);
        }

        public void Tick()
        {
            try
            {
                IState nextState = CurrentState.GetNextState();

                if (nextState != null)
                    SetState(nextState);

                CurrentState.Tick();

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
        public void SetState(IState state, bool triggerEvents = true)
        {
            if (CurrentState != null && triggerEvents)
                CurrentState.OnStateExit();

            CurrentState = state;

            if (triggerEvents)
                state.OnStateEnter();
        }
    }
}
