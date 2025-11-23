using System.Collections.ObjectModel;
using System.ComponentModel;
using SnakeGame.Helpers;
using System.Text.Json.Serialization;

namespace SnakeGame.Model
{
    public class Scoreboard
    {
        public int CurrentCount => VisualScores.Count;
        public Scoreboard()
        {
            VisualScores = [];
            ScoresMap = [];

            if (SerializeHelper.Deserialize(ref ScoresMap, SerializeOption.Scoreboard))
                InitializeScoreboard();
        }
        private readonly Dictionary<string, List<ScoreEntry>> ScoresMap;
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
        
        ///<returns><strong>bool</strong> whether it added a new entry </returns>
        public bool HandleNewScore(int newScore, string Name)
        {
            if (ScoresMap.TryGetValue(Name, out var scores) && scores.Any(s => s.Score >= newScore))
                return false; // quick tactical retreat if we already have higher scores with the same name on it.

            for (int i = 0; i < VisualScores.Count; i++)
            {
                int entryScore = VisualScores[i].Score;

                if (newScore >= entryScore) // if the new one is higher -> we put it above the i
                {
                    UpdateScores(new ScoreEntry(Name, newScore, DateTime.Now, Rank: i+1), i);
                    return true;
                }
            }

            // if it got to this point -> no lower scores found -> append to the end.
            UpdateScores(new ScoreEntry(Name, newScore, DateTime.Now, Rank: VisualScores.Count + 1));
            return true;
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
            }

            // Updating the ScoresMap dictionary
            if (!ScoresMap.TryAdd(newEntry.Name, [newEntry]))
                ScoresMap[newEntry.Name].Add(newEntry);
        }
        public void ResetScoreboard()
        {
            // this method should only be called after user clicked "yes Im sure" when pop up shows.

            VisualScores.Clear();
            ScoresMap.Clear();
        }
        private void UpdateRanksBelow(int startIdx)
        {
            for (int i = startIdx; i < VisualScores.Count; i++)
                VisualScores[i].Rank++;
        }
        public void SaveOnExit(CancelEventArgs? e = null)
        {
            // saving this method bc of the possible cancellation in the future, if I wanna add "are you sure??" window.
            // but the cancellation should happen before this method is called anyway, it depends on the new pop-up that I'll make.
            // so I'll just keep it for now till its done.

            SerializeHelper.Serialize(ScoresMap, SerializeOption.Scoreboard);
        }
        private void InitializeScoreboard()
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