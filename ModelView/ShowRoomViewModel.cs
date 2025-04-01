using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TPUM.Data;
using TPUM.Presentation.Model;

namespace TPUM.Presentation.ViewModel
{
    public class ShowRoomViewModel : INotifyPropertyChanged
    {
        public IModelRoom? CurrentRoom => ViewModelData.CurrentRoom;

        public string RoomName => CurrentRoom != null ? CurrentRoom.Name : "";

        public float RoomWidth => CurrentRoom != null ? CurrentRoom.Width : 0f;

        public float RoomHeight => CurrentRoom != null ? CurrentRoom.Height : 0f;

        public int RoomTemp => CurrentRoom != null ? (int)CurrentRoom.AvgTemperature : 0;

        public ReadOnlyObservableCollection<IModelHeater> Heaters => 
            CurrentRoom != null ? CurrentRoom.Heaters : new ReadOnlyObservableCollection<IModelHeater>([]);

        public ReadOnlyObservableCollection<IModelHeatSensor> HeatSensors =>
            CurrentRoom != null ? CurrentRoom.HeatSensors : new ReadOnlyObservableCollection<IModelHeatSensor>([]);

        public ICommand BackCommand { get; }
        public ICommand TurnHeaterCommand { get; }
        public ICommand MoveHeaterCommand { get; }
        public ICommand RemoveHeaterCommand { get; }
        public ICommand AddHeaterCommand { get; }
        public ICommand MoveHeatSensorCommand { get; }
        public ICommand RemoveHeatSensorCommand { get; }
        public ICommand AddHeatSensorCommand { get; }

        public ShowRoomViewModel()
        {
            BackCommand = new CustomCommand(Back);
            TurnHeaterCommand = new CustomCommand(TurnHeater);
            MoveHeaterCommand = new CustomCommand(NotImplemented);
            RemoveHeaterCommand = new CustomCommand(NotImplemented);
            AddHeaterCommand = new CustomCommand(AddHeater);
            MoveHeatSensorCommand = new CustomCommand(NotImplemented);
            RemoveHeatSensorCommand = new CustomCommand(NotImplemented);
            AddHeatSensorCommand = new CustomCommand(AddHeatSensor);

            if (CurrentRoom == null) return;
            CurrentRoom.TemperatureChanged += GetTemperatureChange;
            CurrentRoom.EnableChange += GetEnableChange;
            CurrentRoom.PositionChanged += GetPositionChange;
        }

        private void GetTemperatureChange(object source, TemperatureChangedEventArgs args)
        {
            OnPropertyChanged(nameof(RoomTemp));
        }

        private void GetEnableChange(object source,  EnableChangeEventArgs args)
        {
        }

        private void GetPositionChange(object source, PositionChangedEventArgs args)
        {
        }

        private void NotImplemented(object? parameter)
        {
            MessageBox.Show("Not Implemented");
        }

        private void Back(object? parameter)
        {
            if (parameter == null) return;
            if (CurrentRoom != null)
            {
                CurrentRoom.TemperatureChanged -= GetTemperatureChange;
                CurrentRoom.EnableChange -= GetEnableChange;
                CurrentRoom.PositionChanged -= GetPositionChange;
            }
            ViewModelData.SetView((Type)parameter);
        }

        private void TurnHeater(object? parameter)
        {
            if (parameter == null) return;
            var heater = Heaters.ToList().Find(heater => heater.Id == (long)parameter);
            if (heater == null) return;

            if (heater.IsOn)
            {
                heater.TurnOff();
            }
            else
            {
                heater.TurnOn();
            }
        }

        private void AddHeater(object? parameter)
        {
            if (parameter == null) return;
            ViewModelData.OpenSubWindow((Type)parameter);
        }

        private void AddHeatSensor(object? parameter)
        {
            if (parameter == null) return;
            ViewModelData.OpenSubWindow((Type)parameter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
