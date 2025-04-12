using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class Room : IRoom
    {
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly IRoomLogic _logic;

        private readonly object _roomLock = new();

        public Guid Id => _logic.Id;
        public string Name => _logic.Name;
        public float Width => _logic.Width;
        public float Height => _logic.Height;

        private readonly List<Heater> _heaters = [];
        public IReadOnlyCollection<IHeater> Heaters
        {
            get
            {
                lock (_roomLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<HeatSensor> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensor> HeatSensors
        {
            get
            {
                lock (_roomLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public Room(IRoomLogic logic)
        {
            _logic = logic;
            foreach (var logicHeater in _logic.Heaters)
            {
                var heater = new Heater(logicHeater);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
            }

            foreach (var logicHeatSensor in _logic.HeatSensors)
            {
                var sensor = new HeatSensor(logicHeatSensor);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
            }
        }

        private void GetPositionChanged(Guid roomId, object? source, IPosition lastPosition, IPosition newPosition)
        {
            PositionChanged?.Invoke(Id, source, lastPosition, newPosition);
        }

        private void GetTemperatureChanged(Guid roomId, object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(Id, source, lastTemperature, newTemperature);
        }

        private void GetEnableChanged(Guid roomId, object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(Id,source, lastEnable, newEnable);
        }

        private void SubscribeToHeater(Heater heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(Heater heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnableChanged;
        }

        public IHeater AddHeater(float x, float y, float temperature)
        {
            var heater = new Heater(_logic.AddHeater(x, y, temperature));
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

        public IHeater? GetHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null) return heater;

                var logicHeater = _logic.GetHeater(heaterId);
                if (logicHeater == null) return null;

                heater = new Heater(logicHeater);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
                return heater;
            }
        }

        public void RemoveHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null)
                {
                    UnsubscribeFromHeater(heater);
                    _heaters.Remove(heater);
                }
            }
            _logic.RemoveHeater(heaterId);
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

        private void SubscribeToHeatSensor(HeatSensor sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(HeatSensor sensor)
        {
            sensor.PositionChanged -= GetPositionChanged;
            sensor.TemperatureChanged -= GetTemperatureChanged;
        }

        public IHeatSensor AddHeatSensor(float x, float y)
        {
            var sensor = new HeatSensor(_logic.AddHeatSensor(x, y));
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

        public IHeatSensor? GetHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null) return sensor;

                var logicSensor = _logic.GetHeatSensor(sensorId);
                if (logicSensor == null) return null;

                sensor = new HeatSensor(logicSensor);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
                return sensor;
            }
        }

        public void RemoveHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null)
                {
                    UnsubscribeFromHeatSensor(sensor);
                    _heatSensors.Remove(sensor);
                }
            }
            _logic.RemoveHeatSensor(sensorId);
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
