using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class Room : IRoom
    {
        private readonly IRoomLogic _logic;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public Guid Id => _logic.Id;
        public string Name { get; set; }
        public float Width => _logic.Width;
        public float Height => _logic.Height;
        public float AvgTemperature => _logic.AvgTemperature;

        private readonly List<IHeater> _heaters = [];
        public IReadOnlyCollection<IHeater> Heaters => _heaters.AsReadOnly();

        private readonly List<IHeatSensor> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensor> HeatSensors => _heatSensors.AsReadOnly();

        public Room(string name, IRoomLogic logic)
        {
            _logic = logic;
            _logic.TemperatureChanged += GetTemperatureChanged;
            Name = name;

            foreach (var heater in logic.Heaters)
            {
                var modelHeater = new Heater(heater);
                SubscribeToHeater(modelHeater);
                _heaters.Add(modelHeater);
            }

            foreach (var sensor in logic.HeatSensors)
            {
                var modelSensor = new HeatSensor(sensor);
                SubscribeToHeatSensor(modelSensor);
                _heatSensors.Add(modelSensor);
            }
        }

        private void GetTemperatureChanged(object? source, Logic.Events.TemperatureChangedEventArgs args)
        {
            if (source != _logic) return;
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetTemperatureChanged(object? source, TemperatureChangedEventArgs args)
        {
            if (source != _logic) return;
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
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
            var modelSensor = new HeatSensor(_logic.AddHeatSensor(x, y));
            SubscribeToHeatSensor(modelSensor);
            _heatSensors.Add(modelSensor);
            return modelSensor;
        }

        public void RemoveHeatSensor(Guid id)
        {
            var sensor = _heatSensors.Find(sensor => sensor.Id == id);
            if (sensor != null)
            {
                UnsubscribeToHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
            _logic.RemoveHeatSensor(id);
        }

        public void ClearHeatSensors()
        {
            foreach (var sensor in _heatSensors)
            {
                UnsubscribeToHeatSensor(sensor);
            }
            _heatSensors.Clear();
            _logic.ClearHeatSensors();
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
            var modelHeater = new Heater(_logic.AddHeater(x, y, temperature));
            SubscribeToHeater(modelHeater);
            _heaters.Add(modelHeater);
            return modelHeater;
        }

        public void RemoveHeater(Guid id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater != null)
            {
                UnsubscribeToHeater(heater);
                _heaters.Remove(heater);
            }
            _logic.RemoveHeater(id);
        }

        public void ClearHeaters()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeToHeater(heater);
            }
            _heaters.Clear();
            _logic.ClearHeaters();
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