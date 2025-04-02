using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TPUM.Presentation.Model;

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

        private float _temperature;
        public float Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature == value) return;
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
            bool temp = _temperature >= 0f;
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
