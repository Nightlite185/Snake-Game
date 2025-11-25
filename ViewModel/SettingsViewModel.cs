using SnakeGame.Helpers;
using SnakeGame.Model;
using System.ComponentModel;
using System.Windows.Media;

namespace SnakeGame.ViewModel
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool notifyUI = false;
        
        /// <returns>a bool whether the method called INPC (PropertyChanged)</returns>
        protected void TryNotify<T>(ref T field, T value, object caller, params string[] ToNotifyList)
        {
            if (notifyUI)
            {
                if (!EqualityComparer<T>.Default.Equals(field, value))
                {
                    field = value;

                    foreach (string target in ToNotifyList)
                        PropertyChanged?.Invoke(caller, new PropertyChangedEventArgs(target));
                }
            }

            else 
                field = value;
        }
    }
    public class SettingsViewModel : NotifyBase
    {
        #region PRIVATE PROPERTIES
        private bool IsChanged
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    SaveChangesCommand.ScreamCanExecuteChanged();
                    DiscardChangesCommand.ScreamCanExecuteChanged();
                }
            }
        }
        private bool IsDraftDefault
        {
            get;
            set
            {
                field = value;
                ResetToDefaultCommand.ScreamCanExecuteChanged();
            }
        }
        private bool isOGDefault;
        private bool updateStates;
        private Settings OGSettings { get; set; }
        #endregion

        #region UI SETTINGS BINDING PROPERTIES

        // SNAKE 
        public int MaxSnakeStartLength => Math.Max(Rows, Columns) - 2;
        public int StartingLength
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(StartingLength));
                if (updateStates) UpdateChangedState();
            }
        }
        public Direction StartingDirection
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(StartingDirection));
                if (updateStates) UpdateChangedState();
            }
        }

        // GRID
        public int Rows
        {
            get;
            set
            {
                string[] ToNotify = [nameof(Rows), nameof(MaxChoosableMaxActiveFoods), nameof(MaxSnakeStartLength)];

                TryNotify(ref field, value, this, ToNotify);
                if (updateStates) UpdateChangedState();
            }
        }
        public int Columns
        {
            get;
            set
            {
                string[] ToNotify = [nameof(Columns), nameof(MaxChoosableMaxActiveFoods), nameof(MaxSnakeStartLength)];
                
                TryNotify(ref field, value, this, ToNotify);
                if (updateStates) UpdateChangedState();
            }
        }

        // GENERAL
        public int SnakeSpeed
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(SnakeSpeed));
                if (updateStates) UpdateChangedState();
            }
        }
        public int MaxActiveFoods
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(MaxActiveFoods));
                if (updateStates) UpdateChangedState();
            }
        }
        public int MaxChoosableMaxActiveFoods => Rows * Columns;
        public int FoodSpawnFreq
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(FoodSpawnFreq));
                if (updateStates) UpdateChangedState();
            }
        }

        // THEME
        public Color? SnakeHeadColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(SnakeHeadColor));
                if (updateStates) UpdateChangedState();
            }
        } 
        public Color? SnakeBodyColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(SnakeBodyColor));
                if (updateStates) UpdateChangedState();
            }
        }
        public Color? BackgroundColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(BackgroundColor));
                if (updateStates) UpdateChangedState();
            }
        }
        public Color? FoodColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(FoodColor));
                if (updateStates) UpdateChangedState();
            }
        }
        #endregion

        #region LOAD / SAVE METHODS
        private void LoadFromOG()
        {
            updateStates = false;

            // SNAKE
            this.StartingLength = OGSettings.Snake.StartingLength;
            this.StartingDirection = OGSettings.Snake.StartingDirection;

            // GRID
            this.Rows = OGSettings.Grid.Rows;
            this.Columns = OGSettings.Grid.Columns;

            // GENERAL
            this.SnakeSpeed = OGSettings.General.SnakeSpeed;
            this.MaxActiveFoods = OGSettings.General.MaxActiveFoods;
            this.FoodSpawnFreq = OGSettings.General.FoodSpawnFreq;

            // THEME
            this.SnakeHeadColor = OGSettings.Theme.SnakeHeadColor;
            this.SnakeBodyColor = OGSettings.Theme.SnakeBodyColor;
            this.BackgroundColor = OGSettings.Theme.BackgroundColor;
            this.FoodColor = OGSettings.Theme.FoodColor;
            updateStates = true;
        }
        private void SaveToOG()
        {
            // SNAKE
            OGSettings.Snake.StartingLength = this.StartingLength;
            OGSettings.Snake.StartingDirection = this.StartingDirection;

            // GRID
            OGSettings.Grid.Rows = this.Rows;
            OGSettings.Grid.Columns = this.Columns;

            // GENERAL
            OGSettings.General.SnakeSpeed = this.SnakeSpeed;
            OGSettings.General.MaxActiveFoods = this.MaxActiveFoods;
            OGSettings.General.FoodSpawnFreq = this.FoodSpawnFreq;

            // THEME
            OGSettings.Theme.SnakeHeadColor = SnakeHeadColor;
            OGSettings.Theme.SnakeBodyColor = SnakeBodyColor;
            OGSettings.Theme.BackgroundColor = BackgroundColor;
            OGSettings.Theme.FoodColor = FoodColor;
        }
        private void DraftToDefault()
        {
            updateStates = false;

            // SNAKE
            this.StartingLength = Settings.SnakeSettings.DefStartingLength;
            this.StartingDirection = Settings.SnakeSettings.DefStartDirection;

            // GRID
            this.Rows = Settings.GridSettings.DefRows;
            this.Columns = Settings.GridSettings.DefColumns;

            // GENERAL
            this.SnakeSpeed = Settings.GeneralSettings.DefSnakeSpeed;
            this.MaxActiveFoods = Settings.GeneralSettings.DefMaxFoods;
            this.FoodSpawnFreq = Settings.GeneralSettings.DefFoodSpawnFreq;

            // THEME
            this.SnakeBodyColor = Settings.ThemeSettings.DefSnakeBodyColor;
            this.SnakeHeadColor = Settings.ThemeSettings.DefSnakeHeadColor;
            this.BackgroundColor = Settings.ThemeSettings.DefBackgroundColor;
            this.FoodColor = Settings.ThemeSettings.DefFoodColor;

            updateStates = true;
        }
        #endregion
        public SettingsViewModel(Settings settings)
        {
            this.OGSettings = settings;
            LoadFromOG();
            notifyUI = true;

            #region ICommands
            SaveChangesCommand = new RelayCommand(
                execute: () =>
                {
                    SaveToOG();
                    SerializeHelper.Serialize(OGSettings, SerializeOption.Settings);

                    IsChanged = false;

                    if (IsDraftDefault)
                        isOGDefault = true;

                    else if (isOGDefault && !IsDraftDefault)
                        isOGDefault = false;
                },
                canExecute: () => IsChanged
            );

            ResetToDefaultCommand = new RelayCommand(
                execute: () =>
                {
                    // TO DO:: maybe open some pop-up like "you sure u wanna reset???"
                    DraftToDefault();

                    IsChanged = true;
                    IsDraftDefault = true;
                },
                canExecute: () => !IsDraftDefault
            );

            DiscardChangesCommand = new RelayCommand(
                execute: () =>
                {
                    LoadFromOG();
                    IsChanged = false;

                    if (IsDraftDefault)          // if it was default and we're reverting those changes now
                        IsDraftDefault = false; // we turn it back to false.

                    else if (isOGDefault || !IsDraftDefault) // if og is default, and we are discarding any changes made
                        IsDraftDefault = true;              // -> we're actually reverting draft to default as well
                },
                canExecute: () => IsChanged
            );
            #endregion
        }

        #region ICommands
        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand DiscardChangesCommand { get; }
        public RelayCommand ResetToDefaultCommand { get; }
        #endregion
        public void UpdateChangedState()
        {
            IsChanged = true;

            if (IsDraftDefault)
                IsDraftDefault = false;
        }
    }
}