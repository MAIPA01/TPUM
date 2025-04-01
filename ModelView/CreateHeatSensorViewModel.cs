using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TPUM.Presentation.Model;

namespace TPUM.Presentation.ViewModel
{
    public class CreateHeatSensorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private float _x;
        public float X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        private float _y;
        public float Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public ICommand AddHeatSensorCommand { get; }

        public CreateHeatSensorViewModel()
        {
            AddHeatSensorCommand = new CustomCommand(AddHeatSensor, CanAddHeatSensor);
            PropertyChanged += (_, _) => ((CustomCommand)AddHeatSensorCommand).OnCanExecuteChanged();
        }

        private void AddHeatSensor(object? parameter)
        {
            ViewModelData.CurrentRoom?.AddHeatSensor(_x, _y);
            ViewModelData.CloseSubWindow();
        }

        private bool CanAddHeatSensor(object? parameter)
        {
            if (ViewModelData.CurrentRoom == null) return true;
            return _x <= ViewModelData.CurrentRoom.Width && _x >= 0 && _y <= ViewModelData.CurrentRoom.Height && _y >= 0;
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
