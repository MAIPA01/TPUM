namespace TPUM.Client.Data
{
    public interface IRoomData
    {
        Guid Id { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterData> Heaters { get; }
        IReadOnlyCollection<IHeatSensorData> HeatSensors { get; }
        IHeaterData AddHeater(float x, float y, float temperature);

        void RemoveHeater(Guid id);

        void ClearHeaters();

        IHeatSensorData AddHeatSensor(float x, float y);

        void RemoveHeatSensor(Guid id);

        void ClearHeatSensors();
    }
}
