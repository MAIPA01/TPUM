namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void EnableChangedEventHandler(object? source, bool lastEnable, bool newEnable);

    public interface INotifyEnableChanged
    {
        event EnableChangedEventHandler? EnableChanged;
    }
}