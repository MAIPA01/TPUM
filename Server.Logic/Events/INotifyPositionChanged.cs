namespace TPUM.Client.Logic.Events
{
    public delegate void PositionChangedEventHandler(object? source, PositionChangedEventArgs e);
    public class PositionChangedEventArgs : EventArgs
    {
        public IPositionLogic LastPosition { get; }
        public IPositionLogic NewPosition { get; }

        public PositionChangedEventArgs(IPositionLogic lastPosition, IPositionLogic newPosition)
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