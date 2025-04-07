namespace TPUM.Client.Logic
{
    public interface IRoom : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        long Id { get; }
        IReadOnlyCollection<IHeater> Heaters { get; }
        IReadOnlyCollection<IHeatSensor> HeatSensors { get; }
        float Width { get; }
        float Height { get; }

        float AvgTemperature { get; }

        IHeater AddHeater(float x, float y, float temperature);

        void RemoveHeater(long id);

        void ClearHeaters();

        IHeatSensor AddHeatSensor(float x, float y);

        void RemoveHeatSensor(long id);

        void ClearHeatSensors();

        void StartSimulation();

        void EndSimulation();
    }
}