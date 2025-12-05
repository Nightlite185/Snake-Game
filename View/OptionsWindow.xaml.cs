using SnakeGame.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb.Text.Length != 0) return;
                
            OnEmptyTextbox?.Invoke(GetBoundProp(tb));
        }

        private static string GetBoundProp(TextBox tb)
        {
            var expr = tb.GetBindingExpression(TextBox.TextProperty);
            
            return expr?.ParentBinding?.Path?.Path 
                ?? throw new InvalidOperationException($"Could not get bound property name from {tb.Name}");
        }

        private void RejectIfNotDigit(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(char.IsDigit))
                e.Handled = true;
        }
        public event Action<string>? OnEmptyTextbox;
    }
}