using SnakeGame.ViewModel;
using System.Windows;

namespace SnakeGameProject
{
    public partial class MainWindow : Window
    {
        private readonly GameViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new GameViewModel();
            //DrawInitialGrid();
        }
    }
}