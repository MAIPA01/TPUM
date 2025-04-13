using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class RoomData : IRoomData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;

        private readonly DataApi _data;

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

        public RoomData(DataApi data, Guid id, string name, float width, float height)
        {
            _data = data;
            Id = id;
            Name = name;
            Width = width;
            Height = height;
        }

        private void GetPositionChanged(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(source, lastTemperature, newTemperature);
        }

        private void GetEnableChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
        }

        private void SubscribeToHeater(IHeaterData heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(IHeaterData heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnableChanged;
        }

        internal void OnHeaterAdded(IHeaterData heater)
        {
            lock (_roomLock)
            {
                SubscribeToHeater(heater);
                _heaters.Add(heater);
                HeaterAdded?.Invoke(this, heater);
            }
        }

        public void AddHeater(float x, float y, float temperature)
        {
            _data.AddHeater(Id, x, y, temperature);
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
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null) return heater;

                _data.GetHeater(Id, heaterId);
                return null;
            }
        }

        internal void OnHeaterRemoved(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null)
                {
                    UnsubscribeFromHeater(heater);
                    _heaters.Remove(heater);
                }
                HeaterRemoved?.Invoke(this, heaterId);
            }
        }

        public void RemoveHeater(Guid heaterId)
        {
            _data.RemoveHeater(Id, heaterId);
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

        private void SubscribeToHeatSensor(IHeatSensorData sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensorData sensor)
        {
            sensor.PositionChanged -= GetPositionChanged;
            sensor.TemperatureChanged -= GetTemperatureChanged;
        }

        internal void OnHeatSensorAdded(IHeatSensorData sensor)
        {
            lock (_roomLock)
            {
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
                HeatSensorAdded?.Invoke(this, sensor);
                if (_heatSensors.Count == 1) _data.SubscribeToRoomTemperature(Id);
            }
        }

        public void AddHeatSensor(float x, float y)
        {
            _data.AddHeatSensor(Id, x, y);
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
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null) return sensor;

                _data.GetHeatSensor(Id, sensorId);
                return null;
            }
        }

        internal void OnHeatSensorRemoved(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null)
                {
                    UnsubscribeFromHeatSensor(sensor);
                    _heatSensors.Remove(sensor);
                }
                HeatSensorRemoved?.Invoke(this, sensorId);
                if (_heatSensors.Count == 0) _data.UnsubscribeFromRoomTemperature(Id);
            }
        }

        public void RemoveHeatSensor(Guid sensorId)
        {
            _data.RemoveHeatSensor(Id, sensorId);
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
