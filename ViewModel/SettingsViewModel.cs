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
                    this.OGSettingsRef = draftSettings;
                    IsChanged = false;
                    
                    if (IsDefault)
                        IsDefault = false;
                },
                canExecute: () => IsChanged
            );
            
            ResetToDefaultCommand = new RelayCommand(
                execute: () =>
                {
                    // TO DO:: maybe open some pop-up like "you sure u wanna reset???"
                    draftSettings.ToDefault();
                    this.OGSettingsRef = draftSettings;

                    IsChanged = false;
                    IsDefault = true;
                },
                canExecute: () => !IsDefault
            );

            DiscardChangesCommand = new RelayCommand(
                execute: () =>
                {
                    draftSettings = OGSettingsRef;
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