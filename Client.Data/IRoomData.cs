using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    public interface IRoomData : INotifyHeaterAdded, INotifyHeaterRemoved, INotifyHeatSensorRemoved, INotifyHeatSensorAdded,
        INotifyPositionChanged, INotifyTemperatureChanged, INotifyEnableChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterData> Heaters { get; }
        IReadOnlyCollection<IHeatSensorData> HeatSensors { get; }
        void AddHeater(float x, float y, float temperature);
        bool ContainsHeater(Guid heaterId);
        IHeaterData? GetHeater(Guid heaterId);
        void RemoveHeater(Guid heaterId);
        void AddHeatSensor(float x, float y);
        bool ContainsHeatSensor(Guid sensorId);
        IHeatSensorData? GetHeatSensor(Guid sensorId);
        void RemoveHeatSensor(Guid sensorId);
    }
}
