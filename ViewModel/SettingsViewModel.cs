using SnakeGame.Helpers;
using SnakeGame.Model;
using System.ComponentModel;

namespace SnakeGame.ViewModel
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool liveBinding;
        
        /// <returns>a bool whether the method called INPC (PropertyChanged) </returns>
        protected bool TryNotify<T>(ref T field, T value, object caller, string name)
        {
            if (liveBinding)
            {
                if (!EqualityComparer<T>.Default.Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(caller, new PropertyChangedEventArgs(name));
                    return true;
                }

                return false;
            }

            else 
                field = value;
                return false;
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
                if (TryNotify(ref field, value, this, nameof(StartingLength)))
                    UpdateChangedState();
            }
        }

        public Direction StartingDirection
        {
            get;
            set
            {
                if (TryNotify(ref field, value, this, nameof(StartingDirection)))
                    UpdateChangedState();
            }
        }

        // GRID
        public int Rows
        {
            get;
            set
            {
                if (TryNotify(ref field, value, this, nameof(Rows)))
                    UpdateChangedState();
            }
        }

        public int Columns
        {
            get;
            set
            {
                if (TryNotify(ref field, value, this, nameof(Columns)))
                    UpdateChangedState();
            }
        }

        // GENERAL
        public int TickLength
        {
            get;
            set
            {
                if (TryNotify(ref field, value, this, nameof(TickLength)))
                    UpdateChangedState();
            }
        }

        public int MaxActiveFoods
        {
            get;
            set
            {
                if (TryNotify(ref field, value, this, nameof(MaxActiveFoods)))
                    UpdateChangedState();
            }
        }

        public int FoodSpawningFrequency
        {
            get;
            set
            {
                if (TryNotify(ref field, value, this, nameof(FoodSpawningFrequency)))
                    UpdateChangedState();
            }
        }

        // THEME

        #endregion

        #region LOAD / SAVE METHODS
        private void LoadFromOG()
        {
            liveBinding = false;

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

            liveBinding = true;
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
            liveBinding = false;

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

            liveBinding = true;
        }
        #endregion
        public SettingsViewModel(Settings settings)
        {
            this.OGSettings = settings;
            LoadFromOG();

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
        public void UpdateChangedState() // MOVE THIS TO THE UI SETTERS THIS IS GENIUS ARCHITECTURE SHIT
        {
            IsChanged = true;

            if (IsDraftDefault)
                IsDraftDefault = false;
        }
    }
}