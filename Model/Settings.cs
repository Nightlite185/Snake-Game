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
            public const int DefSnakeSpeed = 330;
            public const int DefMaxFoods = 7;
            public const int DefFoodSpawnFreq = 3;
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
            public void ToDefault()
            {
                
            }
        }
        #endregion
    }
}