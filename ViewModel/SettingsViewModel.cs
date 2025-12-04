using SnakeGame.Helpers;
using SnakeGame.Model;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace SnakeGame.ViewModel
{
    public abstract class NotifyBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INPC implementation
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
        #endregion
        
        #region INDEI implementation
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        private const string PropNotFoundMess = "No such property found in VM";
        public bool HasErrors => activeErrors.Any(kvp => kvp.Value.Count > 0);
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return Array.Empty<string>();
            
            if (activeErrors.TryGetValue(propertyName, out var result))
                return result;

            else throw new ArgumentException(PropNotFoundMess, nameof(propertyName));
        }
        protected Dictionary<string, List<string>> activeErrors = null!;
        protected void AddError(string propertyName, string message)
        {
            if (activeErrors.TryGetValue(propertyName, out var list) && !list.Contains(message))
            {
                list.Add(message);
                ErrorsChanged?.Invoke(this, new(propertyName));
                
            }

            else throw new ArgumentException(PropNotFoundMess, nameof(propertyName));
        }
        protected void ClearErrors(string propertyName)
        {
            if (activeErrors.TryGetValue(propertyName, out var list))
            {
                list.Clear();
                ErrorsChanged?.Invoke(this, new(propertyName));
            }
            
            else throw new ArgumentException(PropNotFoundMess, nameof(propertyName));
        }
        #endregion
    }
    public class SettingsViewModel : NotifyBase
    {
        public Visibility PopUp_Visibility
        {
            get; private set => TryNotify(ref field, value, this, nameof(PopUp_Visibility));
        } = Visibility.Collapsed;
        public bool IsChanged
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
        
        #region PRIVATE PROPERTIES
        private bool IsDraftDefault
        {
            get;
            set
            {
                field = value;
                ResetToDefaultCommand.ScreamCanExecuteChanged();
            }
        }
        private bool isOGDefault = false;
        private bool updateStates;
        private Settings OGSettings { get; set; }
        #endregion

        #region UI SETTINGS BINDING PROPERTIES

        // SNAKE 
        public int MaxSnakeStartLength => Math.Max(Rows, Columns) - 2;
        private const int MinSnakeStartLength = 2;
        public int SnakeSpeed
        {
            get;
            set
            {
                ClearErrors(nameof(SnakeSpeed));
                
                var result = Validation.Int(value, 1, 100); // TO DO:: name magic numbers later
                
                if (result != ValidationResult.Valid)
                {
                    AddError(nameof(SnakeSpeed), GetErrorMessage(result, nameof(SnakeSpeed), "1")); // TO DO:: fix this, it only works one way
                    SaveChangesCommand.ScreamCanExecuteChanged();
                }
                
                TryNotify(ref field, value, this, nameof(SnakeSpeed));
                if (updateStates) UpdateChangedState();
            }
        }
        public int StartingLength
        {
            get;
            set
            {
                ClearErrors(nameof(StartingLength));

                var result = Validation.Int(value, MinSnakeStartLength, MaxSnakeStartLength);

                if (result != ValidationResult.Valid)
                {
                    AddError(nameof(StartingLength), GetErrorMessage(result, nameof(StartingLength), "2"));
                    SaveChangesCommand.ScreamCanExecuteChanged();
                }

                TryNotify(ref field, value, this, nameof(StartingLength));
                if (updateStates) UpdateChangedState();
            }
        }

        // GRID
        public int Rows
        {
            get;
            set
            {
                ClearErrors(nameof(Rows));
                
                var result = Validation.Int(value, 3, 100);
                
                if (result != ValidationResult.Valid)
                {
                    AddError(nameof(Rows), GetErrorMessage(result, nameof(Rows), "3"));
                    SaveChangesCommand.ScreamCanExecuteChanged();
                }
                
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
                ClearErrors(nameof(Columns));
                
                var result = Validation.Int(value, 3, 100);
                
                if (result != ValidationResult.Valid)
                {
                    AddError(nameof(Columns), GetErrorMessage(result, nameof(Columns), "3"));
                    SaveChangesCommand.ScreamCanExecuteChanged();
                }
                
                string[] ToNotify = [nameof(Columns), nameof(MaxChoosableMaxActiveFoods), nameof(MaxSnakeStartLength)];
                
                TryNotify(ref field, value, this, ToNotify);
                if (updateStates) UpdateChangedState();
            }
        }

        // FOOD
        public int MaxActiveFoods
        {
            get;
            set
            {
                ClearErrors(nameof(MaxActiveFoods));
                
                var result = Validation.Int(value, 1, MaxChoosableMaxActiveFoods);
                
                if (result != ValidationResult.Valid)
                {
                    AddError(nameof(MaxActiveFoods), GetErrorMessage(result, nameof(MaxActiveFoods), "1"));
                    SaveChangesCommand.ScreamCanExecuteChanged();
                }
                
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
                ClearErrors(nameof(FoodSpawnFreq));
                
                var result = Validation.Int(value, 1, 10);
                
                if (result != ValidationResult.Valid)
                {
                    AddError(nameof(FoodSpawnFreq), GetErrorMessage(result, nameof(FoodSpawnFreq), "1"));
                    SaveChangesCommand.ScreamCanExecuteChanged();
                }
                
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

            // GRID
            this.Rows = OGSettings.Grid.Rows;
            this.Columns = OGSettings.Grid.Columns;

            // GENERAL
            this.SnakeSpeed = OGSettings.Snake.Speed;
            this.MaxActiveFoods = OGSettings.Food.MaxActiveFoods;
            this.FoodSpawnFreq = OGSettings.Food.FoodSpawnFreq;

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
            OGSettings.Snake.Speed = this.SnakeSpeed;

            // GRID
            OGSettings.Grid.Rows = this.Rows;
            OGSettings.Grid.Columns = this.Columns;

            // GENERAL
            OGSettings.Food.MaxActiveFoods = this.MaxActiveFoods;
            OGSettings.Food.FoodSpawnFreq = this.FoodSpawnFreq;

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
            this.SnakeSpeed = Settings.SnakeSettings.DefSpeed;

            // GRID
            this.Rows = Settings.GridSettings.DefRows;
            this.Columns = Settings.GridSettings.DefColumns;

            // FOOD
            this.MaxActiveFoods = Settings.FoodSettings.DefMaxFoods;
            this.FoodSpawnFreq = Settings.FoodSettings.DefFoodSpawnFreq;

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
            InitializeErrorsDict();
            LoadFromOG();
            notifyUI = true;


            #region ICommands

            // Regular options ICommands 
            SaveChangesCommand = new(
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
                canExecute: () => IsChanged && !HasErrors
            );
            
            ResetToDefaultCommand = new(
                execute: () =>
                {
                    // TO DO:: maybe open some pop-up like "you sure u wanna reset???"
                    DraftToDefault();

                    IsChanged = true;
                    IsDraftDefault = true;
                },
                canExecute: () => !IsDraftDefault
            );
            
            DiscardChangesCommand = new(
                execute: () =>
                {
                    LoadFromOG();
                    IsChanged = false;

                    if (IsDraftDefault)          // if it was default and we're reverting those changes now
                        IsDraftDefault = false; // we turn it back to false.

                    else if (isOGDefault && !IsDraftDefault) // if og is default, and we are discarding any changes made
                        IsDraftDefault = true;              // -> we're actually reverting draft to default as well
                },
                canExecute: () => IsChanged
            );
            
            // Pop-up buttons ICommands
            SaveInPopUpCommand = new(
                execute: () =>
                {
                    SaveChangesCommand.Execute();
                    CloseWinRequest?.Invoke();
                },
                canExecute:() => true
            );
            
            DiscardInPopUpCommand = new(
                execute: () =>
                {
                    DiscardChangesCommand.Execute();
                    CloseWinRequest?.Invoke();
                },
                canExecute:() => true
            );

            CancelInPopUpCommand = new(
                execute: () => ClosePopUpRequest?.Invoke(),
                canExecute: () => true
            );

            #endregion
        }

        private void InitializeErrorsDict() => activeErrors = new()
        {
            // SNAKE
            [nameof(StartingLength)] = [],
            [nameof(SnakeSpeed)] = [],

            // GRID
            [nameof(Rows)] = [],
            [nameof(Columns)] = [],

            // GENERAL
            [nameof(MaxActiveFoods)] = [],
            [nameof(FoodSpawnFreq)] = [],

            // THEME
            [nameof(SnakeBodyColor)] = [],
            [nameof(SnakeHeadColor)] = [],
            [nameof(BackgroundColor)] = [],
            [nameof(FoodColor)] = []
        };

        #region ICommands
        // regular Commands
        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand DiscardChangesCommand { get; }
        public RelayCommand ResetToDefaultCommand { get; }
        // pop-up Commands
        public RelayCommand DiscardInPopUpCommand { get; }
        public RelayCommand SaveInPopUpCommand { get; }
        public RelayCommand CancelInPopUpCommand { get; }
        #endregion
        public void UpdateChangedState()
        {
            IsChanged = true;

            if (IsDraftDefault)
                IsDraftDefault = false;
        }
        private const string ForgotOtherParamMessage = "you didn't add string for 'other' arg";
        private static string GetErrorMessage(ValidationResult vr, string propName, string? other = null) 
        => vr switch
        {
            ValidationResult.ValueTooLow => $"{propName} cannot be lower than {other ?? throw new InvalidOperationException($"{ForgotOtherParamMessage} for {propName}")}.",
            ValidationResult.ValueTooHigh => $"{propName} cannot be higher than {other ?? throw new InvalidOperationException($"{ForgotOtherParamMessage} for {propName}")}.",
            ValidationResult.NullOrEmpty => $"{propName} cannot be empty or whitespace",
            
            _ => throw new InvalidOperationException("cannot get error message for valid result")
        };
        public event Action? CloseWinRequest;
        public event Action? ClosePopUpRequest;
    }
}