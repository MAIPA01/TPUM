using TPUM.Server.Data;
using TPUM.Server.Data.Events;

namespace TPUM.Server.Logic.Tests
{
    public class HeaterData : IHeaterData
    {
        public Guid Id { get; } = Guid.NewGuid();
        public bool IsOn { get; set; }
        public float Temperature { get; set; }
        public IPositionData Position { get; }

        public event EnableChangedEventHandler? EnableChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;

        public HeaterData(float x = 0f, float y = 0f, float temperature = 0f)
        {
            Position = new PositionData(x, y);
            Temperature = temperature;
        }

        public void SetPosition(float x, float y)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class HeatSensorData : IHeatSensorData
    {
        public Guid Id { get; } = Guid.NewGuid();
        public float Temperature { get; set; }
        public IPositionData Position { get; }

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;

        public HeatSensorData(float x = 0f, float y = 0f)
        {
            Position = new PositionData(x, y);
        }

        public void SetPosition(float x, float y)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class PositionData : IPositionData
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PositionData() { }
        public PositionData(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class RoomData : IRoomData
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = "TestRoom";
        public float Width { get; } = 10f;
        public float Height { get; } = 10f;
        public List<IHeaterData> Heaters { get; } = new();
        public List<IHeatSensorData> HeatSensors { get; } = new();

        IReadOnlyCollection<IHeaterData> IRoomData.Heaters => Heaters;

        IReadOnlyCollection<IHeatSensorData> IRoomData.HeatSensors => HeatSensors;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        public IHeaterData AddHeater(float x, float y, float temp)
        {
            var heater = new HeaterData { Temperature = temp };
            Heaters.Add(heater);
            return heater;
        }

        public void RemoveHeater(Guid id) => Heaters.RemoveAll(h => h.Id == id);
        public void ClearHeaters() => Heaters.Clear();

        public IHeatSensorData AddHeatSensor(float x, float y)
        {
            var sensor = new HeatSensorData(x, y);
            HeatSensors.Add(sensor);
            return sensor;
        }

        public void RemoveHeatSensor(Guid id) => HeatSensors.RemoveAll(h => h.Id == id);
        public void ClearHeatSensors() => HeatSensors.Clear();

        public bool ContainsHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public IHeaterData? GetHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool ContainsHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public IHeatSensorData? GetHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
