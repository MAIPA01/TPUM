using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IRoom : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeater> Heaters { get; }
        IReadOnlyCollection<IHeatSensor> HeatSensors { get; }

        IHeater AddHeater(float x, float y, float temperature);

        bool ContainsHeater(Guid heaterId);

        IHeater? GetHeater(Guid heaterId);

        void RemoveHeater(Guid heaterId);

        IHeatSensor AddHeatSensor(float x, float y);

        bool ContainsHeatSensor(Guid sensorId);

        IHeatSensor? GetHeatSensor(Guid sensorId);

        void RemoveHeatSensor(Guid sensorId);
    }
}