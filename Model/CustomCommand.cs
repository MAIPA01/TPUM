
using System.Windows.Input;

namespace Model
{
    public class CustomCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action<object?> _Execute { get; set; }

        private Predicate<object?> _CanExecute { get; set; }

        public CustomCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _Execute(parameter);
        }
    }

}
