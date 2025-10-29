using System.ComponentModel;
using System.Windows.Input;
using SnakeGame.Commands;
using SnakeGame.Model;

namespace SnakeGame.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    { 
        public GameViewModel()
        {
            this.gameManager = new();
            StartGameCommand = new RelayCommand(gameManager.RunGameAsync);
        }
        private const int GridRows = 20;
        private const int GridColumns = 20;
        private readonly GameManager gameManager;


        public ICommand StartGameCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}