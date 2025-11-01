using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        public Visibility StartButton_Visibility
        {
            get;

            private set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartButton_Visibility)));
                }
            }

        }
        public int Score
        {
            get;
            private set
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Score)));
            }
        }
        public static Coords Dimensions => new(GameManager.gridRows, GameManager.gridColumns);
        public IEnumerable<(Coords coords, SolidColorBrush color)> Renderable
            => gameManager.FoodPool.ActiveFoods
                .Select(f => (f.Coords, Brushes.Red))
                .Concat(
                    gameManager.Snake.Body
                    .Select(s => (s.Coords, Brushes.LightGreen))
                );

        #region ICommands
        public ICommand StartGameCommand { get; }
        public ICommand RestartGameCommand { get; }
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
        public event Action? OnRestartRequest;
        public GameViewModel()
        {
            gameManager = new();
            StartButton_Visibility = Visibility.Visible;
            RestartButton_Visibility = Visibility.Collapsed;

            // ==== Button ICommands ====
            StartGameCommand = new RelayCommand(
                executeAsync: gameManager.RunGameAsync,
                canExecute: () => gameManager.State.CurrentState == GameStates.NotStarted
            );
            RestartGameCommand = new RelayCommand(
                execute: gameManager.RestartGame,
                canExecute: () => true
            );

            #region ==== Event Subscribers ====
            gameManager.OnIteration += () => OnRenderRequest?.Invoke();

            gameManager.State.OnGameStarted += () => StartButton_Visibility = Visibility.Collapsed;
            gameManager.State.OnGameStarted += () => RestartButton_Visibility = Visibility.Visible;

            gameManager.State.OnGameEnded += () => StartButton_Visibility = Visibility.Visible;
            gameManager.State.OnGameEnded += () => RestartButton_Visibility = Visibility.Collapsed;

            gameManager.State.OnGameRestarted += () => StartButton_Visibility = Visibility.Visible;
            gameManager.State.OnGameRestarted += () => RestartButton_Visibility = Visibility.Collapsed;
            gameManager.State.OnGameRestarted += () => OnRestartRequest?.Invoke();
            
            gameManager.OnScoreChange += () => this.Score = gameManager.Score;
            #endregion
        }

        public Visibility RestartButton_Visibility 
        {
            get;
            private set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RestartButton_Visibility)));
                }
            } 
        }
    }
}