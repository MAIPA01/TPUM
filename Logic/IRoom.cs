using TPUM.Data;

namespace TPUM.Logic
{
    public interface IRoom : IObservable<IRoom>, IObserver<IHeater>, IObserver<IHeatSensor>, IDisposable
    {
        public List<IHeater> Heaters { get; }
        public List<IHeatSensor> HeatSensors { get; }
        public float Width { get; }
        public float Height { get; }

        public float GetAvgTemperature();

        public float GetTemperatureAtPosition(Position pos);

        public void AddHeater(Position pos, float temperature);

        public void AddHeatSensor(Position pos);

        public void UpdateTemperature(float deltaTime);
    }
}
