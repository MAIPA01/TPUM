using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPUM.Logic
{
    internal class Heater : IHeater
    {
        private readonly Data.IHeater _heater;

        public event EnableChangeEventHandler? EnableChange;
        public event PositionChangedEventHandler? PositionChanged;
        public event TemperatureChangedEventHandler? TemperatureChanged;

        public long Id => _heater.Id;
        public bool IsOn => _heater.IsOn;

        public IPosition Position
        {
            get => new Position(_heater.Position);
            set
            {
                _heater.Position.X = value.X;
                _heater.Position.Y = value.Y;
            }
        }

        public float Temperature
        {
            get => _heater.Temperature;
            set => _heater.Temperature = value;
        }

        public Heater(Data.IHeater heater)
        {
            _heater = heater;
            _heater.PositionChanged += GetPositionChanged;
            _heater.TemperatureChanged += GetTemperatureChanged;
            _heater.EnableChange += GetEnableChanged;
        }

        private void GetPositionChanged(object source, Data.PositionChangedEventArgs args)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(new Position(args.LastPosition), Position));
        }

        private void GetTemperatureChanged(object source, Data.TemperatureChangedEventArgs args)
        {
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(args.LastTemperature, args.NewTemperature));
        }

        private void GetEnableChanged(object source, Data.EnableChangeEventArgs args)
        {
            EnableChange?.Invoke(this, new EnableChangeEventArgs(args.LastEnable, args.NewEnable));
        }

        public void TurnOn()
        {
            _heater.TurnOn();
        }

        public void TurnOff()
        {
            _heater.TurnOff();
        }

        public void Dispose()
        {
            _heater.PositionChanged -= GetPositionChanged;
            _heater.TemperatureChanged -= GetTemperatureChanged;
            _heater.EnableChange -= GetEnableChanged;
            GC.SuppressFinalize(this);
        }
    }
}
