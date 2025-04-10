namespace TPUM.Client.Logic.Events
{
    public delegate void HeatSensorAddedEventHandler(object? source, IHeatSensorLogic sensor);

    public interface INotifyHeatSensorAdded
    {
        event HeatSensorAddedEventHandler? HeatSensorAdded;
    }
}