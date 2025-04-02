namespace TPUM.Data
{
    public delegate void EnableChangedEventHandler(object? source, EnableChangedEventArgs e);
    public class EnableChangedEventArgs(bool lastEnable, bool newEnable) : EventArgs
    {
        public bool LastEnable { get; private set; } = lastEnable;
        public bool NewEnable { get; private set; } = newEnable;
    }

    public interface INotifyEnableChanged
    {
        event EnableChangedEventHandler? EnableChanged;
    }
}