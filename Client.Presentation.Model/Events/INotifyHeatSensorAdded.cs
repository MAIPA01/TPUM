namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void HeatSensorAddedEventHandler(object? source, IHeatSensorModel sensor);
    public interface INotifyHeatSensorAdded
    {
        event HeatSensorAddedEventHandler? HeatSensorAdded;
    }
}
