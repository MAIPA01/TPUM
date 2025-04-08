namespace TPUM.Client.Data
{
    internal class PositionData : IPositionData
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PositionData(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}