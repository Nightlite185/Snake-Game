namespace SnakeGame.Model
{
    internal enum GameStates {Lost, Won, NotStarted, Paused, Running};
    internal class GameState()
    {
        public GameStates CurrentState { get; private set; } = GameStates.NotStarted;

        public void Pause() => CurrentState = (CurrentState != GameStates.Running)
            ? throw new InvalidOperationException("cannot pause a game that is not currently running")
            : CurrentState = GameStates.Paused;
            
        public void Resume() => CurrentState = (CurrentState != GameStates.Paused)
            ? throw new InvalidOperationException("cannot resume a game that is not paused.") 
            : CurrentState = GameStates.Paused;

        public void Start() => CurrentState = (CurrentState != GameStates.NotStarted)
            ? throw new InvalidOperationException("cannot start a game that has already started.")
            : CurrentState = GameStates.Running;

        public void Lose() => CurrentState = (CurrentState != GameStates.Running)
            ? throw new InvalidOperationException("cannot lose a game that is not running.")
            : CurrentState = GameStates.Lost;

        public void Win() => CurrentState = (CurrentState != GameStates.Running)
            ? throw new InvalidOperationException("cannot win a game that is not running")
            : CurrentState = GameStates.Won;
            
        public void Reset() => CurrentState = (CurrentState == GameStates.NotStarted)
            ? throw new InvalidOperationException("cannot restart a game that is already restarted.")
            : CurrentState = GameStates.Paused;
    }
}
