namespace TPUM.Client.Logic
{
    public interface IHeatSensorLogic : IDisposable
    {
        Guid Id { get; }
        IPositionLogic Position { get; }
        float Temperature { get; }
    }
}