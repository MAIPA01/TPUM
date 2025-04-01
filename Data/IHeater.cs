namespace TPUM.Data
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        bool IsOn { get; }
        Position Position { get; set; }
        float Temperature { get; set; }
        void TurnOn();
        void TurnOff();
    }
}
