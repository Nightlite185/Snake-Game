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
            Food.ToDefault();
            Theme.ToDefault();
        }
        
        #region Init things
        public Settings(bool GetDefault = false)
        {
            Snake = new();
            Grid = new();
            Food = new();
            Theme = new();

            if (GetDefault) 
                ToDefault();
        }
        
        [JsonConstructor]
        public Settings()
        {
            Snake = new();
            Grid = new();
            Food = new();
            Theme = new();
        }

        #endregion
        
        #region Section properties
        public SnakeSettings Snake { get; set; }
        public GridSettings Grid { get; set; }
        public FoodSettings Food { get; set; }
        public ThemeSettings Theme { get; set; }
        #endregion
        
        #region Section subclasses
        public class SnakeSettings: IDefaultable
        {
            #region defaults
            public const int DefStartingLength = 3;
            public const int DefSpeed = 50;
            public void ToDefault()
            {
                this.Speed = DefSpeed;
                this.StartingLength = DefStartingLength;
            }
            #endregion
            #region bounds
            public const double MinStartLength = 2; // yes, bounds need to be double bc xaml being a bitch about it and not accepting const ints for some reason
            public const double MinSpeed = 1;
            public const double MaxSpeed = 100;
            #endregion
            public int StartingLength { get; set; }
            public int Speed { get; set; }
        }
        public class GridSettings: IDefaultable
        {
            #region Defaults
            public const int DefRows = 15;
            public const int DefColumns = 15;
            public void ToDefault()
            {
                this.Rows = DefRows;
                this.Columns = DefColumns;
            }
            #endregion
            #region bounds
            public const double MinGridAny = 3;
            public const double MaxGridAny = 100;
            #endregion
            public int Rows { get; set; }
            public int Columns { get; set; }
        }
        public class FoodSettings: IDefaultable
        {
            #region defaults
            public const int DefMaxFoods = 7;
            public const int DefFoodSpawnFreq = 7;
            public void ToDefault()
            {
                
                MaxActiveFoods = DefMaxFoods;
                FoodSpawnFreq = DefFoodSpawnFreq;
            }
            #endregion
            #region bounds
            public const double MinFoodFreq = 1;
            public const double MaxFoodFreq = 10;
            public const double MinActiveFoodsLimit = 1;
            #endregion
            public int MaxActiveFoods { get; set; }
            public int FoodSpawnFreq { get; set; }
        }
        public class ThemeSettings: IDefaultable
        {
            #region defaults
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
            #endregion
            public Color? SnakeHeadColor { get; set; }
            public Color? SnakeBodyColor { get; set; }
            public Color? BackgroundColor { get; set; }
            public Color? FoodColor { get; set; }
        }
        #endregion
    }
}