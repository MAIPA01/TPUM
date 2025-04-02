using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace TPUM.Presentation.ViewModel
{
    public class RoomsListViewModel : INotifyPropertyChanged
    {
        public static ReadOnlyObservableCollection<IRoom>? Rooms => MainViewModel.GetRooms();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand? AddRoomWindowCommand { get; } = MainViewModel.CreateCommand(OpenAddRoomWindow);
        public ICommand? RemoveRoomCommand { get; } = MainViewModel.CreateCommand(RemoveRoom);
        public ICommand? ShowRoomCommand { get; } = MainViewModel.CreateCommand(ShowRoom);

        private static void OpenAddRoomWindow(object? obj)
        {
            if (obj == null) return;
            MainViewModel.OpenSubWindow((Type)obj);
        }

        private static void RemoveRoom(object? parameter)
        {
            if (parameter == null) return;
            MainViewModel.RemoveRoom((long)parameter);
        }

        private static void ShowRoom(object? parameter)
        {
            if (parameter is not object[] { Length: 2 } parameters) return;
            MainViewModel.SetCurrentRoom((long)parameters[1]);
            MainViewModel.SetView((Type)parameters[0]);
        }
    }
}
