using TPUM.Client.Presentation.ViewModel.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TPUM.Client.Presentation.Model;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class Room : IRoom
    {
        private readonly IRoomModel _model;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangeEventHandler? EnableChanged;

        public event HeaterAddedEventHandler? HeaterAdded;
        public event HeaterRemovedEventHandler? HeaterRemoved;
        public event HeatSensorAddedEventHandler? HeatSensorAdded;
        public event HeatSensorRemovedEventHandler? HeatSensorRemoved;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly object _roomLock = new();

        public Guid Id => _model.Id;
        public string Name => _model.Name;
        public float Width => _model.Width;
        public float Height => _model.Height;

        public float AvgTemperature { get; private set; } = 0f;

        private readonly ObservableCollection<IHeater> _heaters = [];
        private readonly ReadOnlyObservableCollection<IHeater> _readOnlyHeaters;
        public ReadOnlyObservableCollection<IHeater> Heaters
        {
            get
            {
                lock (_roomLock)
                {
                    return _readOnlyHeaters;
                }
            }
        }


        private readonly ObservableCollection<IHeatSensor> _heatSensors = [];
        private readonly ReadOnlyObservableCollection<IHeatSensor> _readOnlyHeatSensors;
        public ReadOnlyObservableCollection<IHeatSensor> HeatSensors
        {
            get
            {
                lock (_roomLock)
                {
                    return _readOnlyHeatSensors;
                }
            }
        }

        public Room(IRoomModel model)
        {
            _model = model;

            _readOnlyHeaters = new ReadOnlyObservableCollection<IHeater>(_heaters);
            foreach (var heater in _model.Heaters)
            {
                var roomHeater = new Heater(heater);
                SubscribeToHeater(roomHeater);
                _heaters.Add(roomHeater);
            }

            _readOnlyHeatSensors = new ReadOnlyObservableCollection<IHeatSensor>(_heatSensors);
            foreach (var heatSensor in _model.HeatSensors)
            {
                var roomSensor = new HeatSensor(heatSensor);
                SubscribeToHeatSensor(roomSensor);
                _heatSensors.Add(roomSensor);
            }

            _model.HeaterAdded += GetHeaterAdded;
            _model.HeaterRemoved += GetHeaterRemoved;
            _model.HeatSensorAdded += GetHeatSensorAdded;
            _model.HeatSensorRemoved += GetHeatSensorRemoved;
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
            
            UpdateAvgTemperature();
            OnPropertyChange(nameof(Heaters));
            OnPropertyChange(nameof(HeatSensors));
            OnPropertyChange(nameof(AvgTemperature));
        }

        private void GetPositionChanged(object? source, IPosition lastPosition, IPosition newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
            OnPropertyChange(nameof(Heaters));
            OnPropertyChange(nameof(HeatSensors));
        }

        private void GetEnabledChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
            OnPropertyChange(nameof(Heaters));
        }

        private void GetHeaterAdded(object? source, IHeaterModel heaterModel)
        {
            lock (_roomLock)
            {
                var heater = new Heater(heaterModel);
                SubscribeToHeater(heater);
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _heaters.Add(heater);
                    OnPropertyChange(nameof(Heaters));
                    UpdateAvgTemperature();
                });

                HeaterAdded?.Invoke(this, heater);
            }
        }

        private void GetHeaterRemoved(object? source, Guid heaterId)
        {
            lock (_roomLock)
            {
                if (_heaters.Any(heater => heater.Id == heaterId))
                {
                    var heater = _heaters.First(heater => heater.Id == heaterId);
                    UnsubscribeFromHeater(heater);
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _heaters.Remove(heater);
                        OnPropertyChange(nameof(Heaters));
                        UpdateAvgTemperature();
                    });
                }
                HeaterRemoved?.Invoke(this, heaterId);
            }
        }

        private void GetHeatSensorAdded(object? source, IHeatSensorModel sensorModel)
        {
            lock (_roomLock)
            {
                var sensor = new HeatSensor(sensorModel);
                SubscribeToHeatSensor(sensor);
                _ = Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _heatSensors.Add(sensor);
                    OnPropertyChange(nameof(HeatSensors));
                    UpdateAvgTemperature();
                });

                HeatSensorAdded?.Invoke(this, sensor);
            }
        }

        private void GetHeatSensorRemoved(object? source, Guid sensorId)
        {
            lock (_roomLock)
            {
                if (_heatSensors.Any(sensor => sensor.Id == sensorId))
                {
                    var sensor = _heatSensors.First(sensor => sensor.Id == sensorId);
                    UnsubscribeFromHeatSensor(sensor);
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _heatSensors.Remove(sensor);
                        OnPropertyChange(nameof(HeatSensors));
                        UpdateAvgTemperature();
                    });
                }
                HeatSensorRemoved?.Invoke(this, sensorId);
            }
        }

        private void UpdateAvgTemperature()
        {
            lock (_roomLock)
            {
                AvgTemperature = _heatSensors.Count > 0 ? _heatSensors.Average(sensor => sensor.Temperature) : 0f;
            }
            OnPropertyChange(nameof(AvgTemperature));
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

        public void AddHeater(float x, float y, float temperature)
        {
            _model.AddHeater(x, y, temperature);
        }

        public void RemoveHeater(Guid id)
        {
            _model.RemoveHeater(id);
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

        public void AddHeatSensor(float x, float y)
        {
            _model.AddHeatSensor(x, y);
        }

        public void RemoveHeatSensor(Guid id)
        {
            _model.RemoveHeatSensor(id);
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeFromHeater(heater);
            }
            _heaters.Clear();

            foreach (var sensor in _heatSensors)
            {
                UnsubscribeFromHeatSensor(sensor);
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