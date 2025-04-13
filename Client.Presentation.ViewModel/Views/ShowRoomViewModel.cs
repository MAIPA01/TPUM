using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace TPUM.Client.Presentation.ViewModel
{
    public class ShowRoomViewModel : INotifyPropertyChanged
    {
        // TODO: Add Move Functionality
        // TODO: Add Rename Functionality
        // TODO: Add Clear Functionality
        // TODO: Add Room View On Plane
        // TODO: Add Heaters View On Plane
        // TODO: Add HeatSensors View On Plane

        public event PropertyChangedEventHandler? PropertyChanged;

        public IRoom? CurrentRoom => MainViewModel.Instance?.ViewModelApi.CurrentRoom;

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
            BackCommand = new CustomCommand(Back);
            MoveHeaterCommand = new CustomCommand(NotImplemented);
            RemoveHeaterCommand = new CustomCommand(RemoveHeater);
            AddHeaterCommand = new CustomCommand(AddHeater);
            MoveHeatSensorCommand = new CustomCommand(NotImplemented);
            RemoveHeatSensorCommand = new CustomCommand(RemoveHeatSensor);
            AddHeatSensorCommand = new CustomCommand(AddHeatSensor);

            if (CurrentRoom == null) return;
            CurrentRoom.TemperatureChanged += GetTemperatureChange;
            CurrentRoom.EnableChanged += GetEnableChange;
            CurrentRoom.PositionChanged += GetPositionChange;
            CurrentRoom.HeaterAdded += GetHeaterAdded;
            CurrentRoom.HeaterRemoved += GetHeaterRemoved;
            CurrentRoom.HeatSensorAdded += GetHeatSensorAdded;
            CurrentRoom.HeatSensorRemoved += GetHeatSensorRemoved;
        }

        private void GetTemperatureChange(object? source, float lastTemperature, float newTemperature)
        {
            OnPropertyChanged(nameof(Heaters));
            OnPropertyChanged(nameof(HeatSensors));
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetEnableChange(object? source, bool lastEnable, bool newEnable)
        {
            OnPropertyChanged(nameof(Heaters));
            OnPropertyChanged(nameof(HeatSensors));
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetPositionChange(object? source, IPosition lastPosition, IPosition newPosition)
        {
            OnPropertyChanged(nameof(Heaters));
            OnPropertyChanged(nameof(HeatSensors));
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetHeaterAdded(object? source, IHeater heater)
        {
            OnPropertyChanged(nameof(Heaters));
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetHeaterRemoved(object? source, Guid heaterId)
        {
            OnPropertyChanged(nameof(Heaters));
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetHeatSensorAdded(object? source, IHeatSensor sensor)
        {
            OnPropertyChanged(nameof(HeatSensors));
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetHeatSensorRemoved(object? source, Guid sensorId)
        {
            OnPropertyChanged(nameof(HeatSensors));
            OnPropertyChanged(nameof(RoomTemp));
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
                CurrentRoom.HeaterAdded -= GetHeaterAdded;
                CurrentRoom.HeaterRemoved -= GetHeaterRemoved;
                CurrentRoom.HeatSensorAdded -= GetHeatSensorAdded;
                CurrentRoom.HeatSensorRemoved -= GetHeatSensorRemoved;
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
            if (WindowManager.MakeYesNoWindow(
                    "Are you sure you want to remove Heater of id: '" + (Guid)parameter + "'?",
                    "Heater Removal")
                )
            {
                CurrentRoom?.RemoveHeater((Guid)parameter);
            }
        }

        private void AddHeatSensor(object? parameter)
        {
            if (parameter == null) return;
            WindowManager.OpenSubWindow((Type)parameter);
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void RemoveHeatSensor(object? parameter)
        {
            if (parameter == null) return;
            if (!WindowManager.MakeYesNoWindow(
                    "Are you sure you want to remove Heat Sensor of id: '" + (Guid)parameter + "'?",
                    "Heat Sensor Removal")
               ) return;
            CurrentRoom?.RemoveHeatSensor((Guid)parameter);
            OnPropertyChanged(nameof(RoomTemp));
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}