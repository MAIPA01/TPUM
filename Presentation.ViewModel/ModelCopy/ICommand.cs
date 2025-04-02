namespace TPUM.Presentation.ViewModel
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        void OnCanExecuteChanged();
    }
}