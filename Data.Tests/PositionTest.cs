namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class PositionTest
    {
        [TestMethod]
        public void TestGetX()
        {
            Position pos = new(2f, 2f);
            Assert.AreEqual(2f, pos.X, 1e-10f);
        }

        [TestMethod]
        public void TestSetX()
        {
            Position pos = new(2f, 2f);
            pos.X = 3f;
            Assert.AreEqual(3f, pos.X, 1e-10f);
        }

        [TestMethod]
        public void TestGetY()
        {
            Position pos = new(2f, 2f);
            Assert.AreEqual(2f, pos.Y, 1e-10f);
        }

        [TestMethod]
        public void TestSetY()
        {
            Position pos = new(2f, 2f);
            pos.Y = 3f;
            Assert.AreEqual(3f, pos.Y, 1e-10f);
        }

        [TestMethod]
        public void TestDistance()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 1f);
            Assert.AreEqual(MathF.Sqrt(2f), Position.Distance(pos1, pos2), 1e-10f);
            Assert.AreEqual(MathF.Sqrt(2f), Position.Distance(pos2, pos1), 1e-10f);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 2f);
            Assert.IsFalse(pos1 == pos2);
            Assert.IsFalse(pos2 == pos1);

            Position pos3 = new(2f, 1f);
            Assert.IsFalse(pos1 == pos3);
            Assert.IsFalse(pos3 == pos1);

            Position pos4 = new(2f, 2f);
            Assert.IsTrue(pos1 == pos4);
            Assert.IsTrue(pos4 == pos1);
        }

        [TestMethod]
        public void TestNotEqualOperator()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 2f);
            Assert.IsTrue(pos1 != pos2);
            Assert.IsTrue(pos2 != pos1);

            Position pos3 = new(2f, 1f);
            Assert.IsTrue(pos1 != pos3);
            Assert.IsTrue(pos3 != pos1);

            Position pos4 = new(2f, 2f);
            Assert.IsFalse(pos1 != pos4);
            Assert.IsFalse(pos4 != pos1);
        }

        [TestMethod]
        public void TestEquals()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 2f);
            Assert.IsFalse(pos1.Equals(pos2));
            Assert.IsFalse(pos2.Equals(pos1));

            Position pos3 = new(2f, 1f);
            Assert.IsFalse(pos1.Equals(pos3));
            Assert.IsFalse(pos3.Equals(pos1));

            Position pos4 = new(2f, 2f);
            Assert.IsTrue(pos1.Equals(pos4));
            Assert.IsTrue(pos4.Equals(pos1));

            Assert.IsFalse(pos1.Equals(new object()));
        }

        [TestMethod]
        public void TestGetHashCode()
        {
            Position pos = new(2f, 2f);
            var hash = 3 * pos.X.GetHashCode() + 5 * pos.Y.GetHashCode();
            Assert.AreEqual(hash, pos.GetHashCode(), 1);
        }
    }
}
