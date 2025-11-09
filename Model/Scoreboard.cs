using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace SnakeGame.Model
{
    public class Scoreboard
    {
        public Scoreboard()
        {
            ScoreboardEntries = [];
            ScoresMap = [];

            LoadOnInit();
        }
        private Dictionary<string, List<int>> ScoresMap;
        public ObservableCollection<ScoreEntry> ScoreboardEntries { get; private set; }
        public void HandleNewScore(int newScore, string NameEntered)
        {
            if (ScoresMap.TryGetValue(NameEntered!, out var scores) && scores.Any(s => s >= newScore))
                return; // quick tactical retreat if we already have higher scores with the same name on it.

            for (int i = 0; i < ScoreboardEntries.Count; i++)
            {
                int entryScore = ScoreboardEntries[i].Score;

                if (newScore >= entryScore) // if the new one is higher -> we put it above the i
                {
                    UpdateScores(newScore, NameEntered, i);
                    return;
                }
            }

            // if it got to this point -> no lower scores found -> append to the end.
            UpdateScores(newScore, NameEntered);
        }
        private void UpdateScores(int newScore, string NameEntered, int? idx = null) // if idx not given then add the end.
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
        public void SaveOnExit(CancelEventArgs? e = null)
        {
            string scoresDir = GetScoresDir();
            string scoresPath = Path.Combine(scoresDir, "Scores.json");

            Directory.CreateDirectory(scoresDir);

            File.WriteAllText(scoresPath, JsonSerializer.Serialize(ScoresMap));
        }
        private void LoadOnInit()
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
    }
}