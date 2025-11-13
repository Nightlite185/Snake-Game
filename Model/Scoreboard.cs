using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

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
        public record ScoreEntry(string Name, int Score, DateTime Time, int Rank = 0) : INotifyPropertyChanged
        {
            [JsonIgnore]
            public int Rank
            {
                get;
                set
                {
                    if (value != field)
                    {
                        field = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rank)));
                    }
                }
            } = Rank;

            [JsonIgnore]
            public string StrPoints => $"{this.Score} pts.";

            [JsonIgnore]
            public string StrTime => $"{this.Time: dd/MM/yyyy, hh:mm tt}";

            public event PropertyChangedEventHandler? PropertyChanged;
        }
        public ObservableCollection<ScoreEntry> VisualScores { get; set; }
        public void HandleNewScore(int newScore, string Name)
        {
            if (ScoresMap.TryGetValue(Name, out var scores) && scores.Any(s => s.Score >= newScore))
                return; // quick tactical retreat if we already have higher scores with the same name on it.

            for (int i = 0; i < VisualScores.Count; i++)
            {
                int entryScore = VisualScores[i].Score;

                if (newScore >= entryScore) // if the new one is higher -> we put it above the i
                {
                    UpdateScores(new ScoreEntry(Name, newScore, DateTime.Now, Rank: i+1), i);
                    return;
                }
            }

            // if it got to this point -> no lower scores found -> append to the end.
            UpdateScores(new ScoreEntry(Name, newScore, DateTime.Now, Rank: VisualScores.Count));
        }
        private void UpdateScores(ScoreEntry newEntry, int? idx = null) // if idx not given then add the end.
        {
            // Updating the Visuals (ObservableCollection)
            if (idx == null)
                VisualScores.Add(newEntry);

            else
            {
                VisualScores.Insert((int)idx, newEntry);
                UpdateRanksBelow(startIdx: (int)idx+1);

                int i = 0;
                
                foreach (var entry in VisualScores)
                    if (entry.Rank != ++i) 
                        throw new Exception("entry's rank is wrong lol");
            }

            // Updating the ScoresMap dictionary
            if (!ScoresMap.TryAdd(newEntry.Name, [newEntry]))
                ScoresMap[newEntry.Name].Add(newEntry);
        }
        public void ResetScoreboard()
        {
            VisualScores.Clear();
            ScoresMap.Clear();
        }
        private void UpdateRanksBelow(int startIdx)
        {
            for (int i = startIdx; i < VisualScores.Count; i++)
                VisualScores[i].Rank++;
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
            var orderedEntries = ScoresMap.Values
                .SelectMany(x => x)
                .OrderByDescending(e => e.Score);

            int newRank = 0;

            foreach (var entry in orderedEntries)
                entry.Rank = ++newRank;
            
            VisualScores = new(orderedEntries);
        }
    }
}