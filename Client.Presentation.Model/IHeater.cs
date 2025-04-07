namespace TPUM.Client.Presentation.Model
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
        bool IsOn { get; }

        void TurnOn();
        void TurnOff();
    }
}