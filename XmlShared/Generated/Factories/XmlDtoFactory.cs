﻿namespace TPUM.XmlShared.Generated.Factory
{
    public static class XmlDtoFactory
    {
        public static RoomDataContract CreateRoomDto(Guid roomId, string name, float width, float height, 
            IEnumerable<HeaterDataContract> heaters, IEnumerable<HeatSensorDataContract> sensors)
        {
            return new RoomDataContract
            {
                RoomId = roomId,
                Name = name,
                Width = width,
                Height = height,
                Heaters = [.. heaters],
                HeatSensors = [.. sensors]
            };
        }

        public static HeaterDataContract CreateHeaterDto(Guid heaterId, float x, float y, float temperature, bool isOn)
        {
            return new HeaterDataContract
            {
                HeaterId = heaterId,
                X = x,
                Y = y,
                Temperature = temperature,
                IsOn = isOn
            };
        }

        public static HeatSensorDataContract CreateHeatSensorDto(Guid sensorId, float x, float y, float temperature)
        {
            return new HeatSensorDataContract
            {
                HeatSensorId = sensorId,
                X = x,
                Y = y,
                Temperature = temperature
            };
        }
    }
}
