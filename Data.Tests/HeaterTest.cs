namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class HeaterTest
    {
        [TestMethod]
        public void TestGetPosition()
        {
            Heater heater = new(2f, 2f, 21f);
            Position pos = new(2f, 2f);
            Assert.IsTrue(heater.Position == pos);
        }

        [TestMethod]
        public void TestSetPosition()
        {
            Heater heater = new(2f, 2f, 21f);
            Position pos = new(3f, 2f);
            Assert.IsTrue(heater.Position != pos);
            heater.Position.X = 3f;
            Assert.IsTrue(heater.Position == pos);
        }

        [TestMethod]
        public void TestIsOn()
        {
            Heater heater = new(2f, 2f, 21f);
            Assert.IsFalse(heater.IsOn());

            heater.TurnOn();
            Assert.IsTrue(heater.IsOn());

            heater.TurnOff();
            Assert.IsFalse(heater.IsOn());
        }

        [TestMethod]
        public void TestGetTemperature()
        {
            Heater heater = new(2f, 2f, 21f);
            Assert.AreEqual(0f, heater.Temperature, 1e-10f);

            heater.TurnOn();
            Assert.AreEqual(21f, heater.Temperature, 1e-10f);
        }

        [TestMethod]
        public void TestSetTemperature()
        {
            Heater heater = new(2f, 2f, 21f);
            heater.TurnOn();
            heater.Temperature = 22f;
            Assert.AreEqual(22f, heater.Temperature, 1e-10f);
        }

        private class TestSubscriber : IObserver<IHeater>
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

            public void OnNext(IHeater value)
            {
                Good = true;
            }
        }

        [TestMethod]
        public void TestSubscribe()
        {
            TestSubscriber subscriber = new();
            Heater heater = new(2f, 2f, 21f);
            heater.Subscribe(subscriber);
            heater.Temperature = 2f;

            Assert.IsTrue(subscriber.Good);
        }
    }
}
