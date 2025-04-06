using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;

namespace TPUM.Logic
{
    internal class Room : IRoom
    {
        private readonly Data.IRoom _room;

        // TODO: jednostki wypisać
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

        public long Id => _room.Id;
        public float Width => _room.Width;
        public float Height => _room.Height;

        public float AvgTemperature => _room.AvgTemperature;

        private readonly Thread _thread;
        private bool _endThread;

        private const float TemperatureDecayFactor = 0.1f;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;

        private readonly object _roomTemperatureLock = new();
        private readonly object _heatersLock = new();
        private readonly object _heatSensorsLock = new();

        public Room(Data.IRoom room)
        {
            _room = room;
            _room.TemperatureChanged += GetTemperatureChanged;
            _thread = new Thread(ThreadMethod)
            {
                IsBackground = true
            };
            _endThread = false;
        }

        private void GetTemperatureChanged(object? source, Data.TemperatureChangedEventArgs args)
        {
            if (source != _room) return;
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
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

            var heater = new Heater(_room.AddHeater(x, y, temperature));
            SubscribeToHeater(heater);
            lock (_heatersLock)
            {
                _heaters.Add(heater);
            }
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
            _room.RemoveHeater(id);
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
            _room.ClearHeaters();
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

            var sensor = new HeatSensor(_room.AddHeatSensor(x, y));
            SubscribeToHeatSensor(sensor);
            lock (_heatSensors)
            {
                _heatSensors.Add(sensor);
            }
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
            _room.RemoveHeatSensor(id);
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
            _room.ClearHeatSensors();
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
                    _room.RoomTemperature = MathF.Max(_room.RoomTemperature - TemperatureDecayFactor * deltaTime, 0f);
                    if (onHeaters.Count != 0)
                    {
                        foreach (var heater in onHeaters)
                        {
                            _room.RoomTemperature += (heater.Temperature * TemperatureDecayFactor * deltaTime) / (Width * Height);
                        }
                        _room.RoomTemperature = MathF.Min(_room.RoomTemperature, onHeaters.Max(heater => heater.Temperature));
                    }
                }

                lock (_heatSensorsLock)
                {

                    foreach (var sensor in _heatSensors)
                    {
                        (sensor as HeatSensor)?.SetTemperature(MathF.Max(sensor.Temperature - TemperatureDecayFactor * deltaTime, 0f));

                        if (onHeaters.Count == 0) continue;
                        foreach (var tempDiff in from heater in onHeaters let dist = IPosition.Distance(sensor.Position, heater.Position) 
                                 select MathF.Min(heater.Temperature, heater.Temperature * MathF.Exp(-TemperatureDecayFactor * dist)) * deltaTime)
                        {
                            (sensor as HeatSensor)?.SetTemperature(sensor.Temperature + tempDiff -
                                                                   TemperatureDecayFactor * deltaTime);
                        }
                        (sensor as HeatSensor)?.SetTemperature(MathF.Min(sensor.Temperature, onHeaters.Max(heater => heater.Temperature)));
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