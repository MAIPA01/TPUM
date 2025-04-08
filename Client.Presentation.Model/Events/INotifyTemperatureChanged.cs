namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void TemperatureChangedEventHandler(object? source, TemperatureChangedEventArgs e);

    public class TemperatureChangedEventArgs : EventArgs
    {
        public float LastTemperature { get; }
        public float NewTemperature { get; }

        public TemperatureChangedEventArgs(float lastTemperature, float newTemperature)
        {
            LastTemperature = lastTemperature;
            NewTemperature = newTemperature;
        }
    }

    public interface INotifyTemperatureChanged
    {
        event TemperatureChangedEventHandler? TemperatureChanged;
    }
}