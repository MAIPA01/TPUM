namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void HeaterAddedEventHandler(object? source, IHeater heater);

    public interface INotifyHeaterAdded
    {
        event HeaterAddedEventHandler? HeaterAdded;
    }
}