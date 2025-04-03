using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;

namespace TPUM.Logic
{
    internal class Room : IRoom
    {
        // TODO: jednostki wypisać
        private readonly Data.DataApiBase _data;

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

        public long Id { get; }
        public float Width { get; }
        public float Height { get; }

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

        private readonly Thread _thread;
        private bool _endThread;

        private float _roomTemperature = 0f;
        private const float _temperatureDecayFactor = 0.1f;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _roomTemperatureLock = new();
        private readonly object _heatersLock = new();
        private readonly object _heatSensorsLock = new();

        public Room(long id, float width, float height, Data.DataApiBase data)
        {
            Id = id;
            _data = data;
            Width = width;
            Height = height;
            _thread = new Thread(ThreadMethod)
            {
                IsBackground = true
            };
            _endThread = false;
        }

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

            var pos = _data.CreatePosition(x, y);
            var heatersTemp = 0f;
            lock (_heatersLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                if (onHeaters.Count != 0)
                {
                    foreach (var heater in onHeaters)
                    {
                        var dist = IPosition.Distance(new Position(pos), heater.Position);
                        heatersTemp += MathF.Min(heater.Temperature, heater.Temperature * MathF.Exp(-_temperatureDecayFactor * dist));
                    }
                    heatersTemp = MathF.Min(heatersTemp, onHeaters.Max(heater => heater.Temperature));
                }
            }

            var sensorsSum = 0f;
            var sensorsDistSum = 0f;
            lock (_heatSensorsLock)
            {
                foreach (var sensor in HeatSensors)
                {
                    var dist = IPosition.Distance(new Position(pos), sensor.Position);
                    var weight = dist > 0f ? 1f / dist : float.MaxValue;
                    if (weight == float.MaxValue) return sensor.Temperature;
                    sensorsSum += sensor.Temperature * weight;
                    sensorsDistSum += weight;
                }
            }
            var sensorsTemp = sensorsDistSum > 0f ? sensorsSum / sensorsDistSum : 0f;

            var temp = MathF.Max(sensorsTemp, heatersTemp);

            return temp > 0f ? temp : _roomTemperature;
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

            var heater = new Heater(_data.CreateHeater(x, y, temperature));
            SubscribeToHeater(heater);
            lock (_heatersLock)
            {
                _heaters.Add(heater);
            }

            return heater;
        }

        public void RemoveHeater(long id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater == null) return;
            lock (_heatersLock)
            {
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

            var sensor = new HeatSensor(_data.CreateHeatSensor(x, y));
            SubscribeToHeatSensor(sensor);
            sensor.SetTemperature(GetTemperatureAtPosition(sensor.Position.X, sensor.Position.Y));
            lock (_heatSensors)
            {
                _heatSensors.Add(sensor);
            }
            return sensor;
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.Find(sensor => sensor.Id == id);
            if (sensor == null) return;
            lock (_heatSensorsLock)
            {
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

        public void StartSimulation()
        {
            if ((_thread.ThreadState & ThreadState.Background) == ThreadState.Background &&
                (_thread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                _thread.Start();
            }
        }

        private void ThreadMethod()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            while (!_endThread)
            {
                UpdateTemperature((float)stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
                Thread.Sleep(5);
            }
        }

        private void UpdateTemperature(float deltaTime)
        {
            lock (_heatersLock)
            {
                var onHeaters = _heaters.FindAll(heater => heater.IsOn);
                lock (_roomTemperatureLock)
                {
                    _roomTemperature = MathF.Max(_roomTemperature - _temperatureDecayFactor * deltaTime, 0f);
                    if (onHeaters.Count != 0)
                    {
                        foreach (var heater in onHeaters)
                        {
                            _roomTemperature += (heater.Temperature * _temperatureDecayFactor * deltaTime) / (Width * Height);
                        }
                        _roomTemperature = MathF.Min(_roomTemperature, onHeaters.Max(heater => heater.Temperature));
                    }
                }

                lock (_heatSensorsLock)
                {

                    foreach (var sensor in _heatSensors)
                    {
                        (sensor as HeatSensor)?.SetTemperature(MathF.Max(sensor.Temperature - _temperatureDecayFactor * deltaTime, 0f));
                            
                        if (onHeaters.Count != 0)
                        {
                            foreach (var heater in onHeaters)
                            {
                                var dist = IPosition.Distance(sensor.Position, heater.Position);
                                var tempDiff = MathF.Min(heater.Temperature,
                                    heater.Temperature * MathF.Exp(-_temperatureDecayFactor * dist)) * deltaTime;

                                (sensor as HeatSensor)?.SetTemperature(sensor.Temperature + tempDiff -
                                    _temperatureDecayFactor * deltaTime);
                            }
                            (sensor as HeatSensor)?.SetTemperature(MathF.Min(sensor.Temperature, onHeaters.Max(heater => heater.Temperature)));
                        }
                    }
                }
            }
        }

        public void EndSimulation()
        {
            if ((_thread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted || 
                (_thread.ThreadState & ThreadState.Background) != ThreadState.Background) return;
            _endThread = true;
            _thread.Join();
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