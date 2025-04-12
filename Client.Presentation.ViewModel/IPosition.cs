using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IPosition : INotifyPropertyChanged, IDisposable
    {
        float X { get; }
        float Y { get; }
    }
}