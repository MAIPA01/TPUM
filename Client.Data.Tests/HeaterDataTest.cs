namespace TPUM.Client.Data.Tests
{
    [TestClass]
    public sealed class HeaterDataTest
    {
        private IHeaterData _heater = default!;
        private static readonly Guid _id = Guid.Parse("44465E20-25A1-4FA3-BE1D-0AB7E5C739F4");
        private const float _x = 2f;
        private const float _y = 2f;
        private IPositionData _position = default!;
        private const float _temp = 21f;

        [TestInitialize]
        public void Setup()
        {
            _position = new DummyPositionData(_x, _y);
            _heater = new DummyHeaterData(_id, _position, _temp);
        }

        [TestMethod]
        public void Heater_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_id, _heater.Id);
            Assert.AreEqual(_position, _heater.Position);
            Assert.AreEqual(_x, _heater.Position.X);
            Assert.AreEqual(_y, _heater.Position.Y);
            Assert.AreEqual(_temp, _heater.Temperature, 1e-10f);
            Assert.IsFalse(_heater.IsOn);
        }

        [TestMethod]
        public void SetPosition_ShouldUpdatePositionCorrectly()
        {
            Assert.AreEqual(_position, _heater.Position);
            _heater.Position.X += 1f;
            _position.X += 1f;
            Assert.AreEqual(_position, _heater.Position);
        }

        [TestMethod]
        public void IsOn_ShouldReturnCorrectState_WhenUpdated()
        {
            Assert.IsFalse(_heater.IsOn);
            _heater.IsOn = true;
            Assert.IsTrue(_heater.IsOn);
            _heater.IsOn = false;
            Assert.IsFalse(_heater.IsOn);
        }

        [TestMethod]
        public void Temperature_ShouldReturnCorrectValue()
        {
            Assert.AreEqual(_temp, _heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void SetTemperature_ShouldUpdateTemperatureCorrectly()
        {
            _heater.Temperature = _temp + 1f;
            Assert.AreEqual(_temp + 1f, _heater.Temperature, 1e-10f);
        }
    }
}