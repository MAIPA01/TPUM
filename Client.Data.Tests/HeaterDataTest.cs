namespace TPUM.Client.Data.Tests
{
    [TestClass]
    public sealed class HeaterDataTest
    {
        private IHeaterData? _heater = null;
        private static readonly Guid Id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
        private const float X = 2f;
        private const float Y = 2f;
        private const float Temp = 21f;

        [TestInitialize]
        public void Setup()
        {
            _heater = new DummyHeaterData(Id, X, Y, Temp);
        }

        [TestMethod]
        public void Heater_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(Id, _heater!.Id);
            Assert.AreEqual(X, _heater!.Position.X);
            Assert.AreEqual(Y, _heater!.Position.Y);
            Assert.AreEqual(Temp, _heater!.Temperature, 1e-10f);
            Assert.IsFalse(_heater!.IsOn);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionCorrectly()
        {
            Assert.AreEqual(X, _heater!.Position.X);
            Assert.AreEqual(Y, _heater!.Position.Y);
            _heater.SetPosition(X + 1f, Y + 1f);
            Assert.AreEqual(X + 1f, _heater!.Position.X);
            Assert.AreEqual(Y + 1f, _heater!.Position.Y);
        }

        [TestMethod]
        public void IsOn_ShouldReturnCorrectState_WhenUpdated()
        {
            Assert.IsFalse(_heater!.IsOn);
            _heater.IsOn = true;
            Assert.IsTrue(_heater!.IsOn);
            _heater.IsOn = false;
            Assert.IsFalse(_heater!.IsOn);
        }

        [TestMethod]
        public void Temperature_ShouldReturnCorrectValue()
        {
            Assert.AreEqual(Temp, _heater!.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetTemperature_ShouldUpdateTemperatureCorrectly()
        {
            _heater!.Temperature = Temp + 1f;
            Assert.AreEqual(Temp + 1f, _heater.Temperature, 1e-10f);
        }
    }
}