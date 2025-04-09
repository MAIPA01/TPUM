﻿namespace TPUM.Server.Data
{
    public interface IRoomData
    {
        Guid Id { get; }
        float Width { get; }
        float Height { get; }
        IReadOnlyCollection<IHeaterData> Heaters { get; }
        IReadOnlyCollection<IHeatSensorData> HeatSensors { get; }

        IHeaterData AddHeater(float x, float y, float temperature);
        bool ContainsHeater(Guid id);
        IHeaterData? GetHeater(Guid id);
        void RemoveHeater(Guid id);
        void ClearHeaters();
        IHeatSensorData AddHeatSensor(float x, float y);
        bool ContainsHeatSensor(Guid id);
        IHeatSensorData? GetHeatSensor(Guid id);
        void RemoveHeatSensor(Guid id);
        void ClearHeatSensors();
    }
}
