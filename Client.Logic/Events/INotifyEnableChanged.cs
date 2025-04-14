namespace TPUM.Client.Logic.Events
{
    public delegate void EnableChangedEventHandler(object? source, bool lastEnable, bool newEnable);
    public interface INotifyEnableChanged
    {
        event EnableChangedEventHandler? EnableChanged;
    }
}