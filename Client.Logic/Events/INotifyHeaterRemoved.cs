namespace TPUM.Client.Logic.Events
{
    public delegate void HeaterRemovedEventHandler(object? source, Guid roomId, Guid heaterId);

    public interface INotifyHeaterRemoved
    {
        event HeaterRemovedEventHandler? HeaterRemoved;
    }
}
