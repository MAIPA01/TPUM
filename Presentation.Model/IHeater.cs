namespace TPUM.Presentation.Model
{
    public interface IHeater : INotifyEnableChanged, INotifyPositionChanged, INotifyTemperatureChanged, IDisposable
    {
        long Id { get; }
        IPosition Position { get; set; }
        float Temperature { get; set; }
        bool IsOn { get; }

        ICommand TurnOffCommand { get; }
        ICommand TurnOnCommand { get; }

        //void TurnOn();
        //void TurnOff();
    }
}