namespace TPUM.Client.Data.Events
{
    public delegate void HeaterAddedEventHandler(object? source, IHeaterData heater);

    public interface INotifyHeaterAdded
    {
        event HeaterAddedEventHandler? HeaterAdded;
    }
}
