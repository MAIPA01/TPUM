using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeatSensorLogic : IHeatSensorLogic
    {
        private readonly IHeatSensorData _data;

        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;
        public Guid Id => _data.Id;

        private readonly object _posLock = new();
        private IPositionLogic _position;
        public IPositionLogic Position
        {
            get => _position;
            set
            {
                lock (_posLock)
                {
                    if (_position.Equals(value)) return;
                    IPositionLogic last = new DummyPositionLogic(DataApiBase.GetApi().CreatePosition(_position.X, _position.Y));
                    _position.PositionChanged -= GetPositionChanged;
                    // By nie wywolaly sie eventy 2 razy
                    _position.X = value.X;
                    _position.Y = value.Y;
                    _position.PositionChanged += GetPositionChanged;
                    OnPositionChanged(this, last);
                }
            }
        }

        private readonly object _tempLock = new();
        public float Temperature => _data.Temperature;

        public DummyHeatSensorLogic(IHeatSensorData data)
        {
            _data = data;
            _position = new DummyPositionLogic(_data.Position);
            _position.PositionChanged += GetPositionChanged;
        }

        internal void SetTemperature(float temperature)
        {
            lock (_tempLock)
            {
                if (Math.Abs(_data.Temperature - temperature) < 1e-10f) return;
                float last = _data.Temperature;
                _data.Temperature = temperature;
                OnTemperatureChanged(this, last);
            }
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastPosition, Position));
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            _position.Dispose();
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return 3 * Position.GetHashCode() + 5 * Temperature.GetHashCode();
        }

        private void OnPositionChanged(object? source, IPositionLogic lastPosition)
        {
            PositionChanged?.Invoke(source, new PositionChangedEventArgs(lastPosition, _position));
        }

        private void OnTemperatureChanged(object? source, float lastTemperature)
        {
            TemperatureChanged?.Invoke(source, new TemperatureChangedEventArgs(lastTemperature, _data.Temperature));
        }
    }
}