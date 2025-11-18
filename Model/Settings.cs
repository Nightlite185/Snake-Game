namespace SnakeGame.Model
{
    public interface IDefaultable
    {
        public void ToDefault();
    }
    public class Settings: IDefaultable
    {
        public Settings()
        {
            if (InitializeFromJson() == false) // if json init failed
                ToDefault(); // just build default settings
        }
        private bool InitializeFromJson()
        {

            return false;
        }
        public void ToDefault()
        {
            Snake.ToDefault();
            Grid.ToDefault();
            General.ToDefault();
            Theme.ToDefault();
        }
        public void ImportFrom(Settings other)
        {
            this.Snake = other.Snake;
            this.Grid = other.Grid;
            this.General = other.General;
            this.Theme = other.Theme;
        }
        public SnakeSettings Snake { get; set; }
        public GridSettings Grid { get; set; }
        public GeneralSettings General { get; set; }
        public ThemeSettings Theme { get; set; }
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