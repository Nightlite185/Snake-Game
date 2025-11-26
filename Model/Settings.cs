using System.Windows.Media;
using System.Text.Json.Serialization;
namespace SnakeGame.Model
{
    public interface IDefaultable
    {
        public void ToDefault();
    }
    
    public class Settings: IDefaultable
    {
        public void ToDefault()
        {
            Snake.ToDefault();
            Grid.ToDefault();
            General.ToDefault();
            Theme.ToDefault();
        }
        
        #region Init things
        public Settings(bool GetDefault = false)
        {
            Snake = new();
            Grid = new();
            General = new();
            Theme = new();

            if (GetDefault) 
                ToDefault();
        }
        
        [JsonConstructor]
        public Settings()
        {
            Snake = new();
            Grid = new();
            General = new();
            Theme = new();
        }

        #endregion
        
        #region Section properties
        public SnakeSettings Snake { get; set; }
        public GridSettings Grid { get; set; }
        public GeneralSettings General { get; set; }
        public ThemeSettings Theme { get; set; }
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
            public const int DefSnakeSpeed = 50;
            public const int DefMaxFoods = 7;
            public const int DefFoodSpawnFreq = 7;
            #endregion
            public void ToDefault()
            {
                SnakeSpeed = DefSnakeSpeed;
                MaxActiveFoods = DefMaxFoods;
                FoodSpawnFreq = DefFoodSpawnFreq;
            }
            public int SnakeSpeed { get; set; }
            public int MaxActiveFoods { get; set; }
            public int FoodSpawnFreq { get; set; }
        }
        public class ThemeSettings: IDefaultable
        {
            public static readonly Color DefSnakeHeadColor = Colors.RoyalBlue;
            public static readonly Color DefSnakeBodyColor = Colors.LightSkyBlue;
            public static readonly Color DefFoodColor = Colors.Violet;
            public static readonly Color DefBackgroundColor = Colors.Transparent;
            public void ToDefault()
            {
                SnakeHeadColor = DefSnakeHeadColor;
                SnakeBodyColor = DefSnakeBodyColor;
                BackgroundColor = DefBackgroundColor;
                FoodColor = DefFoodColor;
            }
            public Color? SnakeHeadColor { get; set; }
            public Color? SnakeBodyColor { get; set; }
            public Color? BackgroundColor { get; set; }
            public Color? FoodColor { get; set; }
        }
        #endregion
    }
}