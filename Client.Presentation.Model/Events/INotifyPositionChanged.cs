namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void PositionChangedEventHandler(object? source, PositionChangedEventArgs e);
    public class PositionChangedEventArgs : EventArgs
    {
        public IPosition LastPosition { get; }
        public IPosition NewPosition { get; }

        public PositionChangedEventArgs(IPosition lastPosition, IPosition newPosition)
        {
            LastPosition = lastPosition;
            NewPosition = newPosition;
        }
    }

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}