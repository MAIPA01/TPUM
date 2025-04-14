namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void HeaterRemovedEventHandler(object? source, Guid heaterId);

    public interface INotifyHeaterRemoved
    {
        event HeaterRemovedEventHandler? HeaterRemoved;
    }
}