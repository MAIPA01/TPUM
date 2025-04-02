using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TPUM.Logic;

namespace TPUM.Presentation.Model
{
    internal class ModelRoom : IModelRoom
    {
        private readonly IRoom _room;

        public long Id => _room.Id;
        public string Name { get; set; }
        public float Width => _room.Width;
        public float Height => _room.Height;
        public float AvgTemperature => _room.AvgTemperature;

        private readonly ObservableCollection<IModelHeater> _heaters = [];
        public ReadOnlyObservableCollection<IModelHeater> Heaters { get; }

        private readonly ObservableCollection<IModelHeatSensor> _heatSensors = [];
        public ReadOnlyObservableCollection<IModelHeatSensor> HeatSensors { get; }

        public ICommand ClearHeatSensorsCommand { get; }
        public ICommand ClearHeatersCommand { get; }

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangeEventHandler? EnableChange;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ModelRoom(string name, IRoom room)
        {
            _room = room;
            Name = name;

            Heaters = new ReadOnlyObservableCollection<IModelHeater>(_heaters);
            foreach (var heater in room.Heaters)
            {
                var modelHeater = new ModelHeater(heater);
                modelHeater.TemperatureChanged += GetTemperatureChanged;
                modelHeater.PositionChanged += GetPositionChanged;
                modelHeater.EnableChange += GetEnabledChanged;
                _heaters.Add(modelHeater);
            }

            HeatSensors = new ReadOnlyObservableCollection<IModelHeatSensor>(_heatSensors);
            foreach (var sensor in room.HeatSensors)
            {
                var modelSensor = new ModelHeatSensor(sensor);
                modelSensor.TemperatureChanged += GetTemperatureChanged;
                modelSensor.PositionChanged += GetPositionChanged;
                _heatSensors.Add(modelSensor);
            }

            ClearHeatSensorsCommand = new CustomCommand(ClearHeatSensors, _ => true);
            ClearHeatersCommand = new CustomCommand(ClearHeaters, _ => true);
        }

        private void GetTemperatureChanged(object source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(source, args);
            OnPropertyChange(nameof(AvgTemperature));
            OnPropertyChange(nameof(Heaters));
            OnPropertyChange(nameof(HeatSensors));
        }

        private void GetPositionChanged(object source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(source, args);
            OnPropertyChange(nameof(Heaters));
            OnPropertyChange(nameof(HeatSensors));
        }

        private void GetEnabledChanged(object source, EnableChangeEventArgs args)
        {
            EnableChange?.Invoke(source, args);
            OnPropertyChange(nameof(Heaters));
        }

        public void AddHeatSensor(float x, float y)
        {
            var modelSensor = new ModelHeatSensor(_room.AddHeatSensor(x, y));
            modelSensor.TemperatureChanged += GetTemperatureChanged;
            modelSensor.PositionChanged += GetPositionChanged;
            _heatSensors.Add(modelSensor);
            OnPropertyChange(nameof(HeatSensors));
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.ToList().Find(sensor => sensor.Id == id);
            if (sensor == null) return;
            sensor.TemperatureChanged -= GetTemperatureChanged;
            sensor.PositionChanged -= GetPositionChanged;
            _heatSensors.Remove(sensor);
            OnPropertyChange(nameof(HeatSensors));
        }

        private void ClearHeatSensors(object? obj)
        {
            foreach (var sensor in _heatSensors)
            {
                sensor.TemperatureChanged -= GetTemperatureChanged;
                sensor.PositionChanged -= GetPositionChanged;
            }
            _heatSensors.Clear();
            _room.ClearHeatSensors();
            OnPropertyChange(nameof(HeatSensors));
        }

        public void AddHeater(float x, float y, float temperature)
        {
            var modelHeater = new ModelHeater(_room.AddHeater(x, y, temperature));
            modelHeater.TemperatureChanged += GetTemperatureChanged;
            modelHeater.PositionChanged += GetPositionChanged;
            modelHeater.EnableChange += GetEnabledChanged;
            _heaters.Add(modelHeater);
            OnPropertyChange(nameof(Heaters));
        }

        public void RemoveHeater(long id)
        {
            var heater = _heaters.ToList().Find(heater => heater.Id == id);
            if (heater == null) return;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.PositionChanged -= GetPositionChanged;
            heater.EnableChange -= GetEnabledChanged;
            _heaters.Remove(heater);
            OnPropertyChange(nameof(Heaters));
        }

        private void ClearHeaters(object? obj)
        {
            foreach (var heater in _heaters)
            {
                heater.TemperatureChanged -= GetTemperatureChanged;
                heater.PositionChanged -= GetPositionChanged;
                heater.EnableChange -= GetEnabledChanged;
            }
            _heaters.Clear();
            _room.ClearHeaters();
            OnPropertyChange(nameof(Heaters));
        }

        public void Dispose()
        {
            _room.Dispose();
            foreach (var heater in _heaters)
            {
                heater.TemperatureChanged -= GetTemperatureChanged;
                heater.PositionChanged -= GetPositionChanged;
                heater.EnableChange -= GetEnabledChanged;
                heater.Dispose();
            }
            _heaters.Clear();
            foreach (var sensor in _heatSensors)
            {
                sensor.TemperatureChanged -= GetTemperatureChanged;
                sensor.PositionChanged -= GetPositionChanged;
                sensor.Dispose();
            }
            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }

        private void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
