using Presentation.ViewModel;
using System.ComponentModel;

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
            AddHeatSensorCommand = ViewModelApi.Instance.CreateCommand(AddHeatSensor, CanAddHeatSensor);
            PropertyChanged += (_, _) => AddHeatSensorCommand.OnCanExecuteChanged();
        }

        private void AddHeatSensor(object? parameter)
        {
            ViewModelApi.Instance.CurrentRoom?.AddHeatSensor(_x, _y);
            WindowManager.CloseLastSubWindow();
        }

        private bool CanAddHeatSensor(object? parameter)
        {
            var room = ViewModelApi.Instance.CurrentRoom;
            if (room == null) return true;
            return _x <= room.Width && _x >= 0 && _y <= room.Height && _y >= 0;
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}