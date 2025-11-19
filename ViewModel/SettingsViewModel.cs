using Force.DeepCloner;
using SnakeGame.Helpers;
using SnakeGame.Model;
using System.ComponentModel;

namespace SnakeGame.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool IsChanged
        { 
            get;
            set
            {
                field = value;

                SaveChangesCommand.ScreamCanExecuteChanged();
                DiscardChangesCommand.ScreamCanExecuteChanged();
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

        public Settings DraftSettings { get; }
        private readonly Settings OGSettingsRef;

        public int MaxSnakeStartLength => Math.Max(DraftSettings.Grid.Rows, DraftSettings.Grid.Columns) - 2;
        // REMEMBER TO CALL PROP_CHANGED ON THIS WHEN CHANGING ROWS OR COLS!!!!

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel(Settings settings)
        {
            DraftSettings = settings.DeepClone();
            this.OGSettingsRef = settings;

            #region ICommands
            SaveChangesCommand = new RelayCommand(
                execute: () =>
                {
                    DraftSettings.DeepCloneTo(OGSettingsRef);
                    IsChanged = false;
                    
                    if (IsDraftDefault)
                        isOGDefault = true;

                    else if(isOGDefault && !IsDraftDefault)
                        isOGDefault = false;

                },
                canExecute: () => IsChanged
            );
            
            ResetToDefaultCommand = new RelayCommand(
                execute: () =>
                {
                    // TO DO:: maybe open some pop-up like "you sure u wanna reset???"
                    DraftSettings.ToDefault();
                    
                    IsChanged = true;
                    IsDraftDefault = true;
                },
                canExecute: () => !IsDraftDefault
            );

            DiscardChangesCommand = new RelayCommand(
                execute: () =>
                {
                    OGSettingsRef.DeepCloneTo(DraftSettings);
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
    }
}