namespace TPUM.Client.Data.Events
{
    public delegate void HeatSensorRemovedEventHandler(object? source, Guid roomId, Guid id);

    public interface INotifyHeatSensorRemoved
    {
        event HeatSensorRemovedEventHandler? HeatSensorRemoved;
    }
}
