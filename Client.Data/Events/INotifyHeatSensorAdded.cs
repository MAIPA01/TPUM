namespace TPUM.Client.Data.Events
{
    public delegate void HeatSensorAddedEventHandler(object? source, IHeatSensorData sensor);

    public interface INotifyHeatSensorAdded
    {
        event HeatSensorAddedEventHandler? HeatSensorAdded;
    }
}