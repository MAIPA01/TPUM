namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void PositionChangedEventHandler(object? source, IPositionModel lastPosition, IPositionModel newPosition);

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}