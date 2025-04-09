using TPUM.Client.Data;

namespace TPUM.Client.Logic
{
    internal class RoomLogic : IRoomLogic
    {
        private readonly IRoomData _data;
        public Guid Id => _data.Id;

        private readonly List<IHeaterLogic> _heaters = [];

        private readonly object _heatersLock = new();
        public IReadOnlyCollection<IHeaterLogic> Heaters
        {
            get
            {
                lock (_heatersLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<IHeatSensorLogic> _heatSensors = [];

        private readonly object _heatSensorsLock = new();
        public IReadOnlyCollection<IHeatSensorLogic> HeatSensors
        {
            get
            {
                lock (_heatSensorsLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public string Name => _data.Name;
        public float Width => _data.Width;
        public float Height => _data.Height;

        public float AvgTemperature
        {
            get
            {
                lock (_heatSensorsLock)
                {
                    return _heatSensors.Count == 0 ? 0f : _heatSensors.Average(heatSensor => heatSensor.Temperature);
                }
            }
        }

        public RoomLogic(IRoomData data)
        {
            _data = data;
        }

        public IHeaterLogic AddHeater(IHeaterLogic logic)
        {
            lock (_heatersLock)
            {
                _heaters.Add(logic);
            }
            return logic;
        }

        public bool ContainsHeater(Guid id)
        {
            lock (_heatersLock)
            {
                return _heaters.Any(h => h.Id == id);
            }
        }

        public IHeaterLogic? GetHeater(Guid id)
        {
            lock (_heatersLock)
            {
                return _heaters.Find(h => h.Id == id);
            }
        }

        public void RemoveHeater(Guid id)
        {
            lock (_heatersLock)
            {
                IHeaterLogic? heater = _heaters.Find(heater => heater.Id == id);
                if (heater == null) return;

                _heaters.Remove(heater);
            }
        }

        public IHeatSensorLogic AddHeatSensor(IHeatSensorLogic logic)
        {
            lock (_heatSensorsLock)
            {
                _heatSensors.Add(logic);
            }
            return logic;
        }

        public bool ContainsHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                return _heatSensors.Any(h => h.Id == id);
            }
        }

        public IHeatSensorLogic? GetHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                return _heatSensors.Find(s => s.Id == id);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                IHeatSensorLogic? sensor = _heatSensors.Find(sensor => sensor.Id == id);
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