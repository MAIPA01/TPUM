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
            _data.HeaterAdded += GetHeaterAdded;
            _data.HeaterRemoved += GetHeaterRemoved;
            _data.HeatSensorAdded += GetHeatSensorAdded;
            _data.HeatSensorRemoved += GetHeatSensorRemoved;
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

        private void GetHeaterAdded(object? source, Guid roomId, IHeaterData heater)
        {
            if (roomId != Id) return;
            lock (_roomLock)
            {
                SubscribeToHeater(heater);
                _heaters.Add(heater);
                HeaterAdded?.Invoke(this, roomId, heater);
            }
        }

        public void AddHeater(float x, float y, float temperature)
        {
            _data.AddHeater(Id, x, y, temperature);
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

        private void GetHeaterRemoved(object? source, Guid roomId, Guid heaterId)
        {
            if (roomId != Id) return;
            lock (_roomLock)
            {
                IHeaterData? heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null)
                {
                    UnsubscribeFromHeater(heater);
                    _heaters.Remove(heater);
                }
                HeaterRemoved?.Invoke(this, roomId, heaterId);
            }
        }

        public void RemoveHeater(Guid id)
        {
            _data.RemoveHeater(Id, id);
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

        private void GetHeatSensorAdded(object? source, Guid roomId, IHeatSensorData sensor)
        {
            lock (_roomLock)
            {
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
                HeatSensorAdded?.Invoke(this, roomId, sensor);
            }
        }

        public void AddHeatSensor(float x, float y)
        {
            _data.AddHeatSensor(Id, x, y);
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

        private void GetHeatSensorRemoved(object? source, Guid roomId, Guid sensorId)
        {
            if (Id != roomId) return;
            lock (_roomLock)
            {
                IHeatSensorData? sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null)
                {
                    UnsubscribeFromHeatSensor(sensor);
                    _heatSensors.Remove(sensor);
                }
                HeatSensorRemoved?.Invoke(this, roomId, sensorId);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            _data.RemoveHeatSensor(Id, id);
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
            _data.HeaterAdded -= GetHeaterAdded;
            _data.HeaterRemoved -= GetHeaterRemoved;
            _data.HeatSensorAdded -= GetHeatSensorAdded;
            _data.HeatSensorRemoved -= GetHeatSensorRemoved;
            ClearHeaters();
            ClearHeatSensors();
            GC.SuppressFinalize(this);
        }
    }
}
