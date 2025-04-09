namespace TPUM.Server.Presentation.Events
{
    public delegate void PositionChangedEventHandler(Guid roomId, object? source, IPositionPresentation lastPosition, IPositionPresentation newPosition);

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}
