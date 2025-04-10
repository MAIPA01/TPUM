namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void EnableChangeEventHandler(object? source, bool lastEnable, bool newEnable);
    public interface INotifyEnableChanged
    {
        event EnableChangeEventHandler? EnableChanged;
    }
}