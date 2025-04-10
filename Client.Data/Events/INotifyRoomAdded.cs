namespace TPUM.Client.Data.Events
{
    public delegate void RoomAddedEventHandler(object? source, IRoomData room);

    public interface INotifyRoomAdded
    {
        event RoomAddedEventHandler? RoomAdded;
    }
}