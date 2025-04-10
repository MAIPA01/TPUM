using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Client.Presentation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ViewModelApiBase ViewModelApi { get; }

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
        private const string Connected = "";
        private const string NotConnected = "There is no connection to the server";
        public string ConnectedString => IsConnected ? Connected : NotConnected;

        public MainViewModel()
        {
            ViewModelApi = ViewModelApiBase.GetApi("ws://localhost:5000/ws");
            ViewModelApi.ClientConnected += GetClientConnected;
            Instance = this;
        }

        private void GetClientConnected(object? source)
        {
            IsConnected = true;
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