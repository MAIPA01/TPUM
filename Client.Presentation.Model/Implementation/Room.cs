﻿using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class Room : IRoom
    {
        private readonly IRoomLogic _logic;
        private readonly ModelApi _modelApi;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public Guid Id => _logic.Id;
        public string Name { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float AvgTemperature { get; private set; }

        private readonly List<IHeater> _heaters = [];
        public IReadOnlyCollection<IHeater> Heaters => _heaters.AsReadOnly();

        private readonly List<IHeatSensor> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensor> HeatSensors => _heatSensors.AsReadOnly();

        public Room(IRoomLogic logic, ModelApi api)
        {
            _logic = logic;
            _modelApi = api;
            Name = "";
            UpdateData(_logic.Name, _logic.Width, _logic.Height, _logic.AvgTemperature);

            foreach (var heater in logic.Heaters)
            {
                var modelHeater = new Heater(heater, _modelApi);
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

        internal void UpdateData(string name, float width, float height, float avgTemperature)
        {
            Name = name;
            Width = width;
            Height = height;
            AvgTemperature = avgTemperature;
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

        public IHeatSensor? AddHeatSensor(float x, float y)
        {
            var modelSensor = _modelApi.AddHeatSensor(Id, x, y);
            if (modelSensor == null) return null;

            SubscribeToHeatSensor(modelSensor);
            _heatSensors.Add(modelSensor);
            return modelSensor;
        }

        internal IHeatSensor AddHeatSensor(IHeatSensor sensor)
        {
            SubscribeToHeatSensor(sensor);
            _heatSensors.Add(sensor);
            return sensor;
        }

        internal IHeatSensor? GetHeatSensor(Guid id)
        {
            var sensor = _heatSensors.Find(s => s.Id == id);
            return sensor;
        }

        internal void RemoveHeatSensorFromList(Guid id)
        {
            var sensor = _heatSensors.Find(s => s.Id == id);
            if (sensor != null)
            {
                UnsubscribeToHeatSensor(sensor);
                sensor.Dispose();
                _heatSensors.Remove(sensor);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            RemoveHeatSensorFromList(id);
            _modelApi.RemoveHeatSensor(Id, id);
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

        public IHeater? AddHeater(float x, float y, float temperature)
        {
            var modelHeater = _modelApi.AddHeater(Id, x, y, temperature);
            if (modelHeater == null) return null;

            SubscribeToHeater(modelHeater);
            _heaters.Add(modelHeater);
            return modelHeater;
        }

        internal IHeater AddHeater(IHeater heater)
        {
            SubscribeToHeater(heater);
            _heaters.Add(heater);
            return heater;
        }

        internal IHeater? GetHeater(Guid id)
        {
            var heater = _heaters.Find(h => h.Id == id);
            return heater;
        }

        internal void RemoveHeaterFromList(Guid id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater != null)
            {
                UnsubscribeToHeater(heater);
                heater.Dispose();
                _heaters.Remove(heater);
            }
        }

        public void RemoveHeater(Guid id)
        {
            RemoveHeaterFromList(id);
            _modelApi.RemoveHeater(Id, id);
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