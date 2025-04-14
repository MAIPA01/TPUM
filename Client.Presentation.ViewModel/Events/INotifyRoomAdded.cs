namespace TPUM.Client.Presentation.ViewModel.Events
{
    public delegate void RoomAddedEventHandler(object? source, IRoom room);

    public interface INotifyRoomAdded
    {
        event RoomAddedEventHandler? RoomAdded;
    }
}