namespace TPUM.Server.Presentation.Events
{
    public delegate void EnableChangedEventHandler(object? source, bool lastEnable, bool newEnable);

    public interface INotifyEnableChanged
    {
        event EnableChangedEventHandler? EnableChanged;
    }
}
