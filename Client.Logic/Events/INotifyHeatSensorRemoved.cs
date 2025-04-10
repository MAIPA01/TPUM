namespace TPUM.Client.Logic.Events
{
    public delegate void HeatSensorRemovedEventHandler(object? source, Guid roomId, Guid sensorId);

    public interface INotifyHeatSensorRemoved
    {
        event HeatSensorRemovedEventHandler? HeatSensorRemoved;
    }
}
