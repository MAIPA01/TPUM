namespace TPUM.Server.Data.Tests
{
    internal class DummyPositionData : IPositionData
    {
        public float X { get; set; }
        public float Y { get; set; }

        public DummyPositionData(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}