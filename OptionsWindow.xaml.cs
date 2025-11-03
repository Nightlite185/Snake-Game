using SnakeGame.ViewModel;
using System.Windows;

namespace SnakeGame
{
    public partial class OptionsWindow : Window
    {
        private readonly GameViewModel viewModel;
        public OptionsWindow(GameViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}