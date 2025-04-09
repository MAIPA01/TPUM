namespace TPUM.Client.Logic.Events
{
    public delegate void HeatSensorChangedEventHandler(object? source, HeatSensorChangedEventArgs e);
    public class HeatSensorChangedEventArgs : EventArgs
    {
        public Guid Id { get; }
        public Guid RoomId { get; }
        public bool Updated { get; }
        public bool Removed { get; }

        public HeatSensorChangedEventArgs(Guid id, Guid roomId, bool updated, bool removed)
        {
            Id = id;
            RoomId = roomId;
            Updated = updated;
            Removed = removed;
        }
    }

    public interface INotifyHeatSensorChanged
    {
        event HeatSensorChangedEventHandler? HeatSensorChanged;
    }
}