using TPUM.Client.Presentation.ViewModel.Events;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TPUM.Client.Presentation.Model;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class Position : IPosition
    {
        private readonly IPositionModel _model;

        public event PositionChangedEventHandler? PositionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public float X
        {
            get => _model.X;
            set => _model.X = value;
        }
        public float Y
        {
            get => _model.Y;
            set => _model.Y = value;
        }

        public Position(IPositionModel model)
        {
            _model = model;
            _model.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, IPositionModel lastPosition, IPositionModel newPosition)
        {
            PositionChanged?.Invoke(this, new Position(lastPosition), new Position(newPosition));
            OnPropertyChange(nameof(X));
            OnPropertyChange(nameof(Y));
        }

        public void SetPosition(float x, float y)
        {
            _model.SetPosition(x, y);
        }

        public void Dispose()
        {
            _model.PositionChanged -= GetPositionChanged;
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}