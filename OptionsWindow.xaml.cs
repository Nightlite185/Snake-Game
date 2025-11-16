using SnakeGame.Model;
using SnakeGame.ViewModel;
using System.Windows;

namespace SnakeGame
{
    public partial class OptionsWindow : Window
    {
        private readonly SettingsViewModel SetVM;
        private readonly Settings settingsCopy;
        public OptionsWindow(Settings settingsCopy)
        {
            this.DataContext = SetVM;
            this.settingsCopy = settingsCopy;
            
            SetVM = new SettingsViewModel(settingsCopy);

            InitializeComponent();
        }
    }
}