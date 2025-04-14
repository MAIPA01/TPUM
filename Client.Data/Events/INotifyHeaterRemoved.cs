namespace TPUM.Client.Data.Events
{
    public delegate void HeaterRemovedEventHandler(object? source, Guid heaterId);

    public interface INotifyHeaterRemoved
    {
        event HeaterRemovedEventHandler? HeaterRemoved;
    }
}