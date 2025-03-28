using TPUM.Data;

namespace TPUM.Logic
{
    public class Room(float width, float height, DataAPIBase? data) : IRoom
    {
        private DataAPIBase _data = data ?? DataAPIBase.GetAPI();
        public List<IObserver<IRoom>> _observers = [];
        public List<IHeater> Heaters { get; private set; } = [];
        public List<IHeatSensor> HeatSensors { get; private set; } = [];

        public float Width { get; private set; } = width;
        public float Height { get; private set; } = height;

        public float GetAvgTemperature()
        {
            float avg = 0f;
            if (HeatSensors.Count > 0)
            {
                foreach (var heatSensor in HeatSensors)
                {
                    avg += heatSensor.Temperature;
                }
                avg /= HeatSensors.Count;
            }
            return avg;
        }

        public float GetTemperatureAtPosition(Position pos)
        {
            float temp = 0f;

            if (HeatSensors.Count > 0)
            {
                foreach (var sensor in HeatSensors)
                {
                    temp += sensor.Temperature / (Position.Distance(pos, sensor.Position) + 1);
                }
                temp /= HeatSensors.Count;
            }

            return temp;
        }

        public void AddHeater(Position pos, float temperature)
        {
            IHeater heater = _data.CreateHeater(pos, temperature);
            Heaters.Add(heater);
            heater.Subscribe(this);
            Notify();
        }

        public void AddHeatSensor(Position pos)
        {
            IHeatSensor sensor = _data.CreateHeatSensor(pos);
            HeatSensors.Add(sensor);
            sensor.Temperature = GetTemperatureAtPosition(sensor.Position);
            sensor.Subscribe(this);
            Notify();
        }

        public void UpdateTemperature(float deltaTime)
        {
            float avgTemperature = GetAvgTemperature();
            foreach (var heatSensor in HeatSensors)
            {
                foreach (var heater in Heaters)
                {
                    if (heater.IsOn())
                    {
                        float temperatureDiff = (heater.Temperature - avgTemperature) * deltaTime;
                        temperatureDiff = temperatureDiff < 0f ? 0f : temperatureDiff;

                        heatSensor.Temperature += temperatureDiff / Position.Distance(heatSensor.Position, heater.Position);
                    }
                }
            }

            Notify();
        }

        public IDisposable Subscribe(IObserver<IRoom> observer)
        {
            _observers.Add(observer);
            return this;
        }

        private void Notify()
        {
            foreach(var observer in _observers)
            {
                observer.OnNext(this);
            }
        }

        public void Dispose()
        {
            foreach(var observer in _observers)
            {
                observer.OnCompleted();
            }
            _observers.Clear();
            foreach(var heater in Heaters)
            {
                heater.Dispose();
            }
            Heaters.Clear();
            foreach(var heatSensor in HeatSensors)
            {
                heatSensor.Dispose();
            }
            HeatSensors.Clear();
            GC.SuppressFinalize(this);
        }

        public void OnCompleted() 
        {
            Notify();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(IHeater value)
        {
            Notify();
        }

        public void OnNext(IHeatSensor value)
        {
            Notify();
        }
    }
}
