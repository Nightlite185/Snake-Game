using System.Windows.Input;

namespace SnakeGame.Commands
{
    public class RelayCommand(Func<Task> func) : ICommand
    {
        private readonly Func<Task> execute = func;

        public async void Execute(object? parameter = null)
            => await execute();

        public bool CanExecute(object? parameter = null)
            => true;

        public event EventHandler? CanExecuteChanged;
    }
}