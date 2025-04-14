namespace TPUM.Client.Data.Events
{
    public delegate void PositionChangedEventHandler(object? source, IPositionData lastPosition, IPositionData newPosition);

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}