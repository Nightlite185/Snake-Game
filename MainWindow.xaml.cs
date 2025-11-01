using SnakeGame.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using SnakeGame.Model;
using System.Windows.Input;

namespace SnakeGameProject
{
    public partial class MainWindow : Window
    {
        private readonly GameViewModel viewModel;
        private readonly Rectangle[,] rectPool;
        private readonly Coords bounds = GameViewModel.Dimensions;
        private void InitializeRectPool()
        {
            double tileHeight = GameCanvas.ActualHeight / bounds.Row;
            double tileWidth = GameCanvas.ActualWidth / bounds.Col;

            for (int row = 0; row < bounds.Row; row++)
            {
                for (int col = 0; col < bounds.Col; col++)
                {
                    Rectangle rect = new()
                    {
                        Width = tileWidth,
                        Height = tileHeight,
                        Fill = Brushes.Transparent,
                    };

                    Canvas.SetTop(rect, row * tileHeight);
                    Canvas.SetLeft(rect, col * tileWidth);

                    rectPool[row, col] = rect;
                    GameCanvas.Children.Add(rect);
                }
            }
        }
        public void Window_KeyDown(object sender, KeyEventArgs e) => viewModel.KeyDownHandler(e);
        
        public void RenderGameObjects()
        {
            foreach (var rect in rectPool) // could use hashset and only go through it once, instead of clearing everything and rendering again.
                rect.Fill = Brushes.Transparent; // this is simple but works. Idk which would be faster tho. Hashset also takes time to build from 0 every time so..

            foreach (var (coords, color) in viewModel.Renderable)
                rectPool[coords.Row, coords.Col].Fill = color;
        }
        
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new(this);
            DataContext = viewModel;

            viewModel.OnRenderRequest += RenderGameObjects;

            rectPool = new Rectangle[bounds.Row, bounds.Col];
            
            Loaded += (_, _) => InitializeRectPool();
        }
    }
}