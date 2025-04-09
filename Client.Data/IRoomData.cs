namespace TPUM.Client.Data
{
    public interface IRoomData : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterData> Heaters { get; }
        IReadOnlyCollection<IHeatSensorData> HeatSensors { get; }
        IHeaterData AddHeater(IHeaterData heater);
        bool ContainsHeater(Guid id);
        IHeaterData? GetHeater(Guid id);
        void RemoveHeater(Guid id);
        IHeatSensorData AddHeatSensor(IHeatSensorData sensor);
        bool ContainsHeatSensor(Guid id);
        IHeatSensorData? GetHeatSensor(Guid id);
        void RemoveHeatSensor(Guid id);
    }
}
