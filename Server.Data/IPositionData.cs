namespace TPUM.Server.Data
{
    public interface IPositionData : IDisposable
    {
        public float X { get; }
        public float Y { get; }
    }
}