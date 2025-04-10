using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    public interface IRoomModel : INotifyHeaterAdded, INotifyHeaterRemoved, INotifyHeatSensorAdded, INotifyHeatSensorRemoved,
        INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }

        IReadOnlyCollection<IHeaterModel> Heaters { get; }
        IReadOnlyCollection<IHeatSensorModel> HeatSensors { get; }

        void AddHeater(float x, float y, float temperature);
        public IHeaterModel? GetHeater(Guid id);
        void RemoveHeater(Guid id);
        void AddHeatSensor(float x, float y);
        public IHeatSensorModel? GetHeatSensor(Guid id);
        void RemoveHeatSensor(Guid id);
    }
}