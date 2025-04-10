﻿namespace TPUM.Client.Data
{
    internal class HeatSensorData : IHeatSensorData
    {
        public Guid Id { get; }
        public IPositionData Position { get; set; }
        public float Temperature { get; set; }

        public HeatSensorData(Guid id, IPositionData position, float temperature = 0.0f)
        {
            Id = id;
            Position = position;
            Temperature = temperature;
        }
    }
}