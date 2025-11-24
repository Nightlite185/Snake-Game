using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SnakeGame.Helpers;
using SnakeGame.Model;
using SnakeGame.View;

namespace SnakeGame.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged, IDisposable
    {
        private GameManager? gm;
        public readonly Scoreboard sb;
        private GameState State { get; init; }
        private Settings cfg = null!;
        public void CleanupOnExit()
        {
            
            // Dispose(); // commenting it out bc for now it doesnt do anything yet.
            sb.SaveOnExit(); 
        }
        public void Dispose()
        {
            // TODO:: dispose of any cts in the future HERE IN THIS METHOD
        }
        
        #region Event things
        private void HookGMEvents()
        {
            gm!.OnScoreChange += ScoreChangeHandler;
            gm!.GotFinalScore += FinalScoreHandler;
            gm!.OnIteration += OnRenderRequest;
        }
        private void UnhookGMEvents()
        {
            gm!.OnScoreChange -= ScoreChangeHandler;
            gm!.GotFinalScore -= FinalScoreHandler;
            gm!.OnIteration -= OnRenderRequest;
        }
        private void NotifyStateDepButtons()
        {
            StartGameCommand.ScreamCanExecuteChanged();
            RestartGameCommand.ScreamCanExecuteChanged();
            OpenOptionsCommand.ScreamCanExecuteChanged();
        }
        private void ScoreChangeHandler() => this.Score = gm!.Score;
        private void FinalScoreHandler(int score)
        {
            bool added = sb.HandleNewScore(score, NameEntered!);
            
            if (sb.CurrentCount == 1 && added)
                ResetScoreboardCommand.ScreamCanExecuteChanged();
        }
        #endregion
        public GameViewModel()
        {
            // Objects init
            if (!SerializeHelper.Deserialize(ref cfg, SerializeOption.Settings))
                cfg = new Settings(GetDefault: true);

            sb = new Scoreboard();
            State = new();
            
            // Button Visibility init
            StartButton_Visibility = Visibility.Visible;
            RestartButton_Visibility = Visibility.Collapsed;
            OptionsButton_Visibility = Visibility.Visible;

            #region ICommands
            StartGameCommand = new RelayCommand(
                executeAsync: async () =>
                {
                    gm = new(cfg, State);
                    HookGMEvents();

                    await gm!.RunGameAsync();
                },
                canExecute: () => State.Current == GameStates.NotStarted
            );

            RestartGameCommand = new RelayCommand(
                execute: () =>
                {
                    State.Restart();
                    UnhookGMEvents();

                    gm = null;
                    Score = 0;

                    RestartGameCommand!.ScreamCanExecuteChanged();
                    OnRestartRequest?.Invoke(); // request for view to clear canva visuals.
                },
                canExecute: () => State.Current != GameStates.NotStarted
            );

            ResetScoreboardCommand = new RelayCommand(
                execute: () => 
                {
                    sb.ResetScoreboard();
                    ResetScoreboardCommand!.ScreamCanExecuteChanged();
                },
                canExecute: () => sb.VisualScores.Count > 0
            );
            
            OpenOptionsCommand = new RelayCommand(
                execute: () => 
                {
                    SettingsViewModel setVM = new(cfg);
                    var win = new OptionsWindow(setVM);
                    
                    win.ShowDialog();
                },
                canExecute: () => State.Current == GameStates.NotStarted
            );
            #endregion

            #region Event Subscribers

            // BUTTONS ON START 
            State.OnGameStarted += NotifyStateDepButtons;
            State.OnGameStarted += () => StartButton_Visibility = Visibility.Collapsed;
            State.OnGameStarted += () => RestartButton_Visibility = Visibility.Visible;
            State.OnGameStarted += () => OptionsButton_Visibility = Visibility.Collapsed;

            // BUTTONS ON RESTART
            State.OnGameRestarted += NotifyStateDepButtons;
            State.OnGameRestarted += () => StartButton_Visibility = Visibility.Visible;
            State.OnGameRestarted += () => RestartButton_Visibility = Visibility.Collapsed;
            State.OnGameRestarted += () => OptionsButton_Visibility = Visibility.Visible;
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
        public Coords Dimensions => new(cfg.Grid.Rows, cfg.Grid.Columns);
        public IEnumerable<(Coords coords, SolidColorBrush)> GetRenderable()
        {
            foreach (var food in gm!.FoodPool.ActiveFoods)
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
        {
            if (State.Current == GameStates.Running)
                gm!.QueuedDirection = e.Key switch // only do this when the game is running and when we're not currently focused on the textbox
                {
                    Key.Up or Key.W => Direction.Up,
                    Key.Down or Key.S => Direction.Down,
                    Key.Left or Key.A => Direction.Left,
                    Key.Right or Key.D => Direction.Right,

                    _ => gm.QueuedDirection
                };
        }

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