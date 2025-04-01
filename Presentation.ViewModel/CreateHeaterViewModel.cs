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
            AddHeaterCommand = new CustomCommand(AddHeater, CanAddHeater);
            PropertyChanged += (_, _) => ((CustomCommand)AddHeaterCommand).OnCanExecuteChanged();
        }

        private void AddHeater(object? parameter)
        {
            ViewModelData.CurrentRoom?.AddHeater(_x, _y, _temperature);
            ViewModelData.CloseSubWindow();
        }

        private bool CanAddHeater(object? parameter)
        {
            bool inRoom = true;
            if (ViewModelData.CurrentRoom != null) 
            {
                inRoom = _x <= ViewModelData.CurrentRoom.Width && _x >= 0 && _y <= ViewModelData.CurrentRoom.Height && _y >= 0;
            }
            return inRoom && _temperature >= 0f;
        }

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
