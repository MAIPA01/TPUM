namespace TPUM.Presentation.ViewModel
{
    internal class CustomCommand : ICommand
    {
        private readonly Model.ICommand _command;

        public event EventHandler? CanExecuteChanged;

        public CustomCommand(Model.ICommand command)
        {
            _command = command;
            _command.CanExecuteChanged += GetCanExecuteChanged;
        }

        private void GetCanExecuteChanged(object? sender, EventArgs args)
        {
            CanExecuteChanged?.Invoke(this, args);
        }

        public bool CanExecute(object? parameter)
        {
            return _command.CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _command.Execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            _command.OnCanExecuteChanged();
        }
    }
}