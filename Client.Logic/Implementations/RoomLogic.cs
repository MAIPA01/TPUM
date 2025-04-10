using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic
{
    internal class RoomLogic : IRoomLogic
    {
        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        private readonly IRoomData _data;
        public Guid Id => _data.Id;

        private readonly object _roomLock = new();

        private readonly List<IHeaterLogic> _heaters = [];
        public IReadOnlyCollection<IHeaterLogic> Heaters
        {
            get
            {
                lock (_roomLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<IHeatSensorLogic> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorLogic> HeatSensors
        {
            get
            {
                lock (_roomLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public string Name => _data.Name;
        public float Width => _data.Width;
        public float Height => _data.Height;

        public RoomLogic(IRoomData data)
        {
            _data = data;

            foreach (var heaterData in _data.Heaters)
            {
                var heater = new HeaterLogic(heaterData);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
            }

            foreach (var sensorData in _data.HeatSensors)
            {
                var sensor = new HeatSensorLogic(sensorData);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
            }

            _data.HeaterAdded += GetHeaterAdded;
            _data.HeaterRemoved += GetHeaterRemoved;
            _data.HeatSensorAdded += GetHeatSensorAdded;
            _data.HeatSensorRemoved += GetHeatSensorRemoved;
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
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

        private void GetHeaterAdded(object? source, IHeaterData heaterData)
        {
            lock (_roomLock)
            {
                var heater = new HeaterLogic(heaterData);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
                HeaterAdded?.Invoke(this, heater);
            }
        }

        private void GetHeaterRemoved(object? source, Guid heaterId)
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

        private void GetHeatSensorAdded(object? source, IHeatSensorData sensorData)
        {
            lock (_roomLock)
            {
                var sensor = new HeatSensorLogic(sensorData);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
                HeatSensorAdded?.Invoke(this, sensor);
            }
        }

        private void GetHeatSensorRemoved(object? source, Guid sensorId)
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
            }
        }

        private void SubscribeToHeater(IHeaterLogic heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(IHeaterLogic heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnableChanged;
        }

        public void AddHeater(float x, float y, float temperature)
        {
            _data.AddHeater(x, y, temperature);
        }

        public bool ContainsHeater(Guid id)
        {
            lock (_roomLock)
            {
                return _heaters.Find(h => h.Id == id) != null;
            }
        }

        public IHeaterLogic? GetHeater(Guid id)
        {
            lock (_roomLock)
            {
                return _heaters.Find(h => h.Id == id);
            }
        }

        public void RemoveHeater(Guid id)
        {
            _data.RemoveHeater(id);
        }

        private void SubscribeToHeatSensor(IHeatSensorLogic sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensorLogic sensor)
        {
            sensor.PositionChanged -= GetPositionChanged;
            sensor.TemperatureChanged -= GetTemperatureChanged;
        }

        public void AddHeatSensor(float x, float y)
        {
            _data.AddHeatSensor(x, y);
        }

        public bool ContainsHeatSensor(Guid id)
        {
            lock (_roomLock)
            {
                return _heatSensors.Find(h => h.Id == id) != null;
            }
        }

        public IHeatSensorLogic? GetHeatSensor(Guid id)
        {
            lock (_roomLock)
            {
                return _heatSensors.Find(s => s.Id == id);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            _data.RemoveHeatSensor(id);
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeFromHeater(heater);
            }
            _heaters.Clear();

            foreach (var sensor in _heatSensors)
            {
                UnsubscribeFromHeatSensor(sensor);
            }
            _heatSensors.Clear();

            GC.SuppressFinalize(this);
        }
    }
}