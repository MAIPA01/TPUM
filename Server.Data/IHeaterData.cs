namespace TPUM.Server.Data
{
    public interface IHeaterData
    {
        Guid Id { get; }
        bool IsOn { get; set; }
        IPositionData Position { get; set; }
        float Temperature { get; set; }
    }
}