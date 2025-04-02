using System.Collections.ObjectModel;

namespace TPUM.Logic
{
    public interface IRoom : INotifyTemperatureChanged, INotifyEnableChanged, INotifyPositionChanged, IDisposable
    {
        long Id { get; }
        IReadOnlyCollection<IHeater> Heaters { get; }
        IReadOnlyCollection<IHeatSensor> HeatSensors { get; }
        float Width { get; }
        float Height { get; }

        float AvgTemperature { get; }

        float GetTemperatureAtPosition(float x, float y);

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
