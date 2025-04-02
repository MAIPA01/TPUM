using System.Collections.ObjectModel;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;

namespace TPUM.Logic
{
    internal class Room : IRoom
    {
        private readonly Data.DataApiBase _data;

        private readonly List<IHeater> _heaters = [];
        public ReadOnlyCollection<IHeater> Heaters => _heaters.AsReadOnly();

        private readonly List<IHeatSensor> _heatSensors = [];
        public ReadOnlyCollection<IHeatSensor> HeatSensors => _heatSensors.AsReadOnly();

        public long Id { get; }
        public float Width { get; }
        public float Height { get; }
        public float AvgTemperature => HeatSensors.Count == 0 ? 0f : HeatSensors.Average(heatSensor => heatSensor.Temperature);

        private readonly Thread _thread;
        private bool _endThread;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangeEventHandler? EnableChange;
        public event PositionChangedEventHandler? PositionChanged;

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

        private void GetTemperatureChanged(object source, TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(source, args);
        }

        private void GetPositionChanged(object source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(source, args);
        }

        private void GetEnableChanged(object source, EnableChangeEventArgs args)
        {
            EnableChange?.Invoke(source, args);
        }

        public float GetTemperatureAtPosition(float x, float y)
        {
            if (x > Width || x < 0f || y > Height || y < 0f) return 0f;

            var pos = new Position(_data.CreatePosition(x, y));
            return HeatSensors.Count <= 0 ? 0f : 
                HeatSensors.Average(sensor => sensor.Temperature / 
                                              (IPosition.Distance(pos, sensor.Position) + 1));
        }

        public IHeater AddHeater(float x, float y, float temperature)
        {
            if (x > Width || x < 0f) 
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y > Height || y < 0f)
                throw new ArgumentOutOfRangeException(nameof(y));

            var heater = new Heater(_data.CreateHeater(x, y, temperature));
            heater.TemperatureChanged += GetTemperatureChanged;
            heater.PositionChanged += GetPositionChanged;
            heater.EnableChange += GetEnableChanged;
            _heaters.Add(heater);
            return heater;
        }

        public void RemoveHeater(long id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater == null) return;
            heater.TemperatureChanged -= GetTemperatureChanged;
            heater.PositionChanged -= GetPositionChanged;
            heater.EnableChange -= GetEnableChanged;
            _heaters.Remove(heater);
        }

        public void ClearHeaters()
        {
            foreach (var heater in _heaters)
            {
                heater.TemperatureChanged -= GetTemperatureChanged;
                heater.PositionChanged -= GetPositionChanged;
                heater.EnableChange -= GetEnableChanged;
            }
            _heaters.Clear();
        }

        public IHeatSensor AddHeatSensor(float x, float y)
        {
            if (x > Width || x < 0f)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y > Height || y < 0f)
                throw new ArgumentOutOfRangeException(nameof(y));

            var sensor = new HeatSensor(_data.CreateHeatSensor(x, y));
            sensor.TemperatureChanged += GetTemperatureChanged;
            sensor.PositionChanged += GetPositionChanged;
            sensor.SetTemperature(GetTemperatureAtPosition(sensor.Position.X, sensor.Position.Y));
            _heatSensors.Add(sensor);
            return sensor;
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.Find(sensor => sensor.Id == id);
            if (sensor == null) return;
            sensor.TemperatureChanged -= GetTemperatureChanged;
            sensor.PositionChanged -= GetPositionChanged;
            _heatSensors.Remove(sensor);
        }

        public void ClearHeatSensors()
        {
            foreach (var sensor in _heatSensors)
            {
                sensor.TemperatureChanged -= GetTemperatureChanged;
                sensor.PositionChanged -= GetPositionChanged;
            }
            _heatSensors.Clear();
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
            var avgTemperature = AvgTemperature;
            foreach (var heater in _heaters)
            {
                if (!heater.IsOn) continue;

                var temperatureDiff = (heater.Temperature - avgTemperature) * deltaTime;
                if (temperatureDiff <= 0f) continue;

                foreach (var heatSensor in _heatSensors)
                {
                    (heatSensor as HeatSensor)?.SetTemperature(heatSensor.Temperature + temperatureDiff / 
                                              IPosition.Distance(heatSensor.Position, heater.Position));
                }
            }
        }

        public void EndSimulation()
        {
            if ((_thread.ThreadState & ThreadState.Background) != ThreadState.Background) return;
            _endThread = true;
            _thread.Join();
        }

        public void Dispose()
        {
            EndSimulation();
            foreach (var heater in _heaters)
            {
                heater.TemperatureChanged -= GetTemperatureChanged;
                heater.PositionChanged -= GetPositionChanged;
                heater.EnableChange -= GetEnableChanged;
                heater.Dispose();
            }
            _heaters.Clear();
            foreach (var heatSensor in _heatSensors)
            {
                heatSensor.TemperatureChanged -= GetTemperatureChanged;
                heatSensor.PositionChanged -= GetPositionChanged;
                heatSensor.Dispose();
            }
            _heatSensors.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
