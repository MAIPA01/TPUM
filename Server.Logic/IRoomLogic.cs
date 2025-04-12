using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    public interface IRoomLogic : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterLogic> Heaters { get; }
        IReadOnlyCollection<IHeatSensorLogic> HeatSensors { get; }

        IHeaterLogic AddHeater(float x, float y, float temperature);

        bool ContainsHeater(Guid heaterId);

        IHeaterLogic? GetHeater(Guid heaterId);

        void RemoveHeater(Guid heaterId);

        IHeatSensorLogic AddHeatSensor(float x, float y);

        bool ContainsHeatSensor(Guid sensorId);

        IHeatSensorLogic? GetHeatSensor(Guid sensorId);

        void RemoveHeatSensor(Guid sensorId);
    }
}