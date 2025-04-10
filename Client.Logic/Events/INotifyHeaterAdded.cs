namespace TPUM.Client.Logic.Events
{
    public delegate void HeaterAddedEventHandler(object? source, Guid roomId, IHeaterLogic heater);

    public interface INotifyHeaterAdded
    {
        event HeaterAddedEventHandler? HeaterAdded;
    }
}