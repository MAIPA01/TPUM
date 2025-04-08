using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class Position : IPosition
    {
        private readonly Model.IPosition _position;

        public event PositionChangedEventHandler? PositionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public float X
        {
            get => _position.X;
            set => _position.X = value;
        }
        public float Y
        {
            get => _position.Y;
            set => _position.Y = value;
        }

        public Position(Model.IPosition position)
        {
            _position = position;
            _position.PositionChanged += GetPositionChanged;
        }

        private void GetPositionChanged(object? source, Model.Events.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), this));
            OnPropertyChange(nameof(X));
            OnPropertyChange(nameof(Y));
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