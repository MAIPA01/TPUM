﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using TPUM.Client.Presentation.ViewModel.Events;

namespace TPUM.Client.Presentation.ViewModel
{
    public interface IRoom : INotifyHeaterAdded, INotifyHeaterRemoved, INotifyHeatSensorAdded, INotifyHeatSensorRemoved,
        INotifyTemperatureChanged, INotifyPositionChanged, INotifyEnableChanged, INotifyPropertyChanged, IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        float Width { get; }
        float Height { get; }
        float AvgTemperature { get; }

        ReadOnlyObservableCollection<IHeater> Heaters { get; }
        ReadOnlyObservableCollection<IHeatSensor> HeatSensors { get; }

        void AddHeater(float x, float y, float temperature);
        void RemoveHeater(Guid heaterId);
        void AddHeatSensor(float x, float y);
        void RemoveHeatSensor(Guid sensorId);
    }
}