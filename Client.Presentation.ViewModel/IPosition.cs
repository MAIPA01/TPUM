using System.ComponentModel;
using TPUM.Client.Presentation.ViewModel.Events;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IPosition : INotifyPositionChanged, INotifyPropertyChanged, IDisposable
    {
        float X { get; set; }
        float Y { get; set; }

        void SetPosition(float x, float y);
    }
}