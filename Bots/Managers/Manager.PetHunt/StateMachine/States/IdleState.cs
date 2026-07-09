using AOSharp.Core.UI;

namespace ManagerPetHunt
{
    public class IdleState : IState
    {
        public IState GetNextState()
        {
            if (ManagerPetHunt._settings["Enable"].AsBool())
            {
                return new ScanState();
            }

            return null;
        }

        public void OnStateEnter()
        {
            //Chat.WriteLine("Idle");
        }

        public void OnStateExit()
        {
            
        }

        public void Tick()
        {
        }
    }
}
