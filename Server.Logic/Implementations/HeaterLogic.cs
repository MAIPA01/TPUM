﻿using TPUM.Server.Data;
using TPUM.Server.Logic.Events;

namespace TPUM.Server.Logic
{
    internal class HeaterLogic : IHeaterLogic
    {
        private readonly IHeaterData _data;

        public Guid Id => _data.Id;

        private readonly object _onLock = new();
        public bool IsOn {
            get
            {
                lock (_onLock)
                {
                    return _data.IsOn;
                }
            }
        }

        private readonly object _posLock = new();
        private IPositionLogic _position;
        public IPositionLogic Position
        {
            get
            {
                lock (_posLock)
                {
                    return _position;
                }
            }
            set
            {
                lock (_posLock)
                {
                    if (_position.Equals(value)) return;
                    IPositionLogic last = new PositionLogic(DataApiBase.GetApi().CreatePosition(_position.X, _position.Y));
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
                lock (_tempLock)
                {
                    return IsOn ? _data.Temperature : 0f;
                }
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

        public HeaterLogic(IHeaterData data)
        {
            _data = data;
            _position = new PositionLogic(_data.Position);
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(this, lastPosition, newPosition);
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
            PositionChanged?.Invoke(source, lastPosition, _position);
        }

        private void OnEnableChanged(object? source, bool lastEnable)
        {
            EnableChanged?.Invoke(source, lastEnable, _data.IsOn);
        }

        private void OnTemperatureChanged(object? source, float lastTemperature)
        {
            TemperatureChanged?.Invoke(source, lastTemperature, _data.Temperature);
        }
    }
}