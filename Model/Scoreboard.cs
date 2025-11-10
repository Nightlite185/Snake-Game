using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnakeGame.Model
{
    public class Scoreboard
    {
        public Scoreboard()
        {
            VisualScores = [];
            ScoresMap = [];

            LoadOnInit();
        }
        private Dictionary<string, List<ScoreEntry>> ScoresMap;
        public record ScoreEntry(string Name, int Score, DateTime Time)
        {
            [JsonIgnore]
            public string StrPoints => $"{this.Score} pts.";

            [JsonIgnore]
            public string StrTime => $"{this.Time: dd/MM/yyyy, hh:mm tt}";
        }
        public ObservableCollection<ScoreEntry> VisualScores { get; set; }
        public void HandleNewScore(int newScore, string Name)
        {
            if (ScoresMap.TryGetValue(Name, out var scores) && scores.Any(s => s.Score >= newScore))
                return; // quick tactical retreat if we already have higher scores with the same name on it.

            ScoreEntry newEntry = new(Name, newScore, DateTime.Now);

            for (int i = 0; i < VisualScores.Count; i++)
            {
                int entryScore = VisualScores[i].Score;

                if (newScore >= entryScore) // if the new one is higher -> we put it above the i
                {
                    UpdateScores(newEntry, i);
                    return;
                }
            }

            // if it got to this point -> no lower scores found -> append to the end.
            UpdateScores(newEntry);
        }
        private void UpdateScores(ScoreEntry newEntry, int? idx = null) // if idx not given then add the end.
        {
            // Updating the Visuals (ObservableCollection)
            if (idx == null)
                VisualScores.Add(newEntry);

            else
                VisualScores.Insert((int)idx, newEntry);

            // Updating the ScoresMap dictionary
            if (!ScoresMap.TryAdd(newEntry.Name, [newEntry]))
                ScoresMap[newEntry.Name].Add(newEntry);
        }
        public void ResetScoreboard()
        {
            VisualScores.Clear();
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
            ScoresMap = JsonSerializer.Deserialize<Dictionary<string, List<ScoreEntry>>>(scores) ?? [];

            if (ScoresMap.Count > 0)
                InitializeScoreboard(ScoresMap); 
        }
        private void InitializeScoreboard(Dictionary<string, List<ScoreEntry>> ScoresMap) 
        {
            // I had to divide the work instead of inlining it in new()
            // bc if its squashed into one linq chain there,
            // it just wont order for some reason

            var scoresOrdered = ScoresMap.SelectMany(kvp => kvp.Value).OrderByDescending(e => e.Score);
            VisualScores = new(scoresOrdered);
        }
    }
}