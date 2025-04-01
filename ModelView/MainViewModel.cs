using TPUM.Presentation.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TPUM.Presentation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static ReadOnlyObservableCollection<IModelRoom> Rooms => ViewModelData.Rooms;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object? CurrentView
        {
            get => ViewModelData.CurrentView;
            set
            {
                if (value == null) return;
                ViewModelData.SetView((Type)value);
            }
        }

        public MainViewModel()
        {
            ViewModelData.Instance.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
