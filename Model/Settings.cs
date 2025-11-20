using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SnakeGame.Model
{
    public interface IDefaultable
    {
        public void ToDefault();
    }
    public class Settings: IDefaultable
    {
        public Settings(bool GetDefault = false)
        {
            Snake = new();
            Grid = new();
            General = new();
            Theme = new();

            if (GetDefault) 
                ToDefault();
        }
        public static Settings? TryInitFromJson()
        {
            string path = Path.Combine(GetDirectory, "Settings.json");

            Directory.CreateDirectory(GetDirectory);

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<Settings>(json);
        }
        public void Serialize()
        {
            string path = Path.Combine(GetDirectory, FileName);

            Directory.CreateDirectory(GetDirectory);
            File.WriteAllText(path, JsonSerializer.Serialize(this));
        }
        [JsonIgnore]
        private static string GetDirectory => 
            Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SnakeGame");
        [JsonIgnore]
        private readonly string FileName = "Settings.json";
        public void ToDefault()
        {
            Snake.ToDefault();
            Grid.ToDefault();
            General.ToDefault();
            Theme.ToDefault();
        }
        
        #region Section properties
        public SnakeSettings Snake { get; set; } = null!; // just for roslyn to shut tf up ab nulls in ctor
        public GridSettings Grid { get; set; } = null!;
        public GeneralSettings General { get; set; } = null!;
        public ThemeSettings Theme { get; set; } = null!;
        #endregion
        
        #region Section subclasses
        public class SnakeSettings: IDefaultable
        {
            #region defaults
            public const int DefStartingLength = 3;
            public const Direction DefStartDirection = Direction.Up;
            #endregion
            public void ToDefault()
            {
                this.StartingLength = DefStartingLength;
                this.StartingDirection = DefStartDirection;
            }
            public int StartingLength { get; set; }
            public Direction StartingDirection { get; set; }
        }
        public class GridSettings: IDefaultable
        {
            #region Defaults
            public const int DefRows = 15;
            public const int DefColumns = 15;
            #endregion
            public void ToDefault()
            {
                this.Rows = DefRows;
                this.Columns = DefColumns;
            }
            public int Rows { get; set; }
            public int Columns { get; set; }
        }
        public class GeneralSettings: IDefaultable
        {
            #region defaults
            public const int DefTickLength = 330;
            public const int DefMaxFoods = 7;
            public const int DefSpawningFreq = 3;
            #endregion
            public void ToDefault()
            {
                TickLength = DefTickLength;
                MaxActiveFoods = DefMaxFoods;
                FoodSpawningFrequency = DefSpawningFreq;
            }
            public int TickLength { get; set; }
            public int MaxActiveFoods { get; set; }
            public int FoodSpawningFrequency { get; set; }
        }
        public class ThemeSettings: IDefaultable
        {
            public void ToDefault()
            {
                
            }
        }
        #endregion
    }
}