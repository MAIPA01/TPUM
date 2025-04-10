namespace TPUM.Client.Logic.Events
{
    public delegate void HeatSensorAddedEventHandler(object? source, Guid roomId, IHeatSensorLogic sensor);

    public interface INotifyHeatSensorAdded
    {
        event HeatSensorAddedEventHandler? HeatSensorAdded;
    }
}