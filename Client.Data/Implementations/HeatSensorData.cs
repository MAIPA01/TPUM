using TPUM.Client.Data.Events;

namespace TPUM.Client.Data
{
    internal class HeatSensorData : IHeatSensorData
    {
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        private readonly DataApi _data;
        private readonly Guid _roomId;

        private readonly object _heatSensorLock = new();

        public Guid Id { get; }

        private readonly PositionData _position;
        public IPositionData Position
        {
            get
            {
                lock (_heatSensorLock)
                {
                    return _position;
                }
            }
        }
        private float _temperature;
        public float Temperature
        {
            get
            {
                lock (_heatSensorLock)
                {
                    return _temperature;
                }
            }
        }

        public HeatSensorData(DataApi data, Guid roomId, Guid id, PositionData position, float temperature = 0.0f)
        {
            _data = data;
            _roomId = roomId;
            Id = id;
            _position = position;
            _temperature = temperature;
        }

        public void SetPosition(float x, float y)
        {
            lock (_heatSensorLock)
            {
                if (MathF.Abs(_position.X - x) < 1e-10f && MathF.Abs(_position.Y - y) < 1e-10f) return;
                _data.UpdateHeatSensor(_roomId, Id, x, y);
            }
        }

        internal void UpdateDataFromServer(float x, float y, float temperature)
        {
            lock (_heatSensorLock)
            {
                if (MathF.Abs(_position.X - x) > 1e-10f || MathF.Abs(_position.Y - y) > 1e-10f)
                {
                    var lastPosition = new PositionData(_position.X, _position.Y);
                    _position.SetPosition(x, y);
                    PositionChanged?.Invoke(this, lastPosition, _position);
                }

                if (MathF.Abs(_temperature - temperature) > 1e-10f)
                {
                    var lastTemperature = _temperature;
                    _temperature = temperature;
                    TemperatureChanged?.Invoke(this, lastTemperature, _temperature);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}