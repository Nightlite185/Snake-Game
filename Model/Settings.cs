using System.IO;
using System.Text.Json;

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
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SnakeGame"
            );
            string path = Path.Combine(dir, "Settings.json");

            Directory.CreateDirectory(dir);

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<Settings>(json);
        }
        public void ToDefault()
        {
            Snake.ToDefault();
            Grid.ToDefault();
            General.ToDefault();
            Theme.ToDefault();
        }

        public SnakeSettings Snake { get; set; } = null!; // just for roslyn to shut tf up ab nulls in ctor
        public GridSettings Grid { get; set; } = null!;
        public GeneralSettings General { get; set; } = null!;
        public ThemeSettings Theme { get; set; } = null!;
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
    }
}