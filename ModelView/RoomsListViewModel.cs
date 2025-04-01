using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TPUM.Presentation.Model;

namespace TPUM.Presentation.ViewModel
{
    public class RoomsListViewModel : INotifyPropertyChanged
    {
        public static ReadOnlyObservableCollection<IModelRoom> Rooms => ViewModelData.Rooms;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand AddRoomWindowCommand { get; } = new CustomCommand(OpenAddRoomWindow, _ => true);
        public ICommand RemoveRoomCommand { get; } = new CustomCommand(RemoveRoom, _ => true);
        public ICommand ShowRoomCommand { get; } = new CustomCommand(ShowRoom, _ => true);

        private static void OpenAddRoomWindow(object? obj)
        {
            if (obj == null) return;
            ViewModelData.OpenSubWindow((Type)obj);
        }

        private static void RemoveRoom(object? parameter)
        {
            if (parameter == null) return;
            ViewModelData.RemoveRoom((long)parameter);
        }

        private static void ShowRoom(object? parameter)
        {
            if (parameter is not object[] { Length: 2 } parameters) return;
            ViewModelData.SetCurrentRoom((long)parameters[1]);
            ViewModelData.SetView((Type)parameters[0]);
        }
    }
}
