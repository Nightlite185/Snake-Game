namespace SnakeGame.Model
{
    public enum GameStates {Lost, Won, NotStarted, Paused, Running};
    public class GameState()
    {
        public event Action? OnGameStarted;
        public event Action? OnGameEnded;
        public event Action? OnGameRestarted;
        public GameStates Current { get; private set; } = GameStates.NotStarted;

        public void Pause()
        {
            Current = (Current != GameStates.Running)
                ? throw new InvalidOperationException($"cannot pause a game that is {Current}.")
                : GameStates.Paused;
        }

        public void Resume()
        {
            Current = (Current != GameStates.Paused)
                ? throw new InvalidOperationException($"cannot resume a game that is {Current}.")
                : GameStates.Running;
        }

        public void Start()
        {
            Current = (Current != GameStates.NotStarted)
                ? throw new InvalidOperationException($"cannot start a game that is {Current}.")
                : GameStates.Running;

            OnGameStarted?.Invoke();
        }

        public void Lose()
        {
            Current = (Current != GameStates.Running)
                ? throw new InvalidOperationException($"cannot lose a game that is {Current}.")
                : GameStates.Lost;

            OnGameEnded?.Invoke();
        }

        public void Win()
        {
            Current = (Current != GameStates.Running)
                ? throw new InvalidOperationException($"cannot win a game that is {Current}.")
                : GameStates.Won;
            
            OnGameEnded?.Invoke();
        }

        public void Restart()
        {
            Current = (Current == GameStates.NotStarted)
                ? throw new InvalidOperationException($"cannot restart a game that is {Current}.")
                : GameStates.NotStarted;

            OnGameRestarted?.Invoke();
        }
    }
}
