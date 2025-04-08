using TPUM.Client.Data;
using TPUM.Client.Logic.Events;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeaterLogic : IHeaterLogic
    {
        private readonly IHeaterData _data;

        public Guid Id => _data.Id;

        private readonly object _onLock = new();
        public bool IsOn => _data.IsOn;

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
        public float Temperature
        {
            get
            {
                return IsOn ? _data.Temperature : 0f;
            }
            set
            {
                lock (_tempLock)
                {
                    if (Math.Abs(_data.Temperature - value) < 1e-10f) return;
                    float last = _data.Temperature;
                    _data.Temperature = value;
                    OnTemperatureChanged(this, last);
                }
            }
        }

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public DummyHeaterLogic(IHeaterData data)
        {
            _data = data;
            _position = new DummyPositionLogic(_data.Position);
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(args.LastPosition, Position));
        }

        public void TurnOn()
        {
            lock (_onLock)
            {
                if (IsOn) return;
                bool last = _data.IsOn;
                _data.IsOn = true;
                OnEnableChanged(this, last);
            }
        }

        public void TurnOff()
        {
            lock (_onLock)
            {
                if (!IsOn) return;
                bool last = _data.IsOn;
                _data.IsOn = false;
                OnEnableChanged(this, last);
            }
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            _position.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnPositionChanged(object? source, IPositionLogic lastPosition)
        {
            PositionChanged?.Invoke(source, new PositionChangedEventArgs(lastPosition, _position));
        }

        private void OnEnableChanged(object? source, bool lastEnable)
        {
            EnableChanged?.Invoke(source, new EnableChangedEventArgs(lastEnable, _data.IsOn));
        }

        private void OnTemperatureChanged(object? source, float lastTemperature)
        {
            TemperatureChanged?.Invoke(source, new TemperatureChangedEventArgs(lastTemperature, _data.Temperature));
        }
    }
}