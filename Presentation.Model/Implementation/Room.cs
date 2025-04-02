using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TPUM.Presentation.Model
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

        private readonly ObservableCollection<IHeater> _heaters = [];
        public ReadOnlyObservableCollection<IHeater> Heaters { get; }

        private readonly ObservableCollection<IHeatSensor> _heatSensors = [];
        public ReadOnlyObservableCollection<IHeatSensor> HeatSensors { get; }

        public ICommand ClearHeatSensorsCommand { get; }
        public ICommand ClearHeatersCommand { get; }

        public Room(string name, Logic.IRoom room)
        {
            _room = room;
            Name = name;

            Heaters = new ReadOnlyObservableCollection<IHeater>(_heaters);
            foreach (var heater in room.Heaters)
            {
                var modelHeater = new Heater(heater);
                SubscribeToHeater(modelHeater);
                _heaters.Add(modelHeater);
            }

            HeatSensors = new ReadOnlyObservableCollection<IHeatSensor>(_heatSensors);
            foreach (var sensor in room.HeatSensors)
            {
                var modelSensor = new HeatSensor(sensor);
                SubscribeToHeatSensor(modelSensor);
                _heatSensors.Add(modelSensor);
            }

            ClearHeatSensorsCommand = new CustomCommand(ClearHeatSensors, _ => true);
            ClearHeatersCommand = new CustomCommand(ClearHeaters, _ => true);
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

        public void AddHeatSensor(float x, float y)
        {
            var modelSensor = new HeatSensor(_room.AddHeatSensor(x, y));
            SubscribeToHeatSensor(modelSensor);
            _heatSensors.Add(modelSensor);
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.ToList().Find(sensor => sensor.Id == id);
            if (sensor == null) return;
            UnsubscribeToHeatSensor(sensor);
            _heatSensors.Remove(sensor);
            _room.RemoveHeatSensor(id);
        }

        private void ClearHeatSensors(object? obj)
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

        public void AddHeater(float x, float y, float temperature)
        {
            var modelHeater = new Heater(_room.AddHeater(x, y, temperature));
            SubscribeToHeater(modelHeater);
            _heaters.Add(modelHeater);
        }

        public void RemoveHeater(long id)
        {
            var heater = _heaters.ToList().Find(heater => heater.Id == id);
            if (heater == null) return;
            UnsubscribeToHeater(heater);
            _heaters.Remove(heater);
            _room.RemoveHeater(id);
        }

        private void ClearHeaters(object? obj)
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
            _room.Dispose();
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
