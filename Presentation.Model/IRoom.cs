namespace TPUM.Presentation.Model
{
    public interface IRoom : INotifyTemperatureChanged, INotifyPositionChanged, INotifyEnableChanged, IDisposable
    {
        long Id { get; }
        string Name { get; set; }
        float Width { get; }
        float Height { get; }
        float AvgTemperature { get; }

        IReadOnlyCollection<IHeater> Heaters { get; }
        IReadOnlyCollection<IHeatSensor> HeatSensors { get; }

        IHeater AddHeater(float x, float y, float temperature);
        void RemoveHeater(long id);
        void ClearHeaters();
        IHeatSensor AddHeatSensor(float x, float y);
        void RemoveHeatSensor(long id);
        void ClearHeatSensors();
    }
}