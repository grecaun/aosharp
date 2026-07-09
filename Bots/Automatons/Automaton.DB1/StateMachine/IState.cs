namespace AutomatonDB1
{
    public interface IState
    {
        void OnStateEnter();
        IState GetNextState();
        void Tick();
        void OnStateExit();
    }
}
