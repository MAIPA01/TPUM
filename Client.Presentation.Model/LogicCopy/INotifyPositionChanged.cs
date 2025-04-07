namespace TPUM.Client.Presentation.Model
{
    public delegate void PositionChangedEventHandler(object? source, PositionChangedEventArgs e);
    public class PositionChangedEventArgs(IPosition lastPosition, IPosition newPosition) : EventArgs
    {
        public IPosition LastPosition { get; private set; } = lastPosition;
        public IPosition NewPosition { get; private set; } = newPosition;
    }

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}