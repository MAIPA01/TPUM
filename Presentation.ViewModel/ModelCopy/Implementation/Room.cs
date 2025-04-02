using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TPUM.Presentation.ViewModel
{
    internal class Room : IRoom
    {
        private readonly Model.IRoom _room;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangeEventHandler? EnableChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public long Id => _room.Id;

        public string Name 
        {
            get => _room.Name;
            set
            {
                if (_room.Name == value) return;
                _room.Name = value;
                OnPropertyChange(nameof(Name));
            }
        }

        public float Width => _room.Width;

        public float Height => _room.Height;

        public float AvgTemperature => _room.AvgTemperature;

        private readonly ObservableCollection<IHeater> _heaters = [];
        public ReadOnlyObservableCollection<IHeater> Heaters { get; }

        private readonly ObservableCollection<IHeatSensor> _heatSensors = [];
        public ReadOnlyObservableCollection<IHeatSensor> HeatSensors { get; }

        public ICommand ClearHeatSensorsCommand => new CustomCommand(_room.ClearHeatSensorsCommand);

        public ICommand ClearHeatersCommand => new CustomCommand(_room.ClearHeatersCommand);

        public Room(Model.IRoom room)
        {
            _room = room;
            
            Heaters = new ReadOnlyObservableCollection<IHeater>(_heaters);
            foreach (var heater in _room.Heaters)
            {
                var roomHeater = new Heater(heater);
                SubscribeToHeater(roomHeater);
                _heaters.Add(roomHeater);
            }

            HeatSensors = new ReadOnlyObservableCollection<IHeatSensor>(_heatSensors);
            foreach (var heatSensor in _room.HeatSensors)
            {
                var roomSensor = new HeatSensor(heatSensor);
                SubscribeToHeatSensor(roomSensor);
                _heatSensors.Add(roomSensor);
            }
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(source, args);
            OnPropertyChange(nameof(Heaters));
            OnPropertyChange(nameof(HeatSensors));
        }

        private void GetTemperatureChanged(object? source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(source, args);
            OnPropertyChange(nameof(Heaters));
            OnPropertyChange(nameof(HeatSensors));
            OnPropertyChange(nameof(AvgTemperature));
        }

        private void GetEnabledChanged(object? source, EnableChangeEventArgs args)
        {
            EnableChanged?.Invoke(source, args);
            OnPropertyChange(nameof(Heaters));
        }

        private void SubscribeToHeater(IHeater heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnabledChanged;
        }

        private void UnsubscribeFromHeater(IHeater heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnabledChanged;
        }

        public IHeater AddHeater(float x, float y, float temperature)
        {
            var heater = new Heater(_room.AddHeater(x, y, temperature));
            SubscribeToHeater(heater);
            _heaters.Add(heater);
            return heater;
        }

        public void RemoveHeater(long id)
        {
            // TODO: sprawdzić to czy jest poprawnie niżej
            var heater = _heaters.First(h => h.Id == id);
            UnsubscribeFromHeater(heater);
            _heaters.Remove(heater);
            _room.RemoveHeater(id);
        }

        private void SubscribeToHeatSensor(IHeatSensor sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensor sensor)
        {
            sensor.PositionChanged -= GetPositionChanged;
            sensor.TemperatureChanged -= GetTemperatureChanged;
        }

        public IHeatSensor AddHeatSensor(float x, float y)
        {
            var sensor = new HeatSensor(_room.AddHeatSensor(x, y));
            SubscribeToHeatSensor(sensor);
            _heatSensors.Add(sensor);
            return sensor;
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.First(sensor => sensor.Id == id);
            UnsubscribeFromHeatSensor(sensor);
            _heatSensors.Remove(sensor);
            _room.RemoveHeatSensor(id);
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

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
