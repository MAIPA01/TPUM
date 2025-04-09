using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    public interface IRoomLogic : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        Guid Id { get; }
        IReadOnlyCollection<IHeaterLogic> Heaters { get; }
        IReadOnlyCollection<IHeatSensorLogic> HeatSensors { get; }
        float Width { get; }
        float Height { get; }

        float AvgTemperature { get; }

        IHeaterLogic AddHeater(float x, float y, float temperature);

        bool ContainsHeater(Guid id);

        IHeaterLogic? GetHeater(Guid id);

        void RemoveHeater(Guid id);

        void ClearHeaters();

        IHeatSensorLogic AddHeatSensor(float x, float y);

        bool ContainsHestSensor(Guid id);

        IHeatSensorLogic? GetHeatSensor(Guid id);

        void RemoveHeatSensor(Guid id);

        void ClearHeatSensors();

        void StartSimulation();

        void EndSimulation();
    }
}