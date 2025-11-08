using SnakeGame.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using SnakeGame.Model;
using System.Windows.Input;
using SnakeGame;
using System.ComponentModel;

namespace SnakeGameProject
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly GameViewModel viewModel;
        private readonly Rectangle[,] rectPool;
        private readonly Coords bounds = GameViewModel.Dimensions;

        public event PropertyChangedEventHandler? PropertyChanged;

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
            foreach (var (coords, color) in viewModel.GetRenderable())
                rectPool[coords.Row, coords.Col].Fill = color;
        }
        
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new();

            DataContext = viewModel; // setting "global" datacontext
            InputTip.DataContext = this;

            viewModel.OnRenderRequest += RenderGameObjects;
            viewModel.OnRestartRequest += ClearVisuals;

            rectPool = new Rectangle[bounds.Row, bounds.Col];

            Loaded += (_, _) => InitializeRectPool(); // initialization after loading UI element bc it needs to know actual sizes.
            
            Closing += (_, e) => viewModel.SaveOnExit(e);
            Loaded += (_, _) => viewModel.LoadOnInit();            
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var optionsWin = new OptionsWindow(viewModel);
            optionsWin.ShowDialog();
        }

        private void NicknameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NicknameInput.Text.Length > 9)
                return; // later raise tooltip saying you exceeded max length.
            
            viewModel.NameEntered = NicknameInput.Text;

            if (string.IsNullOrWhiteSpace(NicknameInput.Text))
                InputTip_Visibility = Visibility.Visible;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) 
            => Keyboard.ClearFocus(); // stealing focus from the textbox bc its clingy as fuck and won't let me unfocus.
        public Visibility InputTip_Visibility
        {
            get;

            private set
            {
                if (field != value)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputTip_Visibility)));
                }
            }
        }

        private void NicknameInput_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) 
            => InputTip_Visibility = Visibility.Collapsed;
    }
}