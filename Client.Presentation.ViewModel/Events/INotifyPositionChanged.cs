namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void PositionChangedEventHandler(object? source, IPosition lastPosition, IPosition newPosition);
    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}