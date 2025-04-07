namespace TPUM.Client.Presentation.ViewModel
{
    internal class CustomCommand(Action<object?> execute, Predicate<object?> canExecute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public CustomCommand(Action<object?> execute) : this(execute, _ => true) {}

        public bool CanExecute(object? parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}