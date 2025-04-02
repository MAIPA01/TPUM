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

        ICommand ClearHeatSensorsCommand { get; }
        ICommand ClearHeatersCommand { get; }

        IHeater AddHeater(float x, float y, float temperature);
        void RemoveHeater(long id);
        IHeatSensor AddHeatSensor(float x, float y);
        void RemoveHeatSensor(long id);
    }
}