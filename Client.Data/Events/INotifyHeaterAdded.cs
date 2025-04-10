namespace TPUM.Client.Data.Events
{
    public delegate void HeaterAddedEventHandler(object? source, Guid roomId, IHeaterData heater);

    public interface INotifyHeaterAdded
    {
        event HeaterAddedEventHandler? HeaterAdded;
    }
}
