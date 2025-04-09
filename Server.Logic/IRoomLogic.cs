using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    public interface IRoomLogic : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        Guid Id { get; }
        IReadOnlyCollection<IHeaterLogic> Heaters { get; }
        IReadOnlyCollection<IHeatSensorLogic> HeatSensors { get; }
        float Width { get; }
        float Height { get; }

        float RoomTemperature { get; set; }
        float AvgTemperature { get; }

        float GetTemperatureAtPosition(float x, float y);

        IHeaterLogic AddHeater(float x, float y, float temperature);

        void RemoveHeater(Guid id);

        void ClearHeaters();

        IHeatSensorLogic AddHeatSensor(float x, float y);

        void RemoveHeatSensor(Guid id);

        void ClearHeatSensors();

        void StartSimulation();

        void EndSimulation();
    }
}