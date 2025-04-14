namespace TPUM.Server.Presentation.Events
{
    public delegate void PositionChangedEventHandler(Guid roomId, object? source, IPosition lastPosition, IPosition newPosition);

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}