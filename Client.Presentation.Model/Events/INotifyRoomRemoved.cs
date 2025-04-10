namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void RoomRemovedEventHandler(object? source, Guid roomId);

    public interface INotifyRoomRemoved
    {
        event RoomRemovedEventHandler? RoomRemoved;
    }
}
