namespace TPUM.Server.Logic.Events
{
    public delegate void PositionChangedEventHandler(object? source, IPositionLogic lastPosition, IPositionLogic newPosition);

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}