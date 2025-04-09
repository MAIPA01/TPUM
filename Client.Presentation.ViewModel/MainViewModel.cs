using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Client.Presentation.ViewModel
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

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected == value) return;
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(ConnectedString));
            }
        }
        private const string _connected = "";
        private const string _notConnected = "There is no connection to the server";
        public string ConnectedString => IsConnected ? _connected : _notConnected;

        public MainViewModel()
        {
            Instance = this;
        }

        public void SetView(Type viewType)
        {
            _currentView = Activator.CreateInstance(viewType);
            OnPropertyChanged(nameof(CurrentView));
        }

        public void SetConnectionStatus(bool connected)
        {
            IsConnected = connected;
        }

        protected void OnPropertyChanged([CallerMemberName] string? value = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}