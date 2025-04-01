using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TPUM.Presentation.Model;

namespace TPUM.Presentation.ViewModel
{
    internal class ViewModelData : INotifyPropertyChanged
    {
        public static ViewModelData Instance { get; } = new();

        private readonly ModelApiBase _model = ModelApiBase.GetApi();

        public static ReadOnlyObservableCollection<IModelRoom> Rooms => Instance._model.Rooms;

        public static IModelRoom? CurrentRoom { get; private set; }

        public static object? CurrentView { get; private set; }

        public static void AddRoom(string name, float width, float height)
        {
            Instance._model.AddRoom(name, width, height);
        }

        public static void RemoveRoom(long id)
        {
            Instance._model.RemoveRoom(id);
        }

        public static void OpenSubWindow(Type windowType)
        {
            var win = Activator.CreateInstance(windowType);
            (win as Window)?.ShowDialog();
        }

        public static void CloseSubWindow()
        {
            var win = Application.Current.Windows
                .OfType<Window>()
                .LastOrDefault(w => w.Visibility == Visibility.Visible);
            win?.Close();
        }

        public static void SetView(Type viewType)
        {
            CurrentView = Activator.CreateInstance(viewType);
            Instance.OnPropertyChanged(nameof(CurrentView));
        }

        public static void SetCurrentRoom(long roomId)
        {
            foreach (var modelRoom in Rooms)
            {
                if (modelRoom.Id == roomId)
                {
                    CurrentRoom = modelRoom;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
