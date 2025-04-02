namespace TPUM.Presentation.ViewModel
{
    public delegate void EnableChangeEventHandler(object? source, EnableChangeEventArgs e);
    public class EnableChangeEventArgs(bool lastEnable, bool newEnable) : EventArgs
    {
        public bool LastEnable { get; private set; } = lastEnable;
        public bool NewEnable { get; private set; } = newEnable;
    }

    public interface INotifyEnableChanged
    {
        event EnableChangeEventHandler? EnableChanged;
    }
}