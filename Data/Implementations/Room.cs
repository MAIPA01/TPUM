using System.Runtime.CompilerServices;

namespace TPUM.Data
{
    internal class Room(long id, float width, float height) : IRoom
    {
        private readonly List<IHeater> _heaters = [];

        public IReadOnlyCollection<IHeater> Heaters
        {
            get
            {
                lock (_heatersLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<IHeatSensor> _heatSensors = [];

        public IReadOnlyCollection<IHeatSensor> HeatSensors
        {
            get
            {
                lock (_heatSensorsLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public long Id { get; } = id;
        public float Width { get; } = width;
        public float Height { get; } = height;

        public float RoomTemperature { get; set; }

        public float AvgTemperature
        {
            get
            {
                lock (_heatSensorsLock)
                {
                    return _heatSensors.Count == 0 ? 0f : _heatSensors.Average(heatSensor => heatSensor.Temperature);
                }
            }
        }

        private const float TemperatureDecayFactor = 0.1f;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _heatersLock = new();
        private readonly object _heatSensorsLock = new();

        private void GetTemperatureChanged(object? source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(source, args);
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(source, args);
        }

        private void GetEnableChanged(object? source, EnableChangedEventArgs args)
        {
            EnableChanged?.Invoke(source, args);
        }

        public float GetTemperatureAtPosition(float x, float y)
        {
            if (x > Width || x < 0f || y > Height || y < 0f || HeatSensors.Count == 0) return 0f;

            var pos = new Position(x, y);
            var heatersTemp = 0f;
            lock (_heatersLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                if (onHeaters.Count != 0)
                {
                    heatersTemp += (from heater in onHeaters
                        let dist = IPosition.Distance(pos, heater.Position)
                        select MathF.Min(heater.Temperature,
                            heater.Temperature * MathF.Exp(-TemperatureDecayFactor * dist))).Sum();
                    heatersTemp = MathF.Min(heatersTemp, onHeaters.Max(heater => heater.Temperature));
                }
            }

            var sensorsSum = 0f;
            var sensorsDistSum = 0f;
            lock (_heatSensorsLock)
            {
                foreach (var sensor in HeatSensors)
                {
                    var dist = IPosition.Distance(pos, sensor.Position);
                    var weight = dist > 0f ? 1f / dist : float.MaxValue;
                    if (Math.Abs(weight - float.MaxValue) < 1e-10f) return sensor.Temperature;
                    sensorsSum += sensor.Temperature * weight;
                    sensorsDistSum += weight;
                }
            }

            var sensorsTemp = sensorsDistSum > 0f ? sensorsSum / sensorsDistSum : 0f;

            var temp = MathF.Max(sensorsTemp, heatersTemp);

            return temp > 0 ? temp : RoomTemperature;
        }

        private void SubscribeToHeater(IHeater heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(IHeater heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnableChanged;
        }

        public IHeater AddHeater(float x, float y, float temperature)
        {
            if (x > Width || x < 0f)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y > Height || y < 0f)
                throw new ArgumentOutOfRangeException(nameof(y));

            var heater = new Heater(new Random().NextInt64(), x, y, temperature);
            SubscribeToHeater(heater);
            lock (_heatersLock)
            {
                _heaters.Add(heater);
            }
            OnTemperatureChanged(AvgTemperature);

            return heater;
        }

        public void RemoveHeater(long id)
        {
            lock (_heatersLock)
            {
                var heater = _heaters.Find(heater => heater.Id == id);
                if (heater == null) return;
                UnsubscribeFromHeater(heater);
                _heaters.Remove(heater);
            }
            OnTemperatureChanged(AvgTemperature);
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
            OnTemperatureChanged(AvgTemperature);
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
            if (x > Width || x < 0f)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y > Height || y < 0f)
                throw new ArgumentOutOfRangeException(nameof(y));

            var sensor = new HeatSensor(new Random().NextInt64(), x, y);
            SubscribeToHeatSensor(sensor);
            sensor.Temperature = GetTemperatureAtPosition(sensor.Position.X, sensor.Position.Y);
            lock (_heatSensors)
            {
                _heatSensors.Add(sensor);
            }
            OnTemperatureChanged(AvgTemperature);

            return sensor;
        }

        public void RemoveHeatSensor(long id)
        {
            lock (_heatSensorsLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == id);
                if (sensor == null) return;
                UnsubscribeFromHeatSensor(sensor);
                _heatSensors.Remove(sensor);
            }
            OnTemperatureChanged(AvgTemperature);
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
            OnTemperatureChanged(AvgTemperature);
        }

        public void Dispose()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeFromHeater(heater);
                heater.Dispose();
            }

            _heaters.Clear();
            foreach (var heatSensor in _heatSensors)
            {
                UnsubscribeFromHeatSensor(heatSensor);
                heatSensor.Dispose();
            }

            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }

        protected void OnTemperatureChanged(float lastTemperature)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(lastTemperature, AvgTemperature));
        }
    }
}
