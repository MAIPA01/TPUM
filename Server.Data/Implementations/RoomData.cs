using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    internal class RoomData : IRoomData
    {
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _roomLock = new();

        public Guid Id { get; }
        public string Name { get; }
        public float Width { get; }
        public float Height { get; }

        private readonly List<HeaterData> _heaters = [];
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

        private readonly List<HeatSensorData> _heatSensors = [];
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

        public RoomData(Guid id, string name, float width, float height)
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

        private void SubscribeToHeater(HeaterData heater)
        {
            heater.PositionChanged += GetPositionChange;
            heater.TemperatureChanged += GetTemperatureChange;
            heater.EnableChanged += GetEnableChange;
        }

        private void UnsubscribeFromHeater(HeaterData heater)
        {
            heater.PositionChanged -= GetPositionChange;
            heater.TemperatureChanged -= GetTemperatureChange;
            heater.EnableChanged -= GetEnableChange;
        }

        public IHeaterData AddHeater(float x, float y, float temperature)
        {
            var heater = new HeaterData(Guid.NewGuid(), new PositionData(x, y), temperature);
            SubscribeToHeater(heater);
            lock (_roomLock)
            {
                _heaters.Add(heater);
            }
            return heater;
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
                if (heater == null) return;
                UnsubscribeFromHeater(heater);
                _heaters.Remove(heater);
            }
        }

        private void ClearHeaters()
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

        private void SubscribeToHeatSensor(HeatSensorData sensor)
        {
            sensor.PositionChanged += GetPositionChange;
            sensor.TemperatureChanged += GetTemperatureChange;
        }

        private void UnsubscribeFromHeatSensor(HeatSensorData sensor)
        {
            sensor.PositionChanged -= GetPositionChange;
            sensor.TemperatureChanged -= GetTemperatureChange;
        }

        public IHeatSensorData AddHeatSensor(float x, float y)
        {
            var sensor = new HeatSensorData(Guid.NewGuid(), new PositionData(x, y));
            SubscribeToHeatSensor(sensor);
            lock (_roomLock)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
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
                if (sensor == null) return;
                UnsubscribeFromHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
        }

        private void ClearHeatSensors()
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
