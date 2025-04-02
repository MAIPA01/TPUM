namespace TPUM.Data
{
    public abstract class DataApiBase : IDisposable
    {
        public abstract IHeater CreateHeater(float x, float y, float temperature);
        public abstract IHeatSensor CreateHeatSensor(float x, float y);

        public abstract IPosition CreatePosition(float x, float y);
        public abstract void Dispose();

        public static DataApiBase GetApi()
        {
            return new DataApi();
        }
    }

    internal class DataApi : DataApiBase
    {
        public override IHeater CreateHeater(float x, float y, float temperature)
        {
            return new Heater(new Random().NextInt64(), x, y, temperature);
        }

        public override IHeatSensor CreateHeatSensor(float x, float y)
        {
            return new HeatSensor(new Random().NextInt64(), x, y);
        }

        public override IPosition CreatePosition(float x, float y)
        {
            return new Position(x, y);
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}