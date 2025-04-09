using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    public interface IRoomPresentation : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        Guid Id { get; }
        IReadOnlyCollection<IHeaterPresentation> Heaters { get; }
        IReadOnlyCollection<IHeatSensorPresentation> HeatSensors { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }

        float AvgTemperature { get; }

        IHeaterPresentation AddHeater(float x, float y, float temperature);

        bool ContainsHeater(Guid id);

        IHeaterPresentation? GetHeater(Guid id);

        void RemoveHeater(Guid id);

        void ClearHeaters();

        IHeatSensorPresentation AddHeatSensor(float x, float y);

        bool ContainsHeatSensor(Guid id);

        IHeatSensorPresentation? GetHeatSensor(Guid id);

        void RemoveHeatSensor(Guid id);

        void ClearHeatSensors();
    }
}
