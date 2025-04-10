namespace TPUM.Client.Data.Events
{
    public delegate void HeaterRemovedEventHandler(object? source, Guid roomId, Guid id);

    public interface INotifyHeaterRemoved
    {
        event HeaterRemovedEventHandler? HeaterRemoved;
    }
}
