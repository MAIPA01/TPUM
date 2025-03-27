namespace TPUM.Data
{
    public interface IHeater : IObservable<IHeater>, IDisposable
    {
        Position Position { get; set; }
        float Temperature { get; set; }
        void TurnOn();
        void TurnOff();
        bool IsOn();
    }
}
