using TPUM.Client.Logic;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class TestPositionLogic : IPositionLogic
    {
        public float X { get; }
        public float Y { get; }

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

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        public Guid Id { get; } = Guid.NewGuid();
        public IPositionLogic Position { get; }
        public float Temperature { get; set; }

        public bool IsOn => throw new NotImplementedException();

        public TestHeaterLogic(float x = 0f, float y = 0f, float temperature = 20f)
        {
            Position = new TestPositionLogic(x, y);
            Temperature = temperature;
        }

        public void SetPosition(float x, float y)
        {
            throw new NotImplementedException();
        }

        public void TurnOn()
        {
            throw new NotImplementedException();
        }

        public void TurnOff()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class TestHeatSensorLogic : IHeatSensorLogic
    {

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; } = Guid.NewGuid();
        public IPositionLogic Position { get; }
        public float Temperature { get; }

        public TestHeatSensorLogic(float x, float y, float temperature = 22f)
        {
            Position = new TestPositionLogic(x, y);
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

    internal class TestRoomLogic : IRoomLogic
    {

        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = "Test Room";
        public float Width { get; } = 10;
        public float Height { get; } = 5;
        public List<IHeaterLogic> Heaters { get; }
        public List<IHeatSensorLogic> HeatSensors { get; }

        IReadOnlyCollection<IHeaterLogic> IRoomLogic.Heaters => Heaters.AsReadOnly();

        IReadOnlyCollection<IHeatSensorLogic> IRoomLogic.HeatSensors => HeatSensors.AsReadOnly();

        public TestRoomLogic()
        {
            Heaters = new List<IHeaterLogic> { new TestHeaterLogic(0f, 0f, 23f) };
            HeatSensors = new List<IHeatSensorLogic> { new TestHeatSensorLogic(0f, 0f) };
        }

        public void AddHeater(float x, float y, float temperature)
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

        public void AddHeatSensor(float x, float y)
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
