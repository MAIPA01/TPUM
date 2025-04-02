﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Presentation.ViewModel;

namespace TPUM.Presentation.ViewModel
{
    public class ShowRoomViewModel : INotifyPropertyChanged
    {
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
        //public ICommand TurnHeaterCommand { get; }
        public ICommand MoveHeaterCommand { get; }
        public ICommand RemoveHeaterCommand { get; }
        public ICommand AddHeaterCommand { get; }
        public ICommand MoveHeatSensorCommand { get; }
        public ICommand RemoveHeatSensorCommand { get; }
        public ICommand AddHeatSensorCommand { get; }

        // TODO: dla każdego api nie base zrobić instance które jest widoczne dla internal
        public ShowRoomViewModel()
        {
            BackCommand = ViewModelApi.Instance.CreateCommand(Back);
            //TurnHeaterCommand = ViewModelApi.Instance.CreateCommand(TurnHeater);
            MoveHeaterCommand = ViewModelApi.Instance.CreateCommand(NotImplemented);
            RemoveHeaterCommand = ViewModelApi.Instance.CreateCommand(NotImplemented);
            AddHeaterCommand = ViewModelApi.Instance.CreateCommand(AddHeater);
            MoveHeatSensorCommand = ViewModelApi.Instance.CreateCommand(NotImplemented);
            RemoveHeatSensorCommand = ViewModelApi.Instance.CreateCommand(NotImplemented);
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

        private static void GetEnableChange(object? source,  EnableChangeEventArgs args)
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

        //private void TurnHeater(object? parameter)
        //{
        //    if (parameter == null) return;
        //    var heater = Heaters.ToList().Find(heater => heater.Id == (long)parameter);
        //    if (heater == null) return;

        //    if (heater.IsOn)
        //    {
        //        heater.TurnOff();
        //    }
        //    else
        //    {
        //        heater.TurnOn();
        //    }
        //}

        private static void AddHeater(object? parameter)
        {
            if (parameter == null) return;
            WindowManager.OpenSubWindow((Type)parameter);
        }

        private static void AddHeatSensor(object? parameter)
        {
            if (parameter == null) return;
            WindowManager.OpenSubWindow((Type)parameter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
