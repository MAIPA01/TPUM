namespace TPUM.Client.Logic.Events
{
    public delegate void HeatSensorRemovedEventHandler(object? source, Guid sensorId);

    public interface INotifyHeatSensorRemoved
    {
        event HeatSensorRemovedEventHandler? HeatSensorRemoved;
    }
}