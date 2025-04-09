namespace TPUM.Client.Logic
{
    public interface IRoomLogic : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterLogic> Heaters { get; }
        IReadOnlyCollection<IHeatSensorLogic> HeatSensors { get; }
        float AvgTemperature { get; }

        IHeaterLogic AddHeater(IHeaterLogic logic);
        bool ContainsHeater(Guid id);
        IHeaterLogic? GetHeater(Guid id);
        void RemoveHeater(Guid id);
        IHeatSensorLogic AddHeatSensor(IHeatSensorLogic logic);
        bool ContainsHeatSensor(Guid id);
        IHeatSensorLogic? GetHeatSensor(Guid id);
        void RemoveHeatSensor(Guid id);
    }
}