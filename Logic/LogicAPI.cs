using TPUM.Data;

namespace TPUM.Logic
{
    public abstract class LogicAPIBase
    {
        public abstract Room CreateRoom();

        public static LogicAPIBase GetAPI(DataAPIBase? data)
        {
            return new LogicAPI(data ?? DataAPIBase.GetAPI());
        }
    }

    internal class LogicAPI(DataAPIBase data) : LogicAPIBase
    {
        private DataAPIBase _data = data;

        public override Room CreateRoom()
        {
            return new Room();
        }
    }
}
