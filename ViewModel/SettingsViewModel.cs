using SnakeGame.Helpers;
using SnakeGame.Model;

namespace SnakeGame.ViewModel
{
    public class SettingsViewModel
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
        private bool IsDefault
        { 
            get; 
            set
            {
                field = value;
                ResetToDefaultCommand.ScreamCanExecuteChanged();    
            } 
        }
        private Settings draftSettings;
        private Settings OGSettingsRef;
        public SettingsViewModel(Settings settings)
        {
            this.draftSettings = settings.DeepClone();
            this.OGSettingsRef = settings;

            #region ICommands
            SaveChangesCommand = new RelayCommand(
                execute: () =>
                {
                    OGSettingsRef.ImportFrom(draftSettings);
                    IsChanged = false;
                    
                    if (IsDefault)
                        IsDefault = false; // FIX THIS AAAAHJUFIOLWEHUIG
                },
                canExecute: () => IsChanged
            );
            
            ResetToDefaultCommand = new RelayCommand(
                execute: () =>
                {
                    // TO DO:: maybe open some pop-up like "you sure u wanna reset???"
                    draftSettings.ToDefault();
                    
                    IsChanged = true;
                    IsDefault = true; // FIX THIS, ITS NOT CONSISTENT WITH SAVE COMMAND
                },
                canExecute: () => !IsDefault
            );

            DiscardChangesCommand = new RelayCommand(
                execute: () =>
                {
                    draftSettings.ImportFrom(OGSettingsRef);
                    IsChanged = false;
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