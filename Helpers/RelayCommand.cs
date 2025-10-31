using System.Windows.Input;
using SnakeGame.Model;

namespace SnakeGame.Helpers
{
    public class RelayCommand(Func<Task> execute, Func<bool> canExecute) : ICommand
    {
        private readonly Func<Task> execute = execute;
        private readonly Func<bool> canExecute = canExecute;
        public async void Execute(object? parameter = null)
            => await execute();

        public bool CanExecute(object? parameter = null)
            => canExecute();

        public event EventHandler? CanExecuteChanged;
    }
}