namespace TPUM.Client.Presentation.ViewModel
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        void OnCanExecuteChanged();
    }
}