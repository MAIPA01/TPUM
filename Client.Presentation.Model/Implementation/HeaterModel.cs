using TPUM.Client.Logic;
using TPUM.Client.Presentation.Model.Events;

namespace TPUM.Client.Presentation.Model
{
    internal class HeaterModel : IHeaterModel
    {
        private readonly IHeaterLogic _logic;

        public event EnableChangedEventHandler? EnableChanged;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public Guid Id => _logic.Id;

        private readonly PositionModel _position;
        public IPositionModel Position => _position;

        public float Temperature
        {
            get => _logic.Temperature;
            set => _logic.Temperature = value;
        }

        public bool IsOn => _logic.IsOn;

        public HeaterModel(IHeaterLogic logic)
        {
            _logic = logic;
            _logic.PositionChanged += GetPositionChanged;
            _logic.TemperatureChanged += GetTemperatureChanged;
            _logic.EnableChanged += GetEnableChanged;

            _position = new PositionModel(_logic.Position);
        }

        private void GetPositionChanged(object? source, IPositionLogic lastPosition, IPositionLogic newPosition)
        {
            PositionChanged?.Invoke(this, new PositionModel(lastPosition), new PositionModel(newPosition));
        }

        private void GetTemperatureChanged(object? source, float lastTemperature, float newTemperature)
        {
            TemperatureChanged?.Invoke(this, lastTemperature, newTemperature);
        }

        private void GetEnableChanged(object? source, bool lastEnable, bool newEnable)
        {
            EnableChanged?.Invoke(this, lastEnable, newEnable);
        }

        public void SetPosition(float x, float y)
        {
            _logic.SetPosition(x, y);
        }

        public void TurnOn()
        {
            _logic.TurnOn();
        }

        public void TurnOff()
        {
            _logic.TurnOff();
        }

        public void Dispose()
        {
            _logic.PositionChanged -= GetPositionChanged;
            _logic.TemperatureChanged -= GetTemperatureChanged;
            _logic.EnableChanged -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }
    }
}