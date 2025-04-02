using Presentation.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TPUM.Presentation.ViewModel
{
    public class RoomsListViewModel : INotifyPropertyChanged
    {
        public static ReadOnlyObservableCollection<IRoom>? Rooms => ViewModelApi.Instance.Rooms;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand? AddRoomWindowCommand { get; } = ViewModelApi.Instance.CreateCommand(OpenAddRoomWindow);
        public ICommand? RemoveRoomCommand { get; } = ViewModelApi.Instance.CreateCommand(RemoveRoom);
        public ICommand? ShowRoomCommand { get; } = ViewModelApi.Instance.CreateCommand(ShowRoom);

        private static void OpenAddRoomWindow(object? obj)
        {
            if (obj == null) return;
            WindowManager.OpenSubWindow((Type)obj);
        }

        private static void RemoveRoom(object? parameter)
        {
            if (parameter == null) return;
            ViewModelApi.Instance.RemoveRoom((long)parameter);
        }

        private static void ShowRoom(object? parameter)
        {
            if (parameter is not object[] { Length: 2 } parameters) return;
            ViewModelApi.Instance.SetCurrentRoom((long)parameters[1]);
            MainViewModel.Instance?.SetView((Type)parameters[0]);
        }
    }
}