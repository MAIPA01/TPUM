namespace TPUM.Client.Data.Events
{
    public delegate void HeatSensorAddedEventHandler(object? source, Guid roomId, IHeatSensorData sensor);

    public interface INotifyHeatSensorAdded
    {
        event HeatSensorAddedEventHandler? HeatSensorAdded;
    }
}
