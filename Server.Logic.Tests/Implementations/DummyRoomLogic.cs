using System.Diagnostics;
using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic.Tests
{
    internal class DummyRoomLogic : IRoomLogic
    {
        private readonly IRoomData _data;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        public Guid Id => _data.Id;

        private readonly List<DummyHeaterLogic> _heaters = [];

        private readonly object _heatersLock = new();
        public IReadOnlyCollection<IHeaterLogic> Heaters
        {
            get
            {
                lock (_heatersLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<DummyHeatSensorLogic> _heatSensors = [];

        private readonly object _heatSensorsLock = new();
        public IReadOnlyCollection<IHeatSensorLogic> HeatSensors
        {
            get
            {
                lock (_heatSensorsLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        public string Name => _data.Name;
        public float Width => _data.Width;
        public float Height => _data.Height;

        private float _roomTemperature = 0.0f;

        private Task? _task;
        private CancellationTokenSource? _cts;

        private const float TemperatureDecayFactor = 0.5f;

        private readonly object _roomTemperatureLock = new();

        public DummyRoomLogic(IRoomData data)
        {
            _data = data;

            foreach (var heaterData in _data.Heaters)
            {
                var heater = new DummyHeaterLogic(heaterData);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
            }

            foreach (var sensorData in _data.HeatSensors)
            {
                var sensor = new DummyHeatSensorLogic(sensorData);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
            }

            _task = null;
            _cts = null;
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(source, lastTemperature, newTemperature);
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(source, lastPosition, newPosition);
        }

        private void GetEnableChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, newEnable);
        }

        private float GetTemperatureAtPosition(IPositionLogic position)
        {
            if (HeatSensors.Count == 0 || position.X > Width || position.X < 0f || position.Y > Height || position.Y < 0f) return 0f;

            var heatersTemp = 0f;
            lock (_heatersLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                if (onHeaters.Count != 0)
                {
                    heatersTemp += (from heater in onHeaters
                                    let dist = IPositionLogic.Distance(position, heater.Position)
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
                    var dist = IPositionLogic.Distance(position, sensor.Position);
                    var weight = dist > 0f ? 1f / dist : float.MaxValue;
                    if (Math.Abs(weight - float.MaxValue) < 1e-10f) return sensor.Temperature;
                    sensorsSum += sensor.Temperature * weight;
                    sensorsDistSum += weight;
                }
            }

            var sensorsTemp = sensorsDistSum > 0f ? sensorsSum / sensorsDistSum : 0f;

            var temp = MathF.Max(sensorsTemp, heatersTemp);

            lock (_roomTemperatureLock)
            {
                return temp > 0 ? temp : _roomTemperature;
            }
        }


        private void SubscribeToHeater(IHeaterLogic heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(IHeaterLogic heater)
        {
            heater.PositionChanged -= GetPositionChanged;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.EnableChanged -= GetEnableChanged;
        }

        public IHeaterLogic AddHeater(float x, float y, float temperature)
        {
            if (x > Width || x < 0f)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y > Height || y < 0f)
                throw new ArgumentOutOfRangeException(nameof(y));

            var heater = new DummyHeaterLogic(_data.AddHeater(x, y, temperature));
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

        public IHeaterLogic? GetHeater(Guid id)
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
            _data.RemoveHeater(id);
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

        private void SubscribeToHeatSensor(IHeatSensorLogic sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(IHeatSensorLogic sensor)
        {
            sensor.PositionChanged -= GetPositionChanged;
            sensor.TemperatureChanged -= GetTemperatureChanged;
        }

        public IHeatSensorLogic AddHeatSensor(float x, float y)
        {
            if (x > Width || x < 0f)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y > Height || y < 0f)
                throw new ArgumentOutOfRangeException(nameof(y));

            var sensor = new DummyHeatSensorLogic(_data.AddHeatSensor(x, y));
            sensor.SetTemperature(GetTemperatureAtPosition(sensor.Position));
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

        public IHeatSensorLogic? GetHeatSensor(Guid id)
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
            _data.RemoveHeatSensor(id);
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

        internal void StartSimulation()
        {
            if (_task != null && !_task.IsCompleted) return;
            _cts = new CancellationTokenSource();
            _task = Task.Run(() => TaskMethod(_cts.Token), _cts.Token);
        }

        private async Task TaskMethod(CancellationToken token)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            while (!token.IsCancellationRequested)
            {
                UpdateTemperature((float)stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
                try
                {
                    await Task.Delay(5, token);
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, break loop
                    break;
                }
            }
        }

        private void UpdateTemperature(float deltaTime)
        {
            lock (_heatersLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                lock (_roomTemperatureLock)
                {
                    _roomTemperature = MathF.Max(_roomTemperature - TemperatureDecayFactor * deltaTime, 0f);
                    if (onHeaters.Count != 0)
                    {
                        foreach (var heater in onHeaters)
                        {
                            _roomTemperature += (heater.Temperature * TemperatureDecayFactor * deltaTime) / (Width * Height);
                        }
                        _roomTemperature = MathF.Min(_roomTemperature, onHeaters.Max(heater => heater.Temperature));
                    }
                }

                lock (_heatSensorsLock)
                {
                    foreach (var sensor in _heatSensors)
                    {
                        var temperature = MathF.Max(sensor.Temperature - TemperatureDecayFactor * deltaTime, 0f);

                        if (onHeaters.Count == 0)
                        {
                            sensor.SetTemperature(temperature);
                            continue;
                        }
                        foreach (var heater in onHeaters)
                        {
                            var dist = IPositionLogic.Distance(sensor.Position, heater.Position);
                            var tempDiff = MathF.Min(heater.Temperature,
                                heater.Temperature * MathF.Exp(-TemperatureDecayFactor * dist)) * deltaTime;
                            temperature += tempDiff - TemperatureDecayFactor * deltaTime;
                        }
                        temperature = MathF.Min(temperature, onHeaters.Max(heater => heater.Temperature));
                        sensor.SetTemperature(temperature);
                    }
                }
            }
        }

        internal void EndSimulation()
        {
            if (_cts == null || _task == null || _task.IsCompleted)
                return;

            _cts.Cancel();
            try
            {
                _task.Wait();
            }
            catch (AggregateException ae) when (ae.InnerExceptions.All(e => e is TaskCanceledException))
            {
                // Ignore task cancellation exceptions
            }
            finally
            {
                _cts.Dispose();
            }
        }

        public void Dispose()
        {
            EndSimulation();
            ClearHeaters();
            ClearHeatSensors();
            GC.SuppressFinalize(this);
        }
    }
}