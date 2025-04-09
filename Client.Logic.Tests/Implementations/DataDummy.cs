using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    public class TestPositionData : IPositionData
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class TestHeaterData : IHeaterData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IPositionData Position { get; set; } = new TestPositionData();
        public float Temperature { get; set; }
        public bool IsOn { get; set; }
    }

    public class TestHeatSensorData : IHeatSensorData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public IPositionData Position { get; set; } = new TestPositionData();
        public float Temperature { get; set; }
    }

    public class TestRoomData : IRoomData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "TestRoom";
        public float Width { get; set; } = 5.0f;
        public float Height { get; set; } = 4.0f;

        public IReadOnlyCollection<IHeaterData> Heaters => throw new NotImplementedException();

        public IReadOnlyCollection<IHeatSensorData> HeatSensors => throw new NotImplementedException();

        public IHeaterData AddHeater(IHeaterData heater)
        {
            throw new NotImplementedException();
        }

        public IHeatSensorData AddHeatSensor(IHeatSensorData sensor)
        {
            throw new NotImplementedException();
        }

        public bool ContainsHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool ContainsHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IHeaterData? GetHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public IHeatSensorData? GetHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
