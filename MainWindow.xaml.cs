using SnakeGame.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace SnakeGameProject
{
    public partial class MainWindow : Window
    {
        private readonly GameViewModel viewModel;
        private void DrawTestRect()
        {
            Rectangle rect = new()
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);

            GameCanvas.Children.Add(rect);
        }
        public MainWindow()
        {
            this.viewModel = new();
            InitializeComponent();

            DataContext = viewModel;

            DrawTestRect();
        }
    }
}