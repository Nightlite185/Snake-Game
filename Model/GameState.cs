namespace SnakeGame.Model
{
    public enum GameStates {Lost, Won, NotStarted, Paused, Running};
    public class GameState()
    {
        public event Action<GameStates>? OnStateChange;
        public GameStates CurrentState { get; private set; } = GameStates.NotStarted;
        private GameStates ChangeState(GameStates newState) // separate method to keep it clean and invoke event in the same time.
        {
            OnStateChange?.Invoke(newState);
            return newState;
        }

        public void Pause() => CurrentState = (CurrentState != GameStates.Running)
            ? throw new InvalidOperationException($"cannot pause a game that is {CurrentState}.")
            : ChangeState(GameStates.Paused); // maybe just do it the {} way everywhere and reduce overhead by not calling additional method, idk Ill think later.
            
        public void Resume() => CurrentState = (CurrentState != GameStates.Paused)
            ? throw new InvalidOperationException($"cannot resume a game that is {CurrentState}.") 
            : ChangeState(GameStates.Running);

        public void Start() => CurrentState = (CurrentState != GameStates.NotStarted)
            ? throw new InvalidOperationException($"cannot start a game that is {CurrentState}.")
            : ChangeState(GameStates.Running);

        public void Lose() => CurrentState = (CurrentState != GameStates.Running)
            ? throw new InvalidOperationException($"cannot lose a game that is {CurrentState}.")
            : ChangeState(GameStates.Lost);

        public void Win() => CurrentState = (CurrentState != GameStates.Running)
            ? throw new InvalidOperationException($"cannot win a game that is {CurrentState}.")
            : ChangeState(GameStates.Won);

        public void Reset() => CurrentState = (CurrentState == GameStates.NotStarted)
            ? throw new InvalidOperationException($"cannot restart a game that is {CurrentState}.")
            : ChangeState(GameStates.NotStarted);
    }
}
