using System.Diagnostics;
using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    internal class RoomLogic : IRoomLogic
    {
        private readonly IRoomData _data;
        public Guid Id => _data.Id;

        // TODO: jednostki wypisac
        private readonly List<IHeaterLogic> _heaters = [];

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

        private readonly List<IHeatSensorLogic> _heatSensors = [];

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

        public float Width => _data.Width;
        public float Height => _data.Height;

        private float _roomTemperature = 0.0f;

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

        private Task? _task;
        private CancellationTokenSource? _cts;

        private const float TemperatureDecayFactor = 0.1f;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _roomTemperatureLock = new();

        public RoomLogic(IRoomData data)
        {
            _data = data;
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

        private float GetTemperatureAtPosition(float x, float y)
        {
            if (x > Width || x < 0f || y > Height || y < 0f || HeatSensors.Count == 0) return 0f;

            var pos = new PositionLogic(DataApiBase.GetApi().CreatePosition(x, y));
            var heatersTemp = 0f;
            lock (_heatersLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                if (onHeaters.Count != 0)
                {
                    heatersTemp += (from heater in onHeaters
                                    let dist = IPositionLogic.Distance(pos, heater.Position)
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
                    var dist = IPositionLogic.Distance(pos, sensor.Position);
                    var weight = dist > 0f ? 1f / dist : float.MaxValue;
                    if (Math.Abs(weight - float.MaxValue) < 1e-10f) return sensor.Temperature;
                    sensorsSum += sensor.Temperature * weight;
                    sensorsDistSum += weight;
                }
            }

            var sensorsTemp = sensorsDistSum > 0f ? sensorsSum / sensorsDistSum : 0f;

            var temp = MathF.Max(sensorsTemp, heatersTemp);

            return temp > 0 ? temp : _roomTemperature;
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

            var heater = new HeaterLogic(_data.AddHeater(x, y, temperature));
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
                IHeaterLogic? heater = _heaters.Find(heater => heater.Id == id);
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
            _data.ClearHeaters();
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

            IHeatSensorLogic sensor = new HeatSensorLogic(_data.AddHeatSensor(x, y));
            SubscribeToHeatSensor(sensor);
            lock (_heatSensors)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public bool ContainsHeatSensor(Guid id)
        {
            lock (_heatSensorsLock) {
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
                IHeatSensorLogic? sensor = _heatSensors.Find(sensor => sensor.Id == id);
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
            _data.ClearHeatSensors();
        }

        public void StartSimulation()
        {
            if (_task == null || _task.IsCompleted)
            {
                _cts = new CancellationTokenSource();
                _task = Task.Run(() => TaskMethod(_cts.Token), _cts.Token);
            }
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
                        (sensor as HeatSensorLogic)?.SetTemperature(MathF.Max(sensor.Temperature - TemperatureDecayFactor * deltaTime, 0f));

                        if (onHeaters.Count == 0) continue;
                        foreach (var tempDiff in from heater in onHeaters let dist = IPositionLogic.Distance(sensor.Position, heater.Position) 
                                 select MathF.Min(heater.Temperature, heater.Temperature * MathF.Exp(-TemperatureDecayFactor * dist)) * deltaTime)
                        {
                            (sensor as HeatSensorLogic)?.SetTemperature(sensor.Temperature + tempDiff -
                                                                   TemperatureDecayFactor * deltaTime);
                        }
                        (sensor as HeatSensorLogic)?.SetTemperature(MathF.Min(sensor.Temperature, onHeaters.Max(heater => heater.Temperature)));
                    }
                }
            }
        }

        public void EndSimulation()
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
    }
}