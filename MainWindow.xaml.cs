using SnakeGame.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using SnakeGame.Model;
using System.Windows.Input;
using SnakeGame;

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
        private void ClearVisuals()
        {
            foreach (var rect in rectPool)
                rect.Fill = Brushes.Transparent;
        }
        private void RenderGameObjects()
        {
            ClearVisuals(); // could use hashset and only go through it once, instead of clearing everything and rendering again.
                             // This is simple but works. Idk which would be faster tho. Hashset also takes time to build from 0 every time so..
            foreach (var (coords, color) in viewModel.Renderable)
                rectPool[coords.Row, coords.Col].Fill = color;
        }
        
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new();
            DataContext = viewModel;

            viewModel.OnRenderRequest += RenderGameObjects;
            viewModel.OnRestartRequest += ClearVisuals;

            rectPool = new Rectangle[bounds.Row, bounds.Col];

            Loaded += (_, _) => InitializeRectPool(); // initialization after loading UI element bc it needs to know actual sizes.
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var optionsWin = new OptionsWindow(viewModel);
            optionsWin.ShowDialog();
        }
    }
}