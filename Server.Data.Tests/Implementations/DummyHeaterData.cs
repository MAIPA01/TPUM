namespace TPUM.Server.Data.Tests
{
    internal class DummyHeaterData : IHeaterData
    {
        public Guid Id { get; }
        public bool IsOn { get; set; }
        public IPositionData Position { get; set; }
        public float Temperature { get; set; }

        public DummyHeaterData(Guid id, IPositionData position, float temperature, bool isOn = false)
        {
            Id = id;
            Position = position;
            Temperature = temperature;
            IsOn = isOn;
        }
    }
}