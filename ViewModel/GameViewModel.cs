using System.ComponentModel;
using System.Threading.Tasks.Dataflow;
using System.Windows.Input;
using System.Windows.Media;
using SnakeGame.Helpers;
using SnakeGame.Model;
using SnakeGameProject;

namespace SnakeGame.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly GameManager gameManager;
        private readonly MainWindow view;
        public static Coords Dimensions => new(GameManager.gridRows, GameManager.gridColumns);
        public IEnumerable<(Coords coords, SolidColorBrush color)> Renderable()
        => gameManager.Snake.Body
            .Select(s => (s.Coords, Brushes.LightGreen))
            .Concat(
                gameManager.FoodPool.ActiveFoods
                .Select(f => (f.Coords, Brushes.Red))
            );

        #region ICommands
        public ICommand StartGameCommand { get; }
        #endregion
        
        public void KeyInputHandler(string key)
        {
            gameManager.QueuedDirection = key switch
            {
                "W" or "UpArrow" => Direction.Up,
                "S" or "DownArrow" => Direction.Down,
                "A" or "LeftArrow" => Direction.Left,
                "D" or "RightArrow" => Direction.Right,

                _ => gameManager.QueuedDirection
            };
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public GameViewModel(MainWindow view)
        {
            gameManager = new();
            this.view = view;
            StartGameCommand = new RelayCommand(gameManager.RunGameAsync);
        }
    }
}