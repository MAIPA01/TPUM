using System.Diagnostics;
using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    internal class RoomLogic : IRoomLogic
    {
        private readonly IRoomData _data;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _roomLock = new();

        public Guid Id => _data.Id;
        public string Name => _data.Name;
        public float Width => _data.Width;
        public float Height => _data.Height;

        private readonly List<HeaterLogic> _heaters = [];
        public IReadOnlyCollection<IHeaterLogic> Heaters
        {
            get
            {
                lock (_roomLock)
                {
                    return _heaters.AsReadOnly();
                }
            }
        }

        private readonly List<HeatSensorLogic> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensorLogic> HeatSensors
        {
            get
            {
                lock (_roomLock)
                {
                    return _heatSensors.AsReadOnly();
                }
            }
        }

        private float _roomTemperature = 0.0f;

        private Task? _task;
        private CancellationTokenSource? _cts;

        private const float TemperatureDecayFactor = 0.5f;
        private const float RoomCeilingToFloorDistance = 3f;

        public RoomLogic(IRoomData data)
        {
            _data = data;

            foreach (var heaterData in _data.Heaters)
            {
                var heater = new HeaterLogic(heaterData);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
            }

            foreach (var sensorData in _data.HeatSensors)
            {
                var sensor = new HeatSensorLogic(sensorData);
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
            lock (_roomLock)
            {
                if (HeatSensors.Count == 0 || position.X > Width || position.X < 0f || position.Y > Height ||
                    position.Y < 0f) return 0f;

                var heatersTemp = 0f;
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                if (onHeaters.Count != 0)
                {
                    heatersTemp += (from heater in onHeaters
                        let dist = IPositionLogic.Distance(position, heater.Position)
                        select MathF.Min(heater.Temperature,
                            heater.Temperature * MathF.Exp(-TemperatureDecayFactor * dist))).Sum();
                    heatersTemp = MathF.Min(heatersTemp, onHeaters.Max(heater => heater.Temperature));
                }

                var sensorsSum = 0f;
                var sensorsDistSum = 0f;
                foreach (var sensor in HeatSensors)
                {
                    var dist = IPositionLogic.Distance(position, sensor.Position);
                    var weight = dist > 0f ? 1f / dist : float.MaxValue;
                    if (Math.Abs(weight - float.MaxValue) < 1e-10f) return sensor.Temperature;
                    sensorsSum += sensor.Temperature * weight;
                    sensorsDistSum += weight;
                }

                var sensorsTemp = sensorsDistSum > 0f ? sensorsSum / sensorsDistSum : 0f;

                var temp = MathF.Max(sensorsTemp, heatersTemp);

                return temp > 0 ? MathF.Min(temp, _roomTemperature) : _roomTemperature;
            }
        }

        private void SubscribeToHeater(HeaterLogic heater)
        {
            heater.PositionChanged += GetPositionChanged;
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.EnableChanged += GetEnableChanged;
        }

        private void UnsubscribeFromHeater(HeaterLogic heater)
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

            var heater = new HeaterLogic(_data.AddHeater(x, y, temperature));
            SubscribeToHeater(heater);
            lock (_roomLock)
            {
                _heaters.Add(heater);
            }
            return heater;
        }

        public bool ContainsHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                return _heaters.Any(heater => heater.Id == heaterId);
            }
        }

        public IHeaterLogic? GetHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null) return heater;

                var dataHeater = _data.GetHeater(heaterId);
                if (dataHeater == null) return null;

                heater = new HeaterLogic(dataHeater);
                SubscribeToHeater(heater);
                _heaters.Add(heater);
                return heater;
            }
        }

        public void RemoveHeater(Guid heaterId)
        {
            lock (_roomLock)
            {
                var heater = _heaters.Find(heater => heater.Id == heaterId);
                if (heater != null)
                {
                    UnsubscribeFromHeater(heater);
                    _heaters.Remove(heater);
                }
            }
            _data.RemoveHeater(heaterId);
        }

        private void ClearHeaters()
        {
            lock (_roomLock)
            {
                foreach (var heater in _heaters)
                {
                    UnsubscribeFromHeater(heater);
                }
                _heaters.Clear();
            }
        }

        private void SubscribeToHeatSensor(HeatSensorLogic sensor)
        {
            sensor.PositionChanged += GetPositionChanged;
            sensor.TemperatureChanged += GetTemperatureChanged;
        }

        private void UnsubscribeFromHeatSensor(HeatSensorLogic sensor)
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

            var sensor = new HeatSensorLogic(_data.AddHeatSensor(x, y));
            sensor.SetTemperature(GetTemperatureAtPosition(sensor.Position));
            SubscribeToHeatSensor(sensor);
            lock (_roomLock)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public bool ContainsHeatSensor(Guid sensorId)
        {
            lock (_roomLock) {
                return _heatSensors.Any(sensor => sensor.Id == sensorId);
            }
        }

        public IHeatSensorLogic? GetHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null) return sensor;

                var dataSensor = _data.GetHeatSensor(sensorId);
                if (dataSensor == null) return null;

                sensor = new HeatSensorLogic(dataSensor);
                SubscribeToHeatSensor(sensor);
                _heatSensors.Add(sensor);
                return sensor;
            }
        }

        public void RemoveHeatSensor(Guid sensorId)
        {
            lock (_roomLock)
            {
                var sensor = _heatSensors.Find(sensor => sensor.Id == sensorId);
                if (sensor != null)
                {
                    UnsubscribeFromHeatSensor(sensor);
                    _heatSensors.Remove(sensor);
                }
            }
            _data.RemoveHeatSensor(sensorId);
        }

        private void ClearHeatSensors()
        {
            lock (_roomLock)
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
                    await Task.Delay(2500, token);
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
            lock (_roomLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                if (onHeaters.Count != 0)
                {
                    foreach (var heater in onHeaters)
                    {
                        _roomTemperature += (heater.Temperature * TemperatureDecayFactor * deltaTime) / 
                                            (Width * Height * RoomCeilingToFloorDistance);
                    }
                    _roomTemperature = MathF.Min(_roomTemperature, onHeaters.Max(heater => heater.Temperature));
                }
                else
                {
                    _roomTemperature -= TemperatureDecayFactor * deltaTime;
                }
                _roomTemperature = MathF.Max(_roomTemperature, 0f);

                foreach (var sensor in _heatSensors)
                {
                    var temperature = sensor.Temperature - TemperatureDecayFactor * deltaTime;
                    if (onHeaters.Count == 0)
                    {
                        sensor.SetTemperature(MathF.Max(temperature, 0f));
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
                    sensor.SetTemperature(MathF.Max(temperature, 0f));
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