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
        bool ContainsHeater(Guid id);
        IHeaterLogic? GetHeater(Guid id);
        void RemoveHeater(Guid id);
        void AddHeatSensor(float x, float y);
        bool ContainsHeatSensor(Guid id);
        IHeatSensorLogic? GetHeatSensor(Guid id);
        void RemoveHeatSensor(Guid id);
    }
}