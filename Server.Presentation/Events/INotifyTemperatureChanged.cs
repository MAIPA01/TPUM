namespace TPUM.Server.Presentation.Events
{
    public delegate void TemperatureChangedEventHandler(Guid roomId, object? source, float lastTemperature, float newTemperature);

    public interface INotifyTemperatureChanged
    {
        event TemperatureChangedEventHandler? TemperatureChanged;
    }
}