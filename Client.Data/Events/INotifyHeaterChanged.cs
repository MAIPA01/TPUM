namespace TPUM.Client.Data.Events
{
    public delegate void HeaterChangedEventHandler(object? source, HeaterChangedEventArgs e);
    public class HeaterChangedEventArgs : EventArgs
    {
        public Guid Id { get; }
        public Guid RoomId { get; }
        public bool Updated { get; }
        public bool Removed { get; }

        public HeaterChangedEventArgs(Guid id, Guid roomId, bool updated, bool removed)
        {
            Id = id;
            RoomId = roomId;
            Updated = updated;
            Removed = removed;
        }
    }

    public interface INotifyHeaterChanged
    {
        event HeaterChangedEventHandler? HeatSensorChanged;
    }
}