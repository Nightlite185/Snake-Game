using SnakeGame.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using SnakeGame.Model;

namespace SnakeGameProject
{
    public partial class MainWindow : Window
    {
        private readonly GameViewModel viewModel;
        private readonly Coords bounds;
        private readonly double tileHeight;
        private readonly double tileWidth;
        private void InitializeRectPool()
        {
            for (double row = 0; row < bounds.Row; row += tileHeight)
            {
                for (double col = 0; col < bounds.Col; col += tileWidth)
                {
                    Rectangle rect = new()
                    {
                        Width = tileWidth,
                        Height = tileHeight,
                        Fill = Brushes.Transparent,
                    };

                    Canvas.SetTop(rect, row);
                    Canvas.SetLeft(rect, col);
                }
            }
        }
        private readonly Rectangle[,] rectPool;
        public void RenderGameObjects()
        {
            foreach (var obj in viewModel.Renderable())
            {
                var rect = rectPool[obj.coords.Row, obj.coords.Col];

                rect.Fill = obj.color;
                GameCanvas.Children.Add(rect);
            }
        }
        public void ClearFrame() => GameCanvas.Children.Clear();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel = new(this);

            bounds = GameViewModel.Dimensions;
            tileHeight = GameCanvas.ActualHeight / bounds.Row;
            tileWidth = GameCanvas.ActualWidth / bounds.Col;

            rectPool = new Rectangle[bounds.Row, bounds.Col];

            InitializeRectPool();
        }
    }
}