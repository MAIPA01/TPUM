namespace TPUM.Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int a = 2 + 2;
            Assert.AreEqual(4, a, 1);
        }
    }
}
