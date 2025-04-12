using TPUM.Client.Data;
using TPUM.Client.Data.Events;

namespace TPUM.Client.Logic.Tests
{
    public class TestPositionData : IPositionData
    {
        public float X { get; internal set; } = 0f;
        public float Y { get; internal set; } = 0f;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class TestHeaterData : IHeaterData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; } = Guid.NewGuid();
        internal readonly TestPositionData _position = new TestPositionData();
        public IPositionData Position => _position;
        public float Temperature { get; set; } = 0f;
        public bool IsOn { get; set; } = false;

        public void SetPosition(float x, float y)
        {
            _position.X = x;
            _position.Y = y;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class TestHeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id { get; } = Guid.NewGuid();
        internal readonly TestPositionData _position = new TestPositionData();
        public IPositionData Position => _position;
        public float Temperature { get; } = 0f;
        
        public void SetPosition(float x, float y)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class TestRoomData : IRoomData
    {
        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = "TestRoom";
        public float Width { get; } = 5.0f;
        public float Height { get; } = 4.0f;

        private readonly List<TestHeaterData> _heaters = [];
        public IReadOnlyCollection<IHeaterData> Heaters => _heaters.AsReadOnly();

        private readonly List<TestHeatSensorData> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorData> HeatSensors => _heatSensors.AsReadOnly();

        public void AddHeater(float x, float y, float temperature)
        {
            _heaters.Add(new TestHeaterData() {_position = { X = x, Y = y }, Temperature = temperature});
        }

        public void AddHeatSensor(float x, float y)
        {
            _heatSensors.Add(new TestHeatSensorData() {_position = { X = x, Y = y }});
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
