using TPUM.Client.Data;

namespace TPUM.Client.Logic
{
    internal class HeaterLogic : IHeaterLogic
    {
        private readonly IHeaterData _data;

        public Guid Id => _data.Id;

        public bool IsOn => _data.IsOn;

        public IPositionLogic Position => new PositionLogic(_data.Position);

        public float Temperature => IsOn ? _data.Temperature : 0f;

        public HeaterLogic(IHeaterData data)
        {
            _data = data;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}