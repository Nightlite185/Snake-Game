using System.Windows.Input;

namespace SnakeGame.Helpers
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
        }
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        private readonly Func<Task>? executeAsync;
        private readonly Action? execute;
        private readonly Func<bool> canExecute;
        public async void Execute(object? parameter = null)
        {
            if (executeAsync != null)
                await executeAsync();

            else
                execute!();
        }
        public bool CanExecute(object? parameter = null) => canExecute();

        public event EventHandler? CanExecuteChanged;
    }
}