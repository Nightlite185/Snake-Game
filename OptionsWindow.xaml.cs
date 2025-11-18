using SnakeGame.Model;
using SnakeGame.ViewModel;
using System.Windows;

namespace SnakeGame
{
    public partial class OptionsWindow : Window
    {
        private readonly SettingsViewModel SetVM;
        public OptionsWindow(Settings OGSettingsRef)
        {
            SetVM = new SettingsViewModel(OGSettingsRef);
            
            this.DataContext = SetVM;

            InitializeComponent();
        }
    }
}