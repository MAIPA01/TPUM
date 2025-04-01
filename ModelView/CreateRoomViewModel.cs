using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TPUM.Presentation.Model;

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
                if (_roomWidth == value) return;
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
                if (_roomHeight == value) return;
                _roomHeight = value;
                OnPropertyChanged(nameof(RoomHeight));
            }
        }

        public ICommand AddRoomCommand { get; }

        public CreateRoomViewModel()
        {
            AddRoomCommand = new CustomCommand(AddRoom, CanAddRoom);
            PropertyChanged += (_, _) => ((CustomCommand)AddRoomCommand).OnCanExecuteChanged();
        }

        private void AddRoom(object? parameter)
        {
            ViewModelData.AddRoom(_roomName, _roomWidth, _roomHeight);
            ViewModelData.CloseSubWindow();
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
