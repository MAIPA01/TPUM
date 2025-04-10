namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void HeaterAddedEventHandler(object? source, IHeaterModel heater);
    public interface INotifyHeaterAdded
    {
        event HeaterAddedEventHandler? HeaterAdded;
    }
}
