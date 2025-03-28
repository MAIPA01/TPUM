namespace TPUM.Data
{
    internal class HeatSensor(Position position) : IHeatSensor
    {
        private readonly List<IObserver<IHeatSensor>> _observers = [];

        Position _position = position;
        public Position Position {
            get
            {
                return _position;
            }
            
            set 
            {
                if (_position != value)
                {
                    _position = value;
                    Notify();
                }
            }
        }
        
        float _temperature = .0f;
        public float Temperature {
            get
            {
                return _temperature;
            }

            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    Notify();
                }
            }
        }

        public IDisposable Subscribe(IObserver<IHeatSensor> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return this;
        }

        private void Notify()
        {
            foreach (var observer in _observers) {
                observer.OnNext(this);
            }
        }

        public void Dispose()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
            _position.Dispose();
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return 3 * Position.GetHashCode() + 5 * Temperature.GetHashCode(); 
        }
    }
}
