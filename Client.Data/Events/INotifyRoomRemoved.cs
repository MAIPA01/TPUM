namespace TPUM.Client.Data.Events
{
    public delegate void RoomRemovedEventHandler(object? source, Guid id);

    public interface INotifyRoomRemoved
    {
        event RoomRemovedEventHandler? RoomRemoved;
    }
}
