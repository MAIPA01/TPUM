using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TPUM.Client.Presentation.ViewModel
{
    public class RoomsListViewModel : INotifyPropertyChanged
    {
        public static ReadOnlyObservableCollection<IRoom>? Rooms => ViewModelApi.Instance.Rooms;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand AddRoomWindowCommand { get; } = new CustomCommand(OpenAddRoomWindow, _ => MainViewModel.Instance?.IsConnected ?? false);
        public ICommand RemoveRoomCommand { get; } = new CustomCommand(RemoveRoom);
        public ICommand ShowRoomCommand { get; } = new CustomCommand(ShowRoom);

        public RoomsListViewModel()
        {
            if (MainViewModel.Instance != null)
            {
                MainViewModel.Instance.PropertyChanged += GetPropertyChanged;
            }
        }

        private void GetPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsConnected")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddRoomWindowCommand)));
            }
        }

        private static void OpenAddRoomWindow(object? obj)
        {
            if (obj == null) return;
            WindowManager.OpenSubWindow((Type)obj);
        }

        private static void RemoveRoom(object? parameter)
        {
            if (parameter == null) return;
            if (!WindowManager.MakeYesNoWindow(
                    "Are you sure you want to remove Room of id: '" + (Guid)parameter + "'?",
                    "Room Removal")
               ) return;
            ViewModelApi.Instance.RemoveRoom((Guid)parameter);
        }

        private static void ShowRoom(object? parameter)
        {
            if (parameter is not object[] { Length: 2 } parameters) return;
            ViewModelApi.Instance.SetCurrentRoom((Guid)parameters[1]);
            MainViewModel.Instance?.SetView((Type)parameters[0]);
        }
    }
}