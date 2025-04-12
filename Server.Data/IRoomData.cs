using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    public interface IRoomData : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterData> Heaters { get; }
        IReadOnlyCollection<IHeatSensorData> HeatSensors { get; }

        IHeaterData AddHeater(float x, float y, float temperature);
        bool ContainsHeater(Guid heaterId);
        IHeaterData? GetHeater(Guid heaterId);
        void RemoveHeater(Guid heaterId);
        IHeatSensorData AddHeatSensor(float x, float y);
        bool ContainsHeatSensor(Guid sensorId);
        IHeatSensorData? GetHeatSensor(Guid sensorId);
        void RemoveHeatSensor(Guid sensorId);
    }
}
