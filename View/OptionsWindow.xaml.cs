using SnakeGame.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SnakeGame.View
{
    public partial class OptionsWindow : Window
    {
        private readonly SettingsViewModel SetVM;
        public OptionsWindow(SettingsViewModel setVM)
        {
            SetVM = setVM;
            this.DataContext = SetVM;
            
            InitializeComponent();
            SetVM.CloseWinRequest += Close;
            SetVM.ClosePopUpRequest += () => PopUpGrid.Visibility = Visibility.Collapsed;
        }

        private void OptionsGrid_MouseDown(object sender, MouseButtonEventArgs e)
            => Keyboard.ClearFocus(); // steal focus from the slider and textboxes

        protected override void OnClosing(CancelEventArgs e)
        {
            if (SetVM.IsChanged)
            {
                e.Cancel = true;
                PopUpGrid.Visibility = Visibility.Visible;
            }

            base.OnClosing(e);
        }
    }
}