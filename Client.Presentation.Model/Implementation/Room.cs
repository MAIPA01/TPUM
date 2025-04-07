namespace TPUM.Client.Presentation.Model
{
    internal class Room : IRoom
    {
        private readonly Logic.IRoom _room;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public long Id => _room.Id;
        public string Name { get; set; }
        public float Width => _room.Width;
        public float Height => _room.Height;
        public float AvgTemperature => _room.AvgTemperature;

        private readonly List<IHeater> _heaters = [];
        public IReadOnlyCollection<IHeater> Heaters => _heaters.AsReadOnly();

        private readonly List<IHeatSensor> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensor> HeatSensors => _heatSensors.AsReadOnly();

        public Room(string name, Logic.IRoom room)
        {
            _room = room;
            _room.TemperatureChanged += GetTemperatureChanged;
            Name = name;

            foreach (var heater in room.Heaters)
            {
                var modelHeater = new Heater(heater);
                SubscribeToHeater(modelHeater);
                _heaters.Add(modelHeater);
            }

            foreach (var sensor in room.HeatSensors)
            {
                var modelSensor = new HeatSensor(sensor);
                SubscribeToHeatSensor(modelSensor);
                _heatSensors.Add(modelSensor);
            }
        }

        private void GetTemperatureChanged(object? source, Logic.TemperatureChangedEventArgs args)
        {
            if (source != _room) return;
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetTemperatureChanged(object? source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(source, args);
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(source, args);
        }

        private void GetEnabledChanged(object? source, EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(source, args);
        }

        private void SubscribeToHeatSensor(IHeatSensor sensor)
        {
            sensor.TemperatureChanged += GetTemperatureChanged;
            sensor.PositionChanged += GetPositionChanged;
        }

        private void UnsubscribeToHeatSensor(IHeatSensor sensor)
        {
            sensor.TemperatureChanged -= GetTemperatureChanged;
            sensor.PositionChanged -= GetPositionChanged;
        }

        public IHeatSensor AddHeatSensor(float x, float y)
        {
            var modelSensor = new HeatSensor(_room.AddHeatSensor(x, y));
            SubscribeToHeatSensor(modelSensor);
            _heatSensors.Add(modelSensor);
            return modelSensor;
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.Find(sensor => sensor.Id == id);
            if (sensor != null)
            {
                UnsubscribeToHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
            _room.RemoveHeatSensor(id);
        }

        public void ClearHeatSensors()
        {
            foreach (var sensor in _heatSensors)
            {
                UnsubscribeToHeatSensor(sensor);
            }
            _heatSensors.Clear();
            _room.ClearHeatSensors();
        }

        private void SubscribeToHeater(IHeater heater)
        {
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.PositionChanged += GetPositionChanged;
            heater.EnableChanged += GetEnabledChanged;
        }

        private void UnsubscribeToHeater(IHeater heater)
        {
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.PositionChanged -= GetPositionChanged;
            heater.EnableChanged -= GetEnabledChanged;
        }

        public IHeater AddHeater(float x, float y, float temperature)
        {
            var modelHeater = new Heater(_room.AddHeater(x, y, temperature));
            SubscribeToHeater(modelHeater);
            _heaters.Add(modelHeater);
            return modelHeater;
        }

        public void RemoveHeater(long id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater != null)
            {
                UnsubscribeToHeater(heater);
                _heaters.Remove(heater);
            }
            _room.RemoveHeater(id);
        }

        public void ClearHeaters()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeToHeater(heater);
            }
            _heaters.Clear();
            _room.ClearHeaters();
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeToHeater(heater);
                heater.Dispose();
            }
            _heaters.Clear();
            foreach (var sensor in _heatSensors)
            {
                UnsubscribeToHeatSensor(sensor);
                sensor.Dispose();
            }
            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }
    }
}