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
        public bool ContainsHeater(Guid heaterId);
        public IHeaterModel? GetHeater(Guid heaterId);
        void RemoveHeater(Guid heaterId);
        void AddHeatSensor(float x, float y);
        public bool ContainsHeatSensor(Guid sensorId);
        public IHeatSensorModel? GetHeatSensor(Guid sensorId);
        void RemoveHeatSensor(Guid sensorId);
    }
}