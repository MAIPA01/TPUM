namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class HeatSensorTest
    {
        [TestMethod]
        public void TestGetPosition()
        {
            HeatSensor sensor = new(2f, 2f);
            Position pos = new(2f, 2f);
            Assert.IsTrue(sensor.Position == pos);
        }

        [TestMethod]
        public void TestSetPosition()
        {
            HeatSensor sensor = new(2f, 2f);
            sensor.Position.X = 0f;
            Position pos = new(0f, 2f);
            Assert.IsTrue(sensor.Position == pos);
        }

        [TestMethod]
        public void TestGetTemperature()
        {
            HeatSensor sensor = new(2f, 2f);
            Assert.AreEqual(0f, sensor.Temperature, 1e-10f);
        }

        [TestMethod]
        public void TestSetTemperature()
        {
            HeatSensor sensor = new(2f, 2f);
            sensor.Temperature = 2f;
            Assert.AreEqual(2f, sensor.Temperature, 1e-10f);
        }

        private class TestSubscriber : IObserver<IHeatSensor>
        {
            public bool Good { get; private set; } = false;
            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(IHeatSensor value)
            {
                Good = true;
            }
        }

        [TestMethod]
        public void TestSubscribe()
        {
            TestSubscriber subscriber = new();
            HeatSensor sensor = new(2f, 2f);
            sensor.Subscribe(subscriber);

            sensor.Temperature = 0f;
            Assert.AreEqual(0f, sensor.Temperature, 1e-10f);
            Assert.IsFalse(subscriber.Good);

            sensor.Temperature = 10f;
            Assert.AreEqual(10f, sensor.Temperature, 1e-10f);
            Assert.IsTrue(subscriber.Good);
        }

        [TestMethod]
        public void TestGetHashCode()
        {
            HeatSensor sensor = new(2f, 2f);
            var hash = 3 * sensor.Position.GetHashCode() + 5 * sensor.Temperature.GetHashCode();
            Assert.AreEqual(hash, sensor.GetHashCode(), 1);
        }
    }
}
