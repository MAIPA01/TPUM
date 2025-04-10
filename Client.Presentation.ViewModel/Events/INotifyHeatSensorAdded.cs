namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void HeatSensorAddedEventHandler(object? source, IHeatSensor sensor);
    public interface INotifyHeatSensorAdded
    {
        event HeatSensorAddedEventHandler? HeatSensorAdded;
    }
}
