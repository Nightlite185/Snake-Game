using SnakeGame.Model;

namespace SnakeGame.ViewModel
{
    public class SettingsViewModel
    {
        public SettingsViewModel(Settings settingsCopy)
        {
            this.settingsCopy = settingsCopy;
            #region ICommands
            //SaveChangesCommand = new RelayCommand(
            //    execute:
            //    canExecute:
            //);

            //DiscardChangesCommand = new RelayCommand(
            //    execute: 
            //    canExecute:
            //)

            //ResetToDefaultCommand = new RelayCommand(
            //    execute: 
            //    canExecute:
            //)
            #endregion
        }
        private readonly Settings settingsCopy;
        
        #region ICommands
        //public ICommand SaveChangesCommand { get; }
        //public ICommand DiscardChangesCommand { get; }
        //public ICommand ResetToDefaultCommand { get; }
        #endregion
    }
}