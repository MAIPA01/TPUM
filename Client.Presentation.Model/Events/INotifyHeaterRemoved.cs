namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void HeaterRemovedEventHandler(object? source, Guid heaterId);

    public interface INotifyHeaterRemoved
    {
        event HeaterRemovedEventHandler? HeaterRemoved;
    }
}