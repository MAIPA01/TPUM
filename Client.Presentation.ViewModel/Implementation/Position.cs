using System.ComponentModel;
using System.Runtime.CompilerServices;
using TPUM.Client.Presentation.Model;

namespace TPUM.Client.Presentation.ViewModel
{
    internal class Position : IPosition
    {
        private readonly IPositionModel _model;

        public event PropertyChangedEventHandler? PropertyChanged;

        public float X => _model.X;

        public float Y => _model.Y;

        public Position(IPositionModel model)
        {
            _model = model;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChange([CallerMemberName] string? name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}