namespace TPUM.Client.Logic.Events
{
    public delegate void RoomChangedEventHandler(object? source, RoomChangedEventArgs e);
    public class RoomChangedEventArgs : EventArgs
    {
        public Guid RoomId { get; }
        public bool Updated { get; }
        public bool Removed { get; }

        public RoomChangedEventArgs(Guid id, bool updated, bool removed)
        {
            RoomId = id;
            Updated = updated;
            Removed = removed;
        }
    }

    public interface INotifyRoomChanged
    {
        event RoomChangedEventHandler? RoomChanged;
    }
}