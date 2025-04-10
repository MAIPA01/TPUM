namespace TPUM.Client.Logic.Events
{
    public delegate void HeaterRemovedEventHandler(object? source, Guid heaterId);

    public interface INotifyHeaterRemoved
    {
        event HeaterRemovedEventHandler? HeaterRemoved;
    }
}
