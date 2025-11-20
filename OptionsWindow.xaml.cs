using SnakeGame.Model;
using SnakeGame.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

            SnakeLengthSlider.ManipulationCompleted += (_, _) => SetVM.IsChanged = true;
            SnakeLengthSlider.DragOver += (_, _) => SetVM.IsChanged = true;

            HookEventsToThumb(SnakeLengthSlider);
        }
        private void HookEventsToThumb(Slider s)
        {
            s.ApplyTemplate();

            var thumb = ((s.Template.FindName("PART_Track", s) as Track)?.Thumb) 
                ?? throw new Exception("thumb not found");

            thumb.DragCompleted += (_, _) => SetVM.IsChanged = true;
        }
    }
}