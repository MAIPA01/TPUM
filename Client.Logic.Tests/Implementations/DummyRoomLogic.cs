using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyRoomLogic : IRoomLogic
    {
        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;

        private readonly IRoomData _data;

        private readonly object _roomLock = new();
        
        public Guid Id => _data.Id;
        public string Name => _data.Name;
        public float Width => _data.Width;
        public float Height => _data.Height;

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

        public DummyRoomLogic(IRoomData data)
        {
            _data = data;
        }

        public void AddHeater(float x, float y, float temperature)
        {
            _data.AddHeater(x, y, temperature);
            var logic = new DummyHeaterLogic(_data.Heaters.Last());
            lock (_roomLock)
            {
                _heaters.Add(logic);
            }
        }

        public bool ContainsHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                return _heaters.Any(heater => heater.Id == heaterId);
            }
        }

        public IHeaterLogic? GetHeater(Guid heaterId)
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

                _heaters.Remove(heater);
            }
        }

        public void AddHeatSensor(float x, float y)
        {
            _data.AddHeatSensor(x, y);
            var logic = new DummyHeatSensorLogic(_data.HeatSensors.Last());
            lock (_roomLock)
            {
                _heatSensors.Add(logic);
            }
        }

        public bool ContainsHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                return _heatSensors.Any(sensor => sensor.Id == sensorId);
            }
        }

        public IHeatSensorLogic? GetHeatSensor(Guid sensorId)
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

                _heatSensors.Remove(sensor);
            }
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                heater.Dispose();
            }
            _heaters.Clear();

            foreach (var heatSensor in _heatSensors)
            {
                heatSensor.Dispose();
            }
            _heatSensors.Clear();

            GC.SuppressFinalize(this);
        }
    }
}