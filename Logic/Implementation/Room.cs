using System.Collections.ObjectModel;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;

namespace TPUM.Logic
{
    internal class Room : IRoom
    {
        private readonly Data.DataApiBase _data;

        private readonly List<IHeater> _heaters = [];
        public IReadOnlyCollection<IHeater> Heaters => _heaters.AsReadOnly();

        private readonly List<IHeatSensor> _heatSensors = [];
        public IReadOnlyCollection<IHeatSensor> HeatSensors => _heatSensors.AsReadOnly();

        public long Id { get; }
        public float Width { get; }
        public float Height { get; }
        public float AvgTemperature => HeatSensors.Count == 0 ? 0f : HeatSensors.Average(heatSensor => heatSensor.Temperature);

        private readonly Thread _thread;
        private bool _endThread;

        public event TemperatureChangedEventHandler? TemperatureChanged;
        public event EnableChangedEventHandler? EnableChanged;
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

            float tempSum = 0f;
            float distSum = 0f;
            foreach (var sensor in HeatSensors)
            {
                float dist = IPosition.Distance(new Position(pos), sensor.Position) + 1f;
                tempSum += sensor.Temperature * dist;
                distSum += dist;
            }

            return tempSum / distSum;
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
            _heaters.Add(heater);
            return heater;
        }

        public void RemoveHeater(long id)
        {
            var heater = _heaters.Find(heater => heater.Id == id);
            if (heater == null) return;
            UnsubscribeFromHeater(heater);
            _heaters.Remove(heater);
        }

        public void ClearHeaters()
        {
            foreach (var heater in _heaters)
            {
                UnsubscribeFromHeater(heater);
            }
            _heaters.Clear();
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
            _heatSensors.Add(sensor);
            return sensor;
        }

        public void RemoveHeatSensor(long id)
        {
            var sensor = _heatSensors.Find(sensor => sensor.Id == id);
            if (sensor == null) return;
            UnsubscribeFromHeatSensor(sensor);
            _heatSensors.Remove(sensor);
        }

        public void ClearHeatSensors()
        {
            foreach (var sensor in _heatSensors)
            {
                UnsubscribeFromHeatSensor(sensor);
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
            // TODO: Błąd gdy nie ma threada
            if ((_thread.ThreadState & ThreadState.Background) != ThreadState.Background) return;
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
