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
            this.DataContext = SetVM;
            
            SetVM = new SettingsViewModel(OGSettingsRef);

            InitializeComponent();
        }
    }
}