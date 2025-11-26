using SnakeGame.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using SnakeGame.Model;

namespace SnakeGame.View
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly GameViewModel GameVM;
        private Settings.ThemeSettings theme = null!;
        private Rectangle[,]? rectPool;
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private void PrepareCanvas(Settings.ThemeSettings theme, Coords bounds)
        {
            this.theme = theme;

            if (
                rectPool != null
                && rectPool.GetLength(0) == bounds.Row 
                && rectPool.GetLength(1) == bounds.Col
            ) 
            { 
                ClearVisuals(); // clear visuals call here to set the background color to the correct one
                return;  // yes ik its lazy but it would make the guard clause more complicated if I wanted to set it below. 
            }            // why fuck up this beautiful logic ;(

            GameCanvas.Children.Clear();

            rectPool = new Rectangle[bounds.Row, bounds.Col];
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
                        Fill = new SolidColorBrush(theme.BackgroundColor ?? Settings.ThemeSettings.DefBackgroundColor)
                    };

                    Canvas.SetTop(rect, row * tileHeight);
                    Canvas.SetLeft(rect, col * tileWidth);

                    rectPool[row, col] = rect;
                    GameCanvas.Children.Add(rect);
                }
            }
        }
        public void Window_KeyDown(object sender, KeyEventArgs e) => GameVM.KeyDownHandler(e);
        private void ClearVisuals()
        {
            foreach (var rect in rectPool!)
                rect.Fill = new SolidColorBrush(theme.BackgroundColor ?? Settings.ThemeSettings.DefBackgroundColor);
        }
        private void RenderFrame()
        {
            ClearVisuals();

            foreach (var (coords, color) in GameVM.GetRenderable())
                rectPool![coords.Row, coords.Col].Fill = color;
        }
        
        public MainWindow()
        {
            InitializeComponent();

            GameVM = new();
            DataContext = GameVM;
            InputTip.DataContext = this;
            Scoreboard.DataContext = GameVM.sb;
            
            rectPool = null;

            GameVM.OnGameStarting += PrepareCanvas;
            GameVM.OnRenderRequest += RenderFrame;
            GameVM.OnRestartRequest += ClearVisuals;

            
            Closing += (_, e) => GameVM.CleanupOnExit();
        }

        private void NicknameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NicknameInput.Text.Length > 9)
            {
                NicknameInput.ToolTip = "Max length (9 characters) exceeded.";
                return;
            }
            
            GameVM.NameEntered = NicknameInput.Text;

            if (string.IsNullOrWhiteSpace(NicknameInput.Text))
                InputTip_Visibility = Visibility.Visible;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) 
            => Keyboard.ClearFocus(); // stealing focus from the textbox bc its clingy af and won't let me unfocus.
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