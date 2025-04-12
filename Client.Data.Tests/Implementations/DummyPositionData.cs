namespace TPUM.Client.Data.Tests
{
    internal class DummyPositionData : IPositionData
    {
        private float _x;
        public float X => _x;
        private float _y;
        public float Y => _y;

        public DummyPositionData(float x, float y)
        {
            _x = x;
            _y = y;
        }

        internal void SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}