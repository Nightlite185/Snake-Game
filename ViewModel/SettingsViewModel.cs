using SnakeGame.Helpers;
using SnakeGame.Model;
using System.ComponentModel;

namespace SnakeGame.ViewModel
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool notifyUI = false;
        
        /// <returns>a bool whether the method called INPC (PropertyChanged)</returns>
        protected void TryNotify<T>(ref T field, T value, object caller, string name)
        {
            if (notifyUI)
            {
                if (!EqualityComparer<T>.Default.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(caller, new PropertyChangedEventArgs(name));
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
        public int MaxSnakeStartLength => Math.Max(Rows, Columns) - 2; /* 
        REMEMBER TO CALL PROP_CHANGED ON THIS WHEN CHANGING ROWS OR COLS!!!!
        this one is tricky bc it also depends (both ways) on the snake's startDirection
        I could block those options (fallback to default) IF the snake's direction is chosen customly.
        And a bool checkbox that tells me if the user chose custom snake's dir. (yes -> lock this, to default.) */
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
                TryNotify(ref field, value, this, nameof(Rows));
                if (updateStates) UpdateChangedState();
            }
        }

        public int Columns
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(Columns));
                if (updateStates) UpdateChangedState();
            }
        }

        // GENERAL
        public int TickLength
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(TickLength));
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

        public int FoodSpawningFrequency
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(FoodSpawningFrequency));
                if (updateStates) UpdateChangedState();
            }
        }

        // THEME

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
            this.TickLength = OGSettings.General.TickLength;
            this.MaxActiveFoods = OGSettings.General.MaxActiveFoods;
            this.FoodSpawningFrequency = OGSettings.General.FoodSpawningFrequency;

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
            OGSettings.General.TickLength = this.TickLength;
            OGSettings.General.MaxActiveFoods = this.MaxActiveFoods;
            OGSettings.General.FoodSpawningFrequency = this.FoodSpawningFrequency;
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
            this.TickLength = Settings.GeneralSettings.DefTickLength;
            this.MaxActiveFoods = Settings.GeneralSettings.DefMaxFoods;
            this.FoodSpawningFrequency = Settings.GeneralSettings.DefSpawningFreq;

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