namespace TPUM.Server.Presentation.Events
{
    public delegate void PositionChangedEventHandler(object? source, IPositionPresentation lastPosition, IPositionPresentation newPosition);

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}
