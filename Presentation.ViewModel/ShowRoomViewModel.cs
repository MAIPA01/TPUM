using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace TPUM.Presentation.ViewModel
{
    public class ShowRoomViewModel : INotifyPropertyChanged
    {
        // TODO: Ask if are you sure you want to remove...
        public IRoom? CurrentRoom => ViewModelApi.Instance.CurrentRoom;

        public string RoomName => CurrentRoom?.Name ?? "";

        public float RoomWidth => CurrentRoom?.Width ?? 0f;

        public float RoomHeight => CurrentRoom?.Height ?? 0f;

        public int RoomTemp => CurrentRoom != null ? (int)CurrentRoom.AvgTemperature : 0;

        public ReadOnlyObservableCollection<IHeater> Heaters =>
            CurrentRoom != null ? CurrentRoom.Heaters : new ReadOnlyObservableCollection<IHeater>([]);

        public ReadOnlyObservableCollection<IHeatSensor> HeatSensors =>
            CurrentRoom != null ? CurrentRoom.HeatSensors : new ReadOnlyObservableCollection<IHeatSensor>([]);

        public ICommand BackCommand { get; }
        public ICommand MoveHeaterCommand { get; }
        public ICommand RemoveHeaterCommand { get; }
        public ICommand AddHeaterCommand { get; }
        public ICommand MoveHeatSensorCommand { get; }
        public ICommand RemoveHeatSensorCommand { get; }
        public ICommand AddHeatSensorCommand { get; }

        public ShowRoomViewModel()
        {
            BackCommand = ViewModelApi.Instance.CreateCommand(Back);
            MoveHeaterCommand = ViewModelApi.Instance.CreateCommand(NotImplemented);
            RemoveHeaterCommand = ViewModelApi.Instance.CreateCommand(RemoveHeater);
            AddHeaterCommand = ViewModelApi.Instance.CreateCommand(AddHeater);
            MoveHeatSensorCommand = ViewModelApi.Instance.CreateCommand(NotImplemented);
            RemoveHeatSensorCommand = ViewModelApi.Instance.CreateCommand(RemoveHeatSensor);
            AddHeatSensorCommand = ViewModelApi.Instance.CreateCommand(AddHeatSensor);

            if (CurrentRoom == null) return;
            CurrentRoom.TemperatureChanged += GetTemperatureChange;
            CurrentRoom.EnableChanged += GetEnableChange;
            CurrentRoom.PositionChanged += GetPositionChange;
        }

        private void GetTemperatureChange(object? source, TemperatureChangedEventArgs args)
        {
            OnPropertyChanged(nameof(RoomTemp));
        }

        private static void GetEnableChange(object? source, EnableChangeEventArgs args)
        {
        }

        private static void GetPositionChange(object? source, PositionChangedEventArgs args)
        {
        }

        private static void NotImplemented(object? parameter)
        {
            MessageBox.Show("Not Implemented");
        }

        private void Back(object? parameter)
        {
            if (parameter == null) return;
            if (CurrentRoom != null)
            {
                CurrentRoom.TemperatureChanged -= GetTemperatureChange;
                CurrentRoom.EnableChanged -= GetEnableChange;
                CurrentRoom.PositionChanged -= GetPositionChange;
            }
            MainViewModel.Instance?.SetView((Type)parameter);
        }

        private static void AddHeater(object? parameter)
        {
            if (parameter == null) return;
            WindowManager.OpenSubWindow((Type)parameter);
        }

        private void RemoveHeater(object? parameter)
        {
            if (parameter == null) return;
            CurrentRoom?.RemoveHeater((long)parameter);
        }

        private static void AddHeatSensor(object? parameter)
        {
            if (parameter == null) return;
            WindowManager.OpenSubWindow((Type)parameter);
        }

        private void RemoveHeatSensor(object? parameter)
        {
            if (parameter == null) return;
            CurrentRoom?.RemoveHeatSensor((long)parameter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}