using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    internal class DummyHeaterLogic : IHeaterLogic
    {
        private readonly IHeaterData _data;

        public Guid Id => _data.Id;

        public bool IsOn => _data.IsOn;

        public IPositionLogic Position => new DummyPositionLogic(_data.Position);

        public float Temperature => IsOn ? _data.Temperature : 0f;

        public DummyHeaterLogic(IHeaterData data)
        {
            _data = data;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}