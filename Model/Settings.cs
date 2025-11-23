using System.Text.Json.Serialization;
namespace SnakeGame.Model
{
    public interface IDefaultable
    {
        public void ToDefault();
    }
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify<T>(ref T field, T value, object caller, string name)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(caller, new PropertyChangedEventArgs(name));
            }
        }
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
        public class SnakeSettings: NotifyBase, IDefaultable
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
            public int StartingLength
            { 
                get; 
                set => Notify(ref field, value, this, nameof(StartingLength));
            }
            public Direction StartingDirection
            { 
                get;
                set => Notify(ref field, value, this, nameof(StartingDirection));
            }
        }
        public class GridSettings: NotifyBase, IDefaultable
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
            public int Rows
            { 
                get; 
                set => Notify(ref field, value, this, nameof(Rows));
            }
            public int Columns
            { 
                get; 
                set => Notify(ref field, value, this, nameof(Columns));
            }
        }
        public class GeneralSettings: NotifyBase, IDefaultable
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
            public int TickLength
            { 
                get; 
                set => Notify(ref field, value, this, nameof(TickLength));
            }
            public int MaxActiveFoods
            { 
                get; 
                set => Notify(ref field, value, this, nameof(MaxActiveFoods));
            }
            public int FoodSpawningFrequency
            { 
                get; 
                set => Notify(ref field, value, this, nameof(FoodSpawningFrequency));
            }
        }
        public class ThemeSettings: NotifyBase, IDefaultable
        {
            public void ToDefault()
            {
                
            }
        }
        #endregion
    }
}