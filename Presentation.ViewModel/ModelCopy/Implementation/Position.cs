using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Presentation.ViewModel
{
    internal class Position : IPosition
    {
        private readonly Model.IPosition _position;

        public event PositionChangedEventHandler? PositionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public float X 
        {
            get => _position.X;
            set
            {
                _position.X = value;
                OnPropertyChange(nameof(X));
            }
        }
        public float Y 
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                OnPropertyChange(nameof(Y));
            }
        }

        public Position(Model.IPosition position)
        {
            _position = position;
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, Model.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), this));
        }

        public void Dispose()
        {
            _position.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
