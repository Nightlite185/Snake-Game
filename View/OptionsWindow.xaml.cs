using SnakeGame.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

            HookEventsToThumb(SnakeLengthSlider);
            HookEventsToThumb(TickSpeedSlider);
        }

        private void HookEventsToThumb(Slider s)
        {
            s.ApplyTemplate();

            var thumb = ((s.Template.FindName("PART_Track", s) as Track)?.Thumb)
                ?? throw new Exception("thumb not found");

            thumb.DragCompleted += (_, _) => SetVM.UpdateChangedState();
        }

        private void OptionsGrid_MouseDown(object sender, MouseButtonEventArgs e)
            => Keyboard.ClearFocus(); // steal focus from the slider and textboxes
    }
}