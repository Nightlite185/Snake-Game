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
        private bool liveBinding;
        private Settings OGSettings { get; set; }
        #endregion

        #region UI SETTINGS BINDING PROPERTIES

        // SNAKE 
        public int MaxSnakeStartLength => Math.Max(Rows, Columns) - 2;
        public int SnakeSpeed
        {
            get;
            set
            {
                string propName = nameof(SnakeSpeed);
                var bounds = (Settings.SnakeSettings.MinSpeed, Settings.SnakeSettings.MaxSpeed);

                TryNotify(ref field, value, this, propName);
                
                if (liveBinding)
                {
                    var result = Validation.Int(value, bounds);

                    UpdateChangedState();
                    ValidationHelper(propName, result, bounds);
                }
            }
        }
        public int StartingLength
        {
            get;
            set
            {
                string propName = nameof(StartingLength);
                var bounds = (Settings.SnakeSettings.MinStartLength, MaxSnakeStartLength);

                TryNotify(ref field, value, this, propName);
                
                if (liveBinding)
                {
                    var result = Validation.Int(value, bounds);

                    UpdateChangedState();
                    ValidationHelper(propName, result, bounds);
                }
            }
        }

        // GRID
        public int Rows
        {
            get;
            set
            {
                string propName = nameof(Rows);
                var bounds = (Settings.GridSettings.MinGridAny, Settings.GridSettings.MaxGridAny);
                string[] toNotify = [nameof(Rows), nameof(MaxChoosableMaxActiveFoods), nameof(MaxSnakeStartLength)];

                TryNotify(ref field, value, this, toNotify);
                
                if (liveBinding) 
                {
                    var result = Validation.Int(value, bounds);

                    UpdateChangedState();
                    ValidationHelper(propName, result, bounds);
                }
            }
        }
        public int Columns
        {
            get;
            set
            {
                string propName = nameof(Columns);
                var bounds = (Settings.GridSettings.MinGridAny, Settings.GridSettings.MaxGridAny);
                string[] toNotify = [nameof(Columns), nameof(MaxChoosableMaxActiveFoods), nameof(MaxSnakeStartLength)];

                TryNotify(ref field, value, this, toNotify);

                if (liveBinding) 
                {
                    var result = Validation.Int(value, bounds);

                    UpdateChangedState();
                    ValidationHelper(propName, result, bounds);
                }
            }
        }

        // FOOD
        public int MaxActiveFoods
        {
            get;
            set
            {
                string propName = nameof(MaxActiveFoods);
                var bounds = (Settings.FoodSettings.MinActiveFoodsLimit, MaxChoosableMaxActiveFoods);

                TryNotify(ref field, value, this, propName);

                if (liveBinding) 
                {
                    var result = Validation.Int(value, bounds);

                    UpdateChangedState();
                    ValidationHelper(propName, result, bounds);
                }
            }
        }
        public int MaxChoosableMaxActiveFoods => Rows * Columns;
        public int FoodSpawnFreq
        {
            get;
            set
            {
                string propName = nameof(FoodSpawnFreq);
                var bounds = (Settings.FoodSettings.MinFoodFreq, Settings.FoodSettings.MaxFoodFreq);

                TryNotify(ref field, value, this, propName);
                
                if (liveBinding) 
                {
                    var result = Validation.Int(value, bounds);
                    
                    UpdateChangedState();
                    ValidationHelper(propName, result, bounds);
                }
            }
        }

        // THEME
        public Color? SnakeHeadColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(SnakeHeadColor));
                if (liveBinding) UpdateChangedState();
            }
        } 
        public Color? SnakeBodyColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(SnakeBodyColor));
                if (liveBinding) UpdateChangedState();
            }
        }
        public Color? BackgroundColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(BackgroundColor));
                if (liveBinding) UpdateChangedState();
            }
        }
        public Color? FoodColor
        {
            get;
            set
            {
                TryNotify(ref field, value, this, nameof(FoodColor));
                if (liveBinding) UpdateChangedState();
            }
        }
        #endregion

        #region LOAD / SAVE METHODS
        private void LoadFromOG()
        {
            liveBinding = false;

            // SNAKE
            this.StartingLength = OGSettings.Snake.StartingLength;
            this.SnakeSpeed = OGSettings.Snake.Speed;

            // GRID
            this.Rows = OGSettings.Grid.Rows;
            this.Columns = OGSettings.Grid.Columns;

            // GENERAL
            this.MaxActiveFoods = OGSettings.Food.MaxActiveFoods;
            this.FoodSpawnFreq = OGSettings.Food.FoodSpawnFreq;

            // THEME
            this.SnakeHeadColor = OGSettings.Theme.SnakeHeadColor;
            this.SnakeBodyColor = OGSettings.Theme.SnakeBodyColor;
            this.BackgroundColor = OGSettings.Theme.BackgroundColor;
            this.FoodColor = OGSettings.Theme.FoodColor;

            liveBinding = true;
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
            liveBinding = false;

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

            liveBinding = true;
        }
        private void UpdateChangedState()
        {
            IsChanged = true;

            if (IsDraftDefault)
                IsDraftDefault = false;
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
        
        private const string ForgotOtherParamMessage = "you didn't provide string for 'other' arg";
        private static string GetErrorMessage(ValidationResult vr, string propName, (double? min, double? max) bounds, string? reason)
        => vr switch
        {
            ValidationResult.ValueTooLow => $"{propName} cannot be lower than {bounds.min ?? throw new InvalidOperationException($"{ForgotOtherParamMessage} for {propName}")}{(", because " + reason) ?? ""}.",
            ValidationResult.ValueTooHigh => $"{propName} cannot be higher than {bounds.max ?? throw new InvalidOperationException($"{ForgotOtherParamMessage} for {propName}")}{(", because " + reason) ?? ""}.",
            ValidationResult.NullOrEmpty => $"{propName} cannot be empty or whitespace",
            
            _ => throw new InvalidOperationException("cannot get error message for valid result")
        };
        private void ValidationHelper(string propName, ValidationResult result, (double? min, double? max) bounds, string? reason = null)
        {
            ClearErrors(propName);
            SaveChangesCommand.ScreamCanExecuteChanged();

            if (result != ValidationResult.Valid)
            {
                AddError(propName, GetErrorMessage(result, propName, bounds, reason));
                SaveChangesCommand.ScreamCanExecuteChanged();
            }   
        }
        public event Action? CloseWinRequest;
        public event Action? ClosePopUpRequest;
    }
}