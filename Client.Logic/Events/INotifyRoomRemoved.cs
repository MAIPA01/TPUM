namespace TPUM.Client.Logic.Events
{
    public delegate void RoomRemovedEventHandler(object? source, Guid roomId);

    public interface INotifyRoomRemoved
    {
        event RoomRemovedEventHandler? RoomRemoved;
    }
}
