using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using SnakeGame.Helpers;
using SnakeGame.Model;
using SnakeGameProject;

namespace SnakeGame.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly GameManager gameManager;
        private readonly MainWindow view;
        public static Coords Dimensions => new(GameManager.gridRows, GameManager.gridColumns);
        public IEnumerable<(Coords coords, SolidColorBrush color)> Renderable()
            => gameManager.Snake.Body
                .Select(s => (s.Coords, Brushes.LightGreen))
                .Concat(
                    gameManager.FoodPool.ActiveFoods
                    .Select(f => (f.Coords, Brushes.Red))
                );

        #region ICommands
        public ICommand StartGameCommand { get; }
        #endregion
        public void KeyDownHandler(KeyEventArgs e)
        {
            gameManager.QueuedDirection = e.Key switch
            {
                Key.Up or Key.W => Direction.Up,
                Key.Down or Key.S => Direction.Down,
                Key.Left or Key.A => Direction.Left,
                Key.Right or Key.D => Direction.Right,

                _ => gameManager.QueuedDirection
            };
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? OnRenderRequest;
        public GameViewModel(MainWindow view)
        {
            gameManager = new();
            this.view = view;
            StartGameCommand = new RelayCommand(
                execute: gameManager.RunGameAsync,
                canExecute: () => gameManager.State.CurrentState == GameStates.NotStarted
            );

            gameManager.OnIterationEnd += () => OnRenderRequest?.Invoke();
        }
    }
}