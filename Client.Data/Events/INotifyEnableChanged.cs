namespace TPUM.Client.Data.Events
{
    public delegate void EnableChangedEventHandler(object? sender, bool lastEnable, bool newEnable);

    public interface INotifyEnableChanged
    {
        event EnableChangedEventHandler? EnableChanged;
    }
}