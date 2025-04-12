using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model.Tests
{
    internal class DummyRoomModel : IRoomModel
    {
        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;

        private readonly IRoomLogic _logic;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public Guid Id => _logic.Id;
        public string Name { get; }
        public float Width { get; }
        public float Height { get; }

        private readonly List<IHeaterModel> _heaters = [];
        public IReadOnlyCollection<IHeaterModel> Heaters => _heaters.AsReadOnly();

        private readonly List<IHeatSensorModel> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorModel> HeatSensors => _heatSensors.AsReadOnly();

        public DummyRoomModel(IRoomLogic logic)
        {
            _logic = logic;
            Name = "";

            foreach (var heater in logic.Heaters)
            {
                var modelHeater = new DummyHeater(heater);
                _heaters.Add(modelHeater);
            }

            foreach (var sensor in logic.HeatSensors)
            {
                var modelSensor = new DummyHeatSensorModel(sensor);
                _heatSensors.Add(modelSensor);
            }
        }

        public void AddHeatSensor(float x, float y)
        {
            var sensor = new DummyHeatSensorModel(new TestHeatSensorLogic(x, y));
            _heatSensors.Add(sensor);
        }

        public bool ContainsHeatSensor(Guid sensorId)
        {
            return _heatSensors.Any(sensor => sensor.Id == sensorId);
        }

        public IHeatSensorModel? GetHeatSensor(Guid id)
        {
            return _heatSensors.Find(s => s.Id == id);
        }

        public void RemoveHeatSensor(Guid id)
        {
            var sensor = _heatSensors.Find(s => s.Id == id);
            if (sensor == null) return;
            sensor.Dispose();
            _heatSensors.Remove(sensor);
        }

        public void AddHeater(float x, float y, float temperature)
        {
            var heater = new DummyHeater(new TestHeaterLogic(x, y, temperature));
            _heaters.Add(heater);
        }

        public bool ContainsHeater(Guid heaterId)
        {
            return _heaters.Any(heater => heater.Id == heaterId);
        }

        public IHeaterModel? GetHeater(Guid id)
        {
            var heater = _heaters.Find(h => h.Id == id);
            return heater;
        }

        public void RemoveHeater(Guid id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater == null) return;
            heater.Dispose();
            _heaters.Remove(heater);
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                heater.Dispose();
            }
            _heaters.Clear();
            foreach (var sensor in _heatSensors)
            {
                sensor.Dispose();
            }
            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }
    }
}