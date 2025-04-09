namespace TPUM.Client.Data
{
    internal class RoomData : IRoomData
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        private readonly object _heatersLock = new();
        private readonly List<IHeaterData> _heaters = [];
        public IReadOnlyCollection<IHeaterData> Heaters
        {
            get
            {
                lock (_heatersLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly object _heatSensorsLock = new();
        private readonly List<IHeatSensorData> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorData> HeatSensors
        {
            get
            {
                lock (_heatSensorsLock)
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

        public IHeaterData AddHeater(IHeaterData heater)
        {
            lock (_heatersLock)
            {
                _heaters.Add(heater);
            }
            return heater;
        }

        public bool ContainsHeater(Guid id)
        {
            lock ( _heatersLock)
            {
                return _heaters.Find(heater => heater.Id == id) != null;
            }
        }

        public IHeaterData? GetHeater(Guid id)
        {
            lock (_heatersLock)
            {
                return _heaters.Find(heater => heater.Id == id);
            }
        }

        public void RemoveHeater(Guid id)
        {
            lock (_heatersLock)
            {
                IHeaterData? heater = _heaters.Find(heater => heater.Id == id);
                if (heater != null) _heaters.Remove(heater);
            }
        }

        public IHeatSensorData AddHeatSensor(IHeatSensorData sensor)
        {
            lock (_heatSensorsLock)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public bool ContainsHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == id) != null;
            }
        }

        public IHeatSensorData? GetHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == id);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                IHeatSensorData? sensor = _heatSensors.Find(sensor => sensor.Id == id);
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
