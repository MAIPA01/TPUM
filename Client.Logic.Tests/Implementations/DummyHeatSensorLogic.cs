using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeatSensorLogic : IHeatSensorLogic
    {
        private readonly IHeatSensorData _data;

        public Guid Id => _data.Id;

        public IPositionLogic Position => new DummyPositionLogic(_data.Position);

        public float Temperature => _data.Temperature;

        public DummyHeatSensorLogic(IHeatSensorData data)
        {
            _data = data;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return 3 * Position.GetHashCode() + 5 * Temperature.GetHashCode();
        }
    }
}