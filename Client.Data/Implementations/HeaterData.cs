namespace TPUM.Client.Data
{
    internal class HeaterData : IHeaterData
    {
        public Guid Id { get; }
        public bool IsOn { get; set; }
        public IPositionData Position { get; set; }
        public float Temperature { get; set; }

        public HeaterData(Guid id, IPositionData position, float temperature, bool isOn = false)
        {
            Id = id;
            Position = position;
            Temperature = temperature;
            IsOn = isOn;
        }
    }
}