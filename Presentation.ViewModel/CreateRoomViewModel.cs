using System.ComponentModel;

namespace TPUM.Presentation.ViewModel
{
    public class CreateRoomViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _roomName = "";

        public string RoomName
        {
            get => _roomName;
            set
            {
                if (_roomName == value) return;
                _roomName = value;
                OnPropertyChanged(nameof(RoomName));
            }
        }

        private float _roomWidth;

        public float RoomWidth
        {
            get => _roomWidth;
            set
            {
                if (Math.Abs(_roomWidth - value) < 1e-10f) return;
                _roomWidth = value;
                OnPropertyChanged(nameof(RoomWidth));
            }
        }

        private float _roomHeight;

        public float RoomHeight
        {
            get => _roomHeight;
            set
            {
                if (Math.Abs(_roomHeight - value) < 1e-10f) return;
                _roomHeight = value;
                OnPropertyChanged(nameof(RoomHeight));
            }
        }

        public ICommand AddRoomCommand { get; }

        public CreateRoomViewModel()
        {
            AddRoomCommand = ViewModelApi.Instance.CreateCommand(AddRoom, CanAddRoom);
            PropertyChanged += (_, _) => AddRoomCommand.OnCanExecuteChanged();
        }

        private void AddRoom(object? parameter)
        {
            ViewModelApi.Instance.AddRoom(_roomName, _roomWidth, _roomHeight);
            WindowManager.CloseLastSubWindow();
        }

        private bool CanAddRoom(object? parameter)
        {
            return _roomName.Length != 0 && _roomWidth > 0 && _roomHeight > 0;
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}