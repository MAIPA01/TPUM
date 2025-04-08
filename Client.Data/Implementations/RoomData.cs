namespace TPUM.Client.Data
{
    internal class RoomData : IRoomData
    {
        public Guid Id { get; }
        public float Width { get; }
        public float Height { get; }

        private readonly object _heatersLock = new();
        private readonly List<IHeaterData> _heaters = [];
        public IReadOnlyCollection<IHeaterData> Heaters
        { 
            get
            {
                return _heaters.AsReadOnly();
            }
        }

        private readonly object _heatSensorsLock = new();
        private readonly List<IHeatSensorData> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorData> HeatSensors
        {
            get
            {
                return _heatSensors.AsReadOnly();
            }
        }

        public RoomData(Guid id, float width, float height)
        {
            Id = id;
            Width = width;
            Height = height;
        }

        public IHeaterData AddHeater(float x, float y, float temperature)
        {
            IHeaterData heater = new HeaterData(Guid.NewGuid(), new PositionData(x, y), temperature);
            lock (_heatersLock) 
            {
                _heaters.Add(heater);
            }
            return heater;
        }

        public void RemoveHeater(Guid id)
        {
            lock (_heatersLock)
            {
                IHeaterData? heater = _heaters.Find(heater => heater.Id == id);
                if (heater != null) _heaters.Remove(heater);
            }
        }

        public void ClearHeaters()
        {
            lock (_heatersLock)
            {
                _heaters.Clear();
            }
        }

        public IHeatSensorData AddHeatSensor(float x, float y)
        {
            IHeatSensorData sensor = new HeatSensorData(Guid.NewGuid(), new PositionData(x, y));
            lock (_heatSensorsLock)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public void RemoveHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                IHeatSensorData? sensor = _heatSensors.Find(sensor => sensor.Id == id);
                if (sensor != null) _heatSensors.Remove(sensor);
            }
        }

        public void ClearHeatSensors()
        {
            lock (_heatSensorsLock)
            {
                _heatSensors.Clear();
            }
        }
    }
}
