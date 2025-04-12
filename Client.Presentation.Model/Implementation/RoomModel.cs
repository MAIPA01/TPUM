using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class RoomModel : IRoomModel
    {
        private readonly IRoomLogic _logic;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;

        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;

        private readonly object _roomLock = new();

        public Guid Id => _logic.Id;
        public string Name => _logic.Name;
        public float Width => _logic.Width;
        public float Height => _logic.Height;

        private readonly List<HeaterModel> _heaters = [];
        public IReadOnlyCollection<IHeaterModel> Heaters
        {
            get
            {
                lock (_roomLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<HeatSensorModel> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorModel> HeatSensors
        {
            get
            {
                lock (_roomLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public RoomModel(IRoomLogic logic)
        {
            _logic = logic;
            foreach (var heaterLogic in logic.Heaters)
            {
                var heater = new HeaterModel(heaterLogic);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
            }

            foreach (var sensorLogic in logic.HeatSensors)
            {
                var sensor = new HeatSensorModel(sensorLogic);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
            }

            _logic.HeaterAdded += GetHeaterAdded;
            _logic.HeaterRemoved += GetHeaterRemoved;
            _logic.HeatSensorAdded += GetHeatSensorAdded;
            _logic.HeatSensorRemoved += GetHeatSensorRemoved;
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
        }

        private void GetPositionChanged(object? source, IPositionModel lastPosition, IPositionModel newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
        }

        private void GetEnabledChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
        }

        private void GetHeaterAdded(object? source, IHeaterLogic heaterLogic)
        {
            lock (_roomLock)
            {
                var heater = new HeaterModel(heaterLogic);
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

        private void GetHeatSensorAdded(object? source, IHeatSensorLogic sensorLogic)
        {
            lock (_roomLock)
            {
                var sensor = new HeatSensorModel(sensorLogic);
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

        private void SubscribeToHeater(HeaterModel heater)
        {
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.PositionChanged += GetPositionChanged;
            heater.EnableChanged += GetEnabledChanged;
        }

        private void UnsubscribeFromHeater(HeaterModel heater)
        {
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.PositionChanged -= GetPositionChanged;
            heater.EnableChanged -= GetEnabledChanged;
        }

        public void AddHeater(float x, float y, float temperature)
        {
            _logic.AddHeater(x, y, temperature);
        }

        public bool ContainsHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                return _heaters.Any(heater => heater.Id == heaterId);
            }
        }

        public IHeaterModel? GetHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null) return heater;

                var logicHeater = _logic.GetHeater(heaterId);
                if (logicHeater == null) return null;

                heater = new HeaterModel(logicHeater);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
                return heater;
            }
        }

        public void RemoveHeater(Guid heaterId)
        {
            _logic.RemoveHeater(heaterId);
        }

        private void SubscribeToHeatSensor(HeatSensorModel sensor)
        {
            sensor.TemperatureChanged += GetTemperatureChanged;
            sensor.PositionChanged += GetPositionChanged;
        }

        private void UnsubscribeFromHeatSensor(HeatSensorModel sensor)
        {
            sensor.TemperatureChanged -= GetTemperatureChanged;
            sensor.PositionChanged -= GetPositionChanged;
        }

        public void AddHeatSensor(float x, float y)
        {
            _logic.AddHeatSensor(x, y);
        }

        public bool ContainsHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                return _heatSensors.Any(sensor => sensor.Id == sensorId);
            }
        }

        public IHeatSensorModel? GetHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null) return sensor;

                var logicSensor = _logic.GetHeatSensor(sensorId);
                if (logicSensor == null) return null;

                sensor = new HeatSensorModel(logicSensor);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
                return sensor;
            }
        }

        public void RemoveHeatSensor(Guid sensorId)
        {
            _logic.RemoveHeatSensor(sensorId);
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