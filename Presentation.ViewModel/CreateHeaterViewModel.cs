using System.ComponentModel;

namespace TPUM.Presentation.ViewModel
{
    public class CreateHeaterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private float _x;
        public float X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) < 1e-10f) return;
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
                if (Math.Abs(_y - value) < 1e-10f) return;
                _y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        private float _temperature;
        public float Temperature
        {
            get => _temperature;
            set
            {
                if (Math.Abs(_temperature - value) < 1e-10f) return;
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        public ICommand AddHeaterCommand { get; }

        public CreateHeaterViewModel()
        {
            AddHeaterCommand = ViewModelApi.Instance.CreateCommand(AddHeater, CanAddHeater);
            PropertyChanged += (_, _) => AddHeaterCommand.OnCanExecuteChanged();
        }

        private void AddHeater(object? parameter)
        {
            ViewModelApi.Instance.CurrentRoom?.AddHeater(_x, _y, _temperature);
            WindowManager.CloseLastSubWindow();
        }

        private bool CanAddHeater(object? parameter)
        {
            var temp = _temperature >= 0f;
            var room = ViewModelApi.Instance.CurrentRoom;
            if (room == null) return temp;
            return temp && _x <= room.Width && _x >= 0 && _y <= room.Height && _y >= 0;
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}