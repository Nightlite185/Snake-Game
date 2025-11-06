using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SnakeGame.Helpers;
using SnakeGame.Model;

namespace SnakeGame.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly GameManager gameManager;
        public int Score
        {
            get;
            private set
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Score)));
            }
        }
        public string? NicknameEntered
        {
            get
            {
                if (string.IsNullOrWhiteSpace(field))
                    return "Guest";

                return field;
            }

            set;
        }
        public ObservableCollection<ScoreEntry> ScoreboardEntries { get; private set; } = [];
        public static Coords Dimensions => new(GameManager.gridRows, GameManager.gridColumns);
        public IEnumerable<(Coords coords, SolidColorBrush)> GetRenderable()
        {
            foreach (var food in gameManager.FoodPool.ActiveFoods)
                yield return (food.Coords, Brushes.Red);

            yield return (gameManager.Snake.HeadPos, Brushes.RoyalBlue);

            foreach (var seg in gameManager.Snake.Body.Skip(1))
                yield return (seg.Coords, Brushes.LightSkyBlue);
        }

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
            OptionsButton_Visibility = Visibility.Visible;

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
            gameManager.State.OnGameStarted += () => OptionsButton_Visibility = Visibility.Collapsed;

            gameManager.State.OnGameRestarted += () => StartButton_Visibility = Visibility.Visible;
            gameManager.State.OnGameRestarted += () => RestartButton_Visibility = Visibility.Collapsed;
            gameManager.State.OnGameRestarted += () => OptionsButton_Visibility = Visibility.Visible;

            gameManager.State.OnGameRestarted += () => OnRestartRequest?.Invoke();

            gameManager.OnScoreChange += () => this.Score = gameManager.Score;
            gameManager.GotFinalScore += newScore =>
            {
                for (int i = 0; i < ScoreboardEntries.Count; i++)
                {
                    var entry = ScoreboardEntries[i];

                    switch (entry.Score)
                    {
                        case int s when newScore > s: // if the new one is higher -> we put it above the i
                            ScoreboardEntries.Insert(i, new ScoreEntry(newScore, NicknameEntered!));
                            return;

                        case int s when newScore < s: // if its lower -> we continue checking next
                            continue;

                        default: // if its equal -> check if it already has that user: yes -> skip | no -> put it below.
                            if(
                                ScoreboardEntries
                                .Where(e => e.Score == newScore)
                                .Any(e => e.Nickname == NicknameEntered)
                            ) 
                                return;

                            else
                                ScoreboardEntries.Insert(i + 1, new ScoreEntry(newScore, NicknameEntered!));
                                return;
                    }
                }
                
                // if it got to this point -> no lower scores found -> append to the end.
                ScoreboardEntries.Add(new ScoreEntry(newScore, NicknameEntered!));
            };
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
        public Visibility OptionsButton_Visibility
        {
            get;
            private set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OptionsButton_Visibility)));
                }
            }
        }
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
    }
}