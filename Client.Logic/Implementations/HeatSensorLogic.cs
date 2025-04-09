using TPUM.Client.Data;

namespace TPUM.Client.Logic
{
    internal class HeatSensorLogic : IHeatSensorLogic
    {
        private readonly IHeatSensorData _data;

        public Guid Id => _data.Id;

        public IPositionLogic Position => new PositionLogic(_data.Position);

        public float Temperature => _data.Temperature;

        public HeatSensorLogic(IHeatSensorData data)
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