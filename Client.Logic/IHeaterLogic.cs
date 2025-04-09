namespace TPUM.Client.Logic
{
    public interface IHeaterLogic : IDisposable
    {
        Guid Id { get; }
        bool IsOn { get; }
        IPositionLogic Position { get; }
        float Temperature { get; }
    }
}