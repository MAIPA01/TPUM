using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public interface IRoomLogic : INotifyHeaterAdded, INotifyHeaterRemoved, INotifyHeatSensorAdded, INotifyHeatSensorRemoved,
        INotifyPositionChanged, INotifyTemperatureChanged, INotifyEnableChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterLogic> Heaters { get; }
        IReadOnlyCollection<IHeatSensorLogic> HeatSensors { get; }

        void AddHeater(float x, float y, float temperature);
        bool ContainsHeater(Guid heaterId);
        IHeaterLogic? GetHeater(Guid heaterId);
        void RemoveHeater(Guid heaterId);
        void AddHeatSensor(float x, float y);
        bool ContainsHeatSensor(Guid sensorId);
        IHeatSensorLogic? GetHeatSensor(Guid sensorId);
        void RemoveHeatSensor(Guid sensorId);
    }
}