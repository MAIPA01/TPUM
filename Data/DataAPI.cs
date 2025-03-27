namespace TPUM.Data
{
    public abstract class DataAPIBase {
        public abstract Heater CreateHeater(Position position, float temperature);
        public abstract HeatSensor CreateHeatSensor(Position position);

        public static DataAPIBase GetAPI()
        {
            return new DataAPI();
        }
    }

    public class DataAPI : DataAPIBase
    {
        public override Heater CreateHeater(Position position, float temperature)
        {
            return new Heater(position, temperature);
        }

        public override HeatSensor CreateHeatSensor(Position position)
        {
            return new HeatSensor(position);
        }
    }
}
