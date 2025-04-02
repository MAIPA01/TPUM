namespace TPUM.Presentation.Model
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        void OnCanExecuteChanged();
    }
}