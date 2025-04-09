using TPUM.Client.Logic;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class TestPositionLogic : IPositionLogic
    {
        public float X { get; set; }
        public float Y { get; set; }

        public TestPositionLogic(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class TestHeaterLogic : IHeaterLogic
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IPositionLogic Position { get; } = new TestPositionLogic();
        public float Temperature { get; set; }

        public bool IsOn => throw new NotImplementedException();

        public TestHeaterLogic(float temperature = 20f)
        {
            Temperature = temperature;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class TestHeatSensorLogic : IHeatSensorLogic
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IPositionLogic Position { get; } = new TestPositionLogic();
        public float Temperature { get; set; }

        public TestHeatSensorLogic(float temperature = 22f)
        {
            Temperature = temperature;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class TestRoomLogic : IRoomLogic
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = "Test Room";
        public float Width { get; } = 10;
        public float Height { get; } = 5;
        public float AvgTemperature { get; } = 21;
        public IEnumerable<IHeaterLogic> Heaters { get; }
        public IEnumerable<IHeatSensorLogic> HeatSensors { get; }

        IReadOnlyCollection<IHeaterLogic> IRoomLogic.Heaters => Heaters.ToList().AsReadOnly();

        IReadOnlyCollection<IHeatSensorLogic> IRoomLogic.HeatSensors => HeatSensors.ToList().AsReadOnly();

        public TestRoomLogic()
        {
            Heaters = new List<IHeaterLogic> { new TestHeaterLogic(23f) };
            HeatSensors = new List<IHeatSensorLogic> { new TestHeatSensorLogic(21f) };
        }

        public IHeaterLogic AddHeater(IHeaterLogic logic)
        {
            throw new NotImplementedException();
        }

        public bool ContainsHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public IHeaterLogic? GetHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveHeater(Guid id)
        {
            throw new NotImplementedException();
        }

        public IHeatSensorLogic AddHeatSensor(IHeatSensorLogic logic)
        {
            throw new NotImplementedException();
        }

        public bool ContainsHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public IHeatSensorLogic? GetHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveHeatSensor(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
