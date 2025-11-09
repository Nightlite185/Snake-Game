using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
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
        public GameViewModel()
        {
            ScoreboardEntries = [];
            ScoresMap = [];
            gameManager = new();
            StartButton_Visibility = Visibility.Visible;
            RestartButton_Visibility = Visibility.Collapsed;
            OptionsButton_Visibility = Visibility.Visible;

            LoadOnInit();

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
            gameManager.GotFinalScore += HandleNewScore;
            #endregion  
        }

        #region Score things !! TO DO: ENCAPSULATE IT INTO NEW CLASS !!
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
        private Dictionary<string, List<int> > ScoresMap;
        public ObservableCollection<ScoreEntry> ScoreboardEntries { get; private set; }
        private void HandleNewScore(int newScore)
        {
            if (ScoresMap.TryGetValue(NameEntered!, out var scores) && scores.Any(s => s >= newScore))
                return; // quick tactical retreat if we already have higher scores with the same name on it.

            for (int i = 0; i < ScoreboardEntries.Count; i++)
            {
                int entryScore = ScoreboardEntries[i].Score;

                if (newScore >= entryScore) // if the new one is higher -> we put it above the i
                {
                    UpdateScores(newScore, i);
                    return;
                }
            }

            // if it got to this point -> no lower scores found -> append to the end.
            UpdateScores(newScore);
        }
        private void UpdateScores(int newScore, int? idx = null) // if idx not given then add the end.
        {
            // Updating the Visuals (ObservableCollection)
            if (idx == null)
                ScoreboardEntries.Add(new ScoreEntry(newScore, NameEntered!));
            
            else
                ScoreboardEntries.Insert((int)idx, new ScoreEntry(newScore, NameEntered!));

            // Updating the ScoresMap dictionary
            if (ScoresMap.TryAdd(NameEntered!, []))
                ScoresMap[NameEntered!] = [newScore];

            else
                ScoresMap[NameEntered!].Add(newScore);
        }
        public void ResetScoreboard()
        {
            ScoreboardEntries.Clear();
            ScoresMap.Clear();
        }
        private static string GetScoresDir()
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SnakeGame"
            );
        public void SaveOnExit(CancelEventArgs e)
        {
            string scoresDir = GetScoresDir();
            string scoresPath = Path.Combine(scoresDir, "Scores.json");

            Directory.CreateDirectory(scoresDir);

            File.WriteAllText(scoresPath, JsonSerializer.Serialize(ScoresMap));
        }
        public void LoadOnInit()
        {
            string scoresDir = GetScoresDir();
            string scoresPath = Path.Combine(scoresDir, "Scores.json");

            Directory.CreateDirectory(scoresDir);

            if (!File.Exists(scoresPath))
                return;

            string scores = File.ReadAllText(scoresPath);
            ScoresMap = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(scores) ?? [];

            if (ScoresMap.Count > 0)
                InitializeScoreboard(ScoresMap);
        }
        private void InitializeScoreboard(Dictionary<string, List<int>> ScoresMap) // initializing scoreboard with loaded data.
        {
            ScoreboardEntries = new(ScoresMap
                .SelectMany(kvp => kvp.Value
                .Select(score => new ScoreEntry(score, nick: kvp.Key)))
                .OrderByDescending(x => x.Score));

            // TO DO:: Add datetime support ScoreMap dict when loading from JSON.
        }
        #endregion
        
        #region Rendering things
        public static Coords Dimensions => new(GameManager.gridRows, GameManager.gridColumns);
        public IEnumerable<(Coords coords, SolidColorBrush)> GetRenderable()
        {
            foreach (var food in gameManager.FoodPool.ActiveFoods)
                yield return (food.Coords, Brushes.Red);

            yield return (gameManager.Snake.HeadPos, Brushes.RoyalBlue);

            foreach (var seg in gameManager.Snake.Body.Skip(1))
                yield return (seg.Coords, Brushes.LightSkyBlue);
        }
        #endregion

        #region ICommands
        public ICommand StartGameCommand { get; }
        public ICommand RestartGameCommand { get; }
        #endregion
        
        #region Events and their handlers
        public void KeyDownHandler(KeyEventArgs e)
            => gameManager.QueuedDirection = e.Key switch
            {
                Key.Up or Key.W => Direction.Up,
                Key.Down or Key.S => Direction.Down,
                Key.Left or Key.A => Direction.Left,
                Key.Right or Key.D => Direction.Right,

                _ => gameManager.QueuedDirection
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