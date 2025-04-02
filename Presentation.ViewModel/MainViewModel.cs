using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Presentation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static MainViewModel? Instance { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private object? _currentView = null;
        public object? CurrentView
        {
            get => _currentView;
            set
            {
                if (value == null) return;
                SetView((Type)value);
            }
        }

        public MainViewModel()
        {
            Instance = this;
        }

        public void SetView(Type viewType)
        {
            _currentView = Activator.CreateInstance(viewType);
            OnPropertyChanged(nameof(CurrentView));
        }

        protected void OnPropertyChanged([CallerMemberName] string? value = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}