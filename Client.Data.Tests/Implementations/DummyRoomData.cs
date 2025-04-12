using TPUM.Client.Data.Events;

namespace TPUM.Client.Data.Tests
{
    internal class DummyRoomData : IRoomData
    {
        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        private readonly object _roomLock = new();

        public Guid Id { get; }
        public string Name { get; }
        public float Width { get; }
        public float Height { get; }

        private readonly List<IHeaterData> _heaters = [];
        public IReadOnlyCollection<IHeaterData> Heaters
        {
            get
            {
                lock (_roomLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<IHeatSensorData> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorData> HeatSensors
        {
            get
            {
                lock (_roomLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public DummyRoomData(Guid id, string name, float width, float height)
        {
            Id = id;
            Name = name;
            Width = width;
            Height = height;
        }

        public void AddHeater(float x, float y, float temperature)
        {
            var heater = new DummyHeaterData(Guid.NewGuid(), x, y, temperature);
            lock (_roomLock)
            {
                _heaters.Add(heater);
            }
        }

        public bool ContainsHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                return _heaters.Any(heater => heater.Id == heaterId);
            }
        }

        public IHeaterData? GetHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                return _heaters.Find(heater => heater.Id == heaterId);
            }
        }

        public void RemoveHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null) _heaters.Remove(heater);
            }
        }

        public void AddHeatSensor(float x, float y)
        {
            var sensor = new DummyHeatSensorData(Guid.NewGuid(), x, y);
            lock (_roomLock)
            {
                _heatSensors.Add(sensor);
            }
        }

        public bool ContainsHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                return _heatSensors.Any(sensor => sensor.Id == sensorId);
            }
        }

        public IHeatSensorData? GetHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == sensorId);
            }
        }

        public void RemoveHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null) _heatSensors.Remove(sensor);
            }
        }

        public void Dispose()
        {
            _heaters.Clear();
            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
