using TPUM.Server.Data.Events;

namespace TPUM.Server.Data.Tests
{
    internal class DummyRoomData : IRoomData
    {
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _roomLock = new();

        public Guid Id { get; }
        public string Name { get; }
        public float Width { get; }
        public float Height { get; }

        private readonly List<DummyHeaterData> _heaters = [];
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

        private readonly List<DummyHeatSensorData> _heatSensors = [];
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

        private void GetPositionChange(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
        }

        private void GetTemperatureChange(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(source, lastTemperature, newTemperature);
        }

        private void GetEnableChange(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
        }

        private void SubscribeToHeater(IHeaterData heater)
        {
            heater.PositionChanged += GetPositionChange;
            heater.TemperatureChanged += GetTemperatureChange;
            heater.EnableChanged += GetEnableChange;
        }

        private void UnsubscribeFromHeater(IHeaterData heater)
        {
            heater.PositionChanged -= GetPositionChange;
            heater.TemperatureChanged -= GetTemperatureChange;
            heater.EnableChanged -= GetEnableChange;
        }

        public IHeaterData AddHeater(float x, float y, float temperature)
        {
            var heater = new DummyHeaterData(Guid.NewGuid(), x, y, temperature);
            SubscribeToHeater(heater);
            lock (_roomLock)
            {
                _heaters.Add(heater);
            }
            return heater;
        }

        public bool ContainsHeater(Guid id)
        {
            lock (_roomLock)
            {
                return _heaters.Find(heater => heater.Id == id) != null;
            }
        }

        public IHeaterData? GetHeater(Guid id)
        {
            lock (_roomLock)
            {
                return _heaters.Find(heater => heater.Id == id);
            }
        }

        public void RemoveHeater(Guid id)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == id);
                if (heater == null) return;
                UnsubscribeFromHeater(heater);
                _heaters.Remove(heater);
            }
        }

        public void ClearHeaters()
        {
            lock (_roomLock)
            {
                foreach (var heater in _heaters)
                {
                    UnsubscribeFromHeater(heater);
                }
                _heaters.Clear();
            }
        }

        private void SubscribeToHeatSensor(IHeatSensorData sensor)
        {
            sensor.PositionChanged += GetPositionChange;
            sensor.TemperatureChanged += GetTemperatureChange;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensorData sensor)
        {
            sensor.PositionChanged -= GetPositionChange;
            sensor.TemperatureChanged -= GetTemperatureChange;
        }

        public IHeatSensorData AddHeatSensor(float x, float y)
        {
            var sensor = new DummyHeatSensorData(Guid.NewGuid(), x, y);
            SubscribeToHeatSensor(sensor);
            lock (_roomLock)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public bool ContainsHeatSensor(Guid id)
        {
            lock (_roomLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == id) != null;
            }
        }

        public IHeatSensorData? GetHeatSensor(Guid id)
        {
            lock (_roomLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == id);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == id);
                if (sensor == null) return;
                UnsubscribeFromHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
        }

        public void ClearHeatSensors()
        {
            lock (_roomLock)
            {
                foreach (var sensor in _heatSensors)
                {
                    UnsubscribeFromHeatSensor(sensor);
                }
                _heatSensors.Clear();
            }
        }
        public void Dispose()
        {
            ClearHeaters();
            ClearHeatSensors();
            GC.SuppressFinalize(this);
        }
    }
}
