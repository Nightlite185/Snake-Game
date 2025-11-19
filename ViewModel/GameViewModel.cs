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
        private readonly GameManager? gm;
        public readonly Scoreboard sb;
        private GameState State { get; init; }
        private Settings Cfg { get; set; }
        public void SaveOnExit() => sb.SaveOnExit();
        public GameViewModel()
        {
            // Objects init
            Cfg = Settings.TryInitFromJson() ?? new Settings(GetDefault: true);
            sb = new Scoreboard();
            State = new();
            
            
            // Button Visibility init
            StartButton_Visibility = Visibility.Visible;
            RestartButton_Visibility = Visibility.Collapsed;
            OptionsButton_Visibility = Visibility.Visible;

            #region ICommands
            StartGameCommand = new RelayCommand(
                executeAsync: gm.RunGameAsync,
                canExecute: () => gm.State.CurrentState == GameStates.NotStarted
            );

            RestartGameCommand = new RelayCommand(
                execute: () => gm.RestartGame(Cfg),
                canExecute: () => true
            );

            ResetScoreboardCommand = new RelayCommand(
                execute: sb.ResetScoreboard,
                canExecute: () => sb.VisualScores.Count > 0
            );

            OpenOptionsCommand = new RelayCommand(
                execute: () => new OptionsWindow(Cfg).ShowDialog(),
                canExecute: () => gm.State.CurrentState != GameStates.NotStarted
            );

            #endregion

            #region Event Subscribers
            gm.OnIteration += () => OnRenderRequest?.Invoke();

            gm.State.OnGameStarted += () => StartButton_Visibility = Visibility.Collapsed;
            gm.State.OnGameStarted += () => RestartButton_Visibility = Visibility.Visible;
            gm.State.OnGameStarted += () => OptionsButton_Visibility = Visibility.Collapsed;

            gm.State.OnGameRestarted += () => StartButton_Visibility = Visibility.Visible;
            gm.State.OnGameRestarted += () => RestartButton_Visibility = Visibility.Collapsed;
            gm.State.OnGameRestarted += () => OptionsButton_Visibility = Visibility.Visible;

            gm.State.OnGameRestarted += () => OnRestartRequest?.Invoke();

            gm.OnScoreChange += () => this.Score = gm.Score;
            gm.GotFinalScore += score => sb.HandleNewScore(score, NameEntered!);
            #endregion  
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

        public string? NameEntered
        {
            get
            {
                if (string.IsNullOrWhiteSpace(field))
                    return "Guest";

                return field;
            }
    
            set;
        }
        
        #region Rendering things
        public Coords Dimensions => new(Cfg.Grid.Rows, Cfg.Grid.Columns);
        public IEnumerable<(Coords coords, SolidColorBrush)> GetRenderable()
        {
            foreach (var food in gm.FoodPool.ActiveFoods)
                yield return (food.Coords, Brushes.Red);

            yield return (gm.Snake.HeadPos, Brushes.RoyalBlue);

            foreach (var seg in gm.Snake.Body.Skip(1))
                yield return (seg.Coords, Brushes.LightSkyBlue);
        }
        #endregion

        #region ICommands
        public RelayCommand StartGameCommand { get; }
        public RelayCommand RestartGameCommand { get; }
        public RelayCommand ResetScoreboardCommand { get; }
        public RelayCommand OpenOptionsCommand { get; }
        #endregion

        #region Events and their handlers
        public void KeyDownHandler(KeyEventArgs e)
            => gm.QueuedDirection = e.Key switch
            {
                Key.Up or Key.W => Direction.Up,
                Key.Down or Key.S => Direction.Down,
                Key.Left or Key.A => Direction.Left,
                Key.Right or Key.D => Direction.Right,

                _ => gm.QueuedDirection
            };
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? OnRenderRequest;
        public event Action? OnRestartRequest;
        #endregion
        
        #region Visibility properties
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
        #endregion
    }
}