namespace TPUM.Client.Logic.Events
{
    public delegate void RoomAddedEventHandler(object? source, IRoomLogic room);

    public interface INotifyRoomAdded
    {
        event RoomAddedEventHandler? RoomAdded;
    }
}