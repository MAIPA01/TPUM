using TPUM.Server.Logic;
using TPUM.Server.Presentation.Events;

namespace TPUM.Server.Presentation
{
    internal class RoomPresentation : IRoomPresentation
    {
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly IRoomLogic _logic;

        public Guid Id => _logic.Id;
        private readonly List<IHeaterPresentation> _heaters = [];
        public IReadOnlyCollection<IHeaterPresentation> Heaters => _heaters.AsReadOnly();
        private readonly List<IHeatSensorPresentation> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorPresentation> HeatSensors => _heatSensors.AsReadOnly();
        public string Name => _logic.Name;
        public float Width => _logic.Width;
        public float Height => _logic.Height;
        public float AvgTemperature => _logic.AvgTemperature;

        public RoomPresentation(IRoomLogic logic)
        {
            _logic = logic;
            _logic.TemperatureChanged += GetTemperatureChanged;

            foreach (var logicHeater in _logic.Heaters)
            {
                var heater = new HeaterPresentation(logicHeater);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
            }

            foreach (var logicHeatSensor in _logic.HeatSensors)
            {
                var sensor = new HeatSensorPresentation(logicHeatSensor);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
            }
        }

        private void GetPositionChanged(object? source, IPositionPresentation lastPosition, IPositionPresentation newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(source == _logic ? this : source, lastTemperature, newTemperature);
        }

        private void GetEnableChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
        }

        private void SubscribeToHeater(IHeaterPresentation heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(IHeaterPresentation heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnableChanged;
        }

        public IHeaterPresentation AddHeater(float x, float y, float temperature)
        {
            var heater = new HeaterPresentation(_logic.AddHeater(x, y, temperature));
            SubscribeToHeater(heater);
            _heaters.Add(heater);
            return heater;
        }

        public bool ContainsHeater(Guid id)
        {
            return _heaters.Find(heater => heater.Id == id) != null;
        }

        public IHeaterPresentation? GetHeater(Guid id)
        {
            return _heaters.Find(heater => heater.Id == id);
        }

        public void RemoveHeater(Guid id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater != null)
            {
                UnsubscribeFromHeater(heater);
                _heaters.Remove(heater);
            }
            _logic.RemoveHeater(id);
        }

        public void ClearHeaters()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeFromHeater(heater);
            }
            _heaters.Clear();
            _logic.ClearHeaters();
        }

        private void SubscribeToHeatSensor(IHeatSensorPresentation sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensorPresentation sensor)
        {
            sensor.PositionChanged -= GetPositionChanged;
            sensor.TemperatureChanged -= GetTemperatureChanged;
        }

        public IHeatSensorPresentation AddHeatSensor(float x, float y)
        {
            var sensor = new HeatSensorPresentation(_logic.AddHeatSensor(x, y));
            SubscribeToHeatSensor(sensor);
            _heatSensors.Add(sensor);
            return sensor;
        }

        public bool ContainsHeatSensor(Guid id)
        {
            return _heatSensors.Find(sensor => sensor.Id == id) != null;
        }

        public IHeatSensorPresentation? GetHeatSensor(Guid id)
        {
            return _heatSensors.Find(sensor => sensor.Id == id);
        }

        public void RemoveHeatSensor(Guid id)
        {
            var sensor = _heatSensors.Find(sensor => sensor.Id == id);
            if (sensor != null)
            {
                UnsubscribeFromHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
            _logic.RemoveHeatSensor(id);
        }

        public void ClearHeatSensors()
        {
            foreach (var sensor in _heatSensors)
            {
                UnsubscribeFromHeatSensor(sensor);
            }
            _heatSensors.Clear();
            _logic.ClearHeatSensors();
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeFromHeater(heater);
                heater.Dispose();
            }
            _heaters.Clear();
            foreach (var sensor in _heatSensors)
            {
                UnsubscribeFromHeatSensor(sensor);
                sensor.Dispose();
            }
            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
