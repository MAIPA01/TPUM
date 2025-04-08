namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void EnableChangedEventHandler(object? source, EnableChangedEventArgs e);
    public class EnableChangedEventArgs : EventArgs
    {
        public bool LastEnable { get; }
        public bool NewEnable { get; }

        public EnableChangedEventArgs(bool lastEnable, bool newEnable)
        {
            LastEnable = lastEnable;
            NewEnable = newEnable;
        }
    }

    public interface INotifyEnableChanged
    {
        event EnableChangedEventHandler? EnableChanged;
    }
}