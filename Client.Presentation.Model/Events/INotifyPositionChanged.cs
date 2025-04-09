namespace TPUM.Client.Presentation.Model.Events
{
    public delegate void PositionChangedEventHandler(object? source, PositionChangedEventArgs e);
    public class PositionChangedEventArgs : EventArgs
    {
        public float LastX { get; }
        public float LastY { get; }
        public float NewX { get; }
        public float NewY { get; }

        public PositionChangedEventArgs(float lastX, float lastY, float newX, float newY)
        {
            LastX = lastX;
            LastY = lastY;
            NewX = newX;
            NewY = newY;
        }
    }

    public interface INotifyPositionChanged
    {
        event PositionChangedEventHandler? PositionChanged;
    }
}