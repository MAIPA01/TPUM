namespace TPUM.Client.Data.Events
{
    public delegate void TemperatureChangedEventHandler(object? source, float lastTemperature, float newTemperature);
    public interface INotifyTemperatureChanged
    {
        event TemperatureChangedEventHandler? TemperatureChanged;
    }
}
