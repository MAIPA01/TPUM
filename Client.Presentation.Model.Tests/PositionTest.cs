namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class PositionTest
    {
        [TestMethod]
        public void Position_UpdateX_ShouldTriggerEvent()
        {
            var pos = new DummyPosition(new TestPositionLogic(1, 1));
            bool eventTriggered = false;

            pos.PositionChanged += (s, e) => eventTriggered = true;

            pos.X = 2;

            Assert.IsTrue(eventTriggered);
        }
    }
}
