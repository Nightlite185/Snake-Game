using System.ComponentModel;

namespace SnakeGame.ViewModel
{
    public class GameViewModel() : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}