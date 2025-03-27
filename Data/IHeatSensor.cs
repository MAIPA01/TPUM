namespace TPUM.Data
{
    public interface IHeatSensor : IObservable<IHeatSensor>, IDisposable
    {
        Position Position { get; set; }
        float Temperature { get; set; }
    }
}
