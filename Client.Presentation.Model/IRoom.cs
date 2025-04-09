using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IRoom : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        float AvgTemperature { get; }

        IReadOnlyCollection<IHeater> Heaters { get; }
        IReadOnlyCollection<IHeatSensor> HeatSensors { get; }

        IHeater? AddHeater(float x, float y, float temperature);
        void RemoveHeater(Guid id);
        IHeatSensor? AddHeatSensor(float x, float y);
        void RemoveHeatSensor(Guid id);
    }
}