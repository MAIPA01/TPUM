namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void RoomAddedEventHandler(object? source, IRoomModel room); 
    public interface INotifyRoomAdded
    {
        event RoomAddedEventHandler? RoomAdded;
    }
}
