using TPUM.Client.Data;

namespace TPUM.Client.Logic.Tests
{
    [TestClass]
    public sealed class RoomLogicTest
    {
        private IRoomLogic _room = default!;
        private const float _width = 100f;
        private const float _height = 100f;

        public class TestPositionData : IPositionData
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

        public class TestHeaterData : IHeaterData
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public IPositionData Position { get; set; } = new TestPositionData();
            public float Temperature { get; set; }
            public bool IsOn { get; set; }
        }

        public class TestHeatSensorData : IHeatSensorData
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public IPositionData Position { get; set; } = new TestPositionData();
            public float Temperature { get; set; }
        }

        public class TestRoomData : IRoomData
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = "TestRoom";
            public float Width { get; set; } = 5.0f;
            public float Height { get; set; } = 4.0f;

            public IReadOnlyCollection<IHeaterData> Heaters => throw new NotImplementedException();

            public IReadOnlyCollection<IHeatSensorData> HeatSensors => throw new NotImplementedException();

            public IHeaterData AddHeater(IHeaterData heater)
            {
                throw new NotImplementedException();
            }

            public IHeatSensorData AddHeatSensor(IHeatSensorData sensor)
            {
                throw new NotImplementedException();
            }

            public bool ContainsHeater(Guid id)
            {
                throw new NotImplementedException();
            }

            public bool ContainsHeatSensor(Guid id)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IHeaterData? GetHeater(Guid id)
            {
                throw new NotImplementedException();
            }

            public IHeatSensorData? GetHeatSensor(Guid id)
            {
                throw new NotImplementedException();
            }

            public void RemoveHeater(Guid id)
            {
                throw new NotImplementedException();
            }

            public void RemoveHeatSensor(Guid id)
            {
                throw new NotImplementedException();
            }
        }

        [TestInitialize]
        public void Setup()
        {
            _room = new DummyRoomLogic(new TestRoomData { Width = _width, Height = _height });
        }

        [TestMethod]
        public void Room_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_width, _room.Width, 1e-10f);
            Assert.AreEqual(_height, _room.Height, 1e-10f);
            Assert.AreEqual(0, _room.Heaters.Count);
            Assert.AreEqual(0, _room.HeatSensors.Count);
            Assert.AreEqual(0f, _room.AvgTemperature, 1e-10f);
        }

        [TestMethod]
        public void RoomLogic_CanAddAndRemoveHeater()
        {
            var room = new DummyRoomLogic(new TestRoomData());
            var heater = new DummyHeaterLogic(new TestHeaterData());

            var added = room.AddHeater(heater);

            Assert.IsTrue(room.Heaters.Contains(added));

            room.RemoveHeater(added.Id);

            Assert.IsFalse(room.Heaters.Contains(added));
        }

        [TestMethod]
        public void RoomLogic_CanAddAndRemoveHeatSensor()
        {
            var room = new DummyRoomLogic(new TestRoomData());
            var sensor = new DummyHeatSensorLogic(new TestHeatSensorData());

            var added = room.AddHeatSensor(sensor);

            Assert.IsTrue(room.HeatSensors.Contains(added));

            room.RemoveHeatSensor(added.Id);

            Assert.IsFalse(room.HeatSensors.Contains(added));
        }

        [TestMethod]
        public void RoomLogic_ShouldCalculateAverageTemperature()
        {
            var room = new DummyRoomLogic(new TestRoomData());
            room.AddHeatSensor(new DummyHeatSensorLogic(new TestHeatSensorData { Temperature = 20 }));
            room.AddHeatSensor(new DummyHeatSensorLogic(new TestHeatSensorData { Temperature = 22 }));
            room.AddHeatSensor(new DummyHeatSensorLogic(new TestHeatSensorData { Temperature = 24 }));

            Assert.AreEqual(22f, room.AvgTemperature);
        }

        [TestMethod]
        public void RoomLogic_ShouldReturnZeroAvgTemp_WhenNoSensors()
        {
            var room = new DummyRoomLogic(new TestRoomData());

            Assert.AreEqual(0f, room.AvgTemperature);
        }
    };
}
