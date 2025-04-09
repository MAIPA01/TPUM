﻿using TPUM.Server.Data.Events;

namespace TPUM.Server.Data
{
    internal class RoomData : IRoomData
    {
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        public Guid Id { get; }
        public string Name { get; }
        public float Width { get; }
        public float Height { get; }

        private readonly object _heatersLock = new();
        private readonly List<HeaterData> _heaters = [];
        public IReadOnlyCollection<IHeaterData> Heaters
        { 
            get
            {
                lock (_heatersLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly object _heatSensorsLock = new();
        private readonly List<HeatSensorData> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorData> HeatSensors
        {
            get
            {
                lock (_heatSensorsLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public RoomData(Guid id, string name, float width, float height)
        {
            Id = id;
            Name = name;
            Width = width;
            Height = height;
        }

        private void GetPositionChange(object? source, IPositionData lastPosition, IPositionData newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
        }

        private void GetTemperatureChange(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(source, lastTemperature, newTemperature);
        }

        private void GetEnableChange(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
        }

        private void SubscribeToHeater(IHeaterData heater)
        {
            heater.PositionChanged += GetPositionChange;
            heater.TemperatureChanged += GetTemperatureChange;
            heater.EnableChanged += GetEnableChange;
        }

        private void UnsubscribeFromHeater(IHeaterData heater)
        {
            heater.PositionChanged -= GetPositionChange;
            heater.TemperatureChanged -= GetTemperatureChange;
            heater.EnableChanged -= GetEnableChange;
        }

        public IHeaterData AddHeater(float x, float y, float temperature)
        {
            var heater = new HeaterData(Guid.NewGuid(), new PositionData(x, y), temperature);
            SubscribeToHeater(heater);
            lock (_heatersLock)
            {
                _heaters.Add(heater);
            }
            return heater;
        }

        public bool ContainsHeater(Guid id)
        {
            lock (_heatersLock)
            {
                return _heaters.Find(heater => heater.Id == id) != null;
            }
        }

        public IHeaterData? GetHeater(Guid id)
        {
            lock (_heatersLock)
            {
                return _heaters.Find(heater => heater.Id == id);
            }
        }

        public void RemoveHeater(Guid id)
        {
            lock (_heatersLock)
            {
                var heater = _heaters.Find(heater => heater.Id == id);
                if (heater == null) return;
                UnsubscribeFromHeater(heater);
                _heaters.Remove(heater);
            }
        }

        public void ClearHeaters()
        {
            lock (_heatersLock)
            {
                foreach (var heater in _heaters)
                {
                    UnsubscribeFromHeater(heater);
                }
                _heaters.Clear();
            }
        }

        private void SubscribeToHeatSensor(IHeatSensorData sensor)
        {
            sensor.PositionChanged += GetPositionChange;
            sensor.TemperatureChanged += GetTemperatureChange;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensorData sensor)
        {
            sensor.PositionChanged -= GetPositionChange;
            sensor.TemperatureChanged -= GetTemperatureChange;
        }

        public IHeatSensorData AddHeatSensor(float x, float y)
        {
            var sensor = new HeatSensorData(Guid.NewGuid(), new PositionData(x, y));
            SubscribeToHeatSensor(sensor);
            lock (_heatSensorsLock)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public bool ContainsHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == id) != null;
            }
        }

        public IHeatSensorData? GetHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                return _heatSensors.Find(sensor => sensor.Id == id);
            }
        }

        public void RemoveHeatSensor(Guid id)
        {
            lock (_heatSensorsLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == id);
                if (sensor == null) return;
                UnsubscribeFromHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
        }

        public void ClearHeatSensors()
        {
            lock (_heatSensorsLock)
            {
                foreach (var sensor in _heatSensors)
                {
                    UnsubscribeFromHeatSensor(sensor);
                }
                _heatSensors.Clear();
            }
        }
        public void Dispose()
        {
            ClearHeaters();
            ClearHeatSensors();
            GC.SuppressFinalize(this);
        }
    }
}
