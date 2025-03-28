namespace TPUM.Data
{
    public abstract class DataAPIBase : IDisposable {
        public abstract IHeater CreateHeater(Position position, float temperature);
        public abstract IHeatSensor CreateHeatSensor(Position position);
        public abstract void Dispose();

        public static DataAPIBase GetAPI()
        {
            return new DataAPI();
        }
    }

    internal class DataAPI : DataAPIBase
    {
        public override IHeater CreateHeater(Position position, float temperature)
        {
            return new Heater(position, temperature);
        }

        public override IHeatSensor CreateHeatSensor(Position position)
        {
            return new HeatSensor(position);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
