namespace TPUM.Client.Logic.Events
{
    public delegate void HeaterAddedEventHandler(object? source, IHeaterLogic heater);

    public interface INotifyHeaterAdded
    {
        event HeaterAddedEventHandler? HeaterAdded;
    }
}