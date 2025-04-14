namespace TPUM.Server.Logic.Tests
{
    [TestClass]
    public sealed class PositionLogicTest
    {
        [TestMethod]
        public void PositionLogic_SetPosition_UpdatesXAndY()
        {
            var data = new PositionData();
            var logic = new DummyPositionLogic(data);

            logic.SetPosition(10.0f, 15.5f);

            Assert.AreEqual(10.0f, logic.X);
            Assert.AreEqual(15.5f, logic.Y);
        }

        [TestMethod]
        public void PositionLogic_Equals_ReturnsTrueForSameCoordinates()
        {
            var data1 = new PositionData(5f, 5f);
            var data2 = new PositionData(5f, 5f);

            var pos1 = new DummyPositionLogic(data1);
            var pos2 = new DummyPositionLogic(data2);

            Assert.IsTrue(pos1.Equals(pos2));
        }
    }
}