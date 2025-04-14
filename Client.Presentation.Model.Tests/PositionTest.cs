namespace TPUM.Client.Presentation.Model.Tests
{
    [TestClass]
    public sealed class PositionTest
    {
        [TestMethod]
        public void Position_GetValues_ShouldReturnProperValues()
        {
            var pos = new DummyPositionModel(new TestPositionLogic(1f, 1f));
            Assert.AreEqual(1f, pos.X, 1e-10f);
            Assert.AreEqual(1f, pos.Y, 1e-10f);
        }
    }
}