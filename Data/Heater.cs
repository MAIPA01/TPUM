namespace TPUM.Data
{
    internal class Heater(Position position, float temperature) : IHeater
    {
        private readonly List<IObserver<IHeater>> _observers = [];

        private bool _isOn = false;

        private Position _position = position;
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
        private float _temperature = temperature;
        public float Temperature { 
            get
            {
                return _isOn ? _temperature : 0f;
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

        public bool IsOn()
        {
            return _isOn;
        }

        public void TurnOff()
        {
            _isOn = false;
            Notify();
        }

        public void TurnOn()
        {
            _isOn = true;
            Notify();
        }

        public IDisposable Subscribe(IObserver<IHeater> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return this;
        }

        private void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(this);
            }
        }

        public void Dispose()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
            Position.Dispose();
        }
    }
}
