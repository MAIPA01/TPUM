namespace TPUM.Data.Tests
{
    [TestClass]
    public sealed class PositionTest
    {
        private Position _position;
        private const float _x = 2f;
        private const float _y = 2f;

        [TestInitialize]
        public void Setup()
        {
            _position = new(_x, _y);
        }

        [TestMethod]
        public void Position_ShouldInitialize_WithCorrectProperties()
        {
            Assert.AreEqual(_x, _position.X, 1e-10f);
            Assert.AreEqual(_y, _position.Y, 1e-10f);
        }

        [TestMethod]
        public void XProperty_ShouldUpdateCorrectly()
        {
            _position.X += 1f;
            Assert.AreEqual(_x + 1f, _position.X, 1e-10f);
        }

        [TestMethod]
        public void YProperty_ShouldUpdateCorrectly()
        {
            _position.Y += 1f;
            Assert.AreEqual(_y + 1f, _position.Y, 1e-10f);
        }

        [TestMethod]
        public void Distance_ShouldCalculateCorrectly()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 1f);
            Assert.AreEqual(MathF.Sqrt(2f), IPosition.Distance(pos1, pos2), 1e-10f);
            Assert.AreEqual(MathF.Sqrt(2f), IPosition.Distance(pos2, pos1), 1e-10f);
        }

        [TestMethod]
        public void EqualOperator_ShouldReturnFalseForTwoPositionsWithDiffrentX()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 2f);
            Assert.IsFalse(pos1 == pos2);
            Assert.IsFalse(pos2 == pos1);
        }

        [TestMethod]
        public void EqualOperator_ShouldReturnFalseForTwoPositionsWithDiffrentY()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(2f, 1f);
            Assert.IsFalse(pos1 == pos2);
            Assert.IsFalse(pos2 == pos1);
        }

        [TestMethod]
        public void EqualOperator_ShouldReturnFalseForTwoDiffrentPositions()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 1f);
            Assert.IsFalse(pos1 == pos2);
            Assert.IsFalse(pos2 == pos1);
        }

        [TestMethod]
        public void EqualOperator_ShouldReturnTrueForTwoSamePositions()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(2f, 2f);
            Assert.IsTrue(pos1 == pos2);
            Assert.IsTrue(pos2 == pos1);
        }

        [TestMethod]
        public void EqualOperator_ShouldReturnTrueForTheSamePosition()
        {
            Assert.AreSame(_position, _position);
            Assert.IsTrue(_position == _position);
        }

        [TestMethod]
        public void NotEqualOperator_ShouldReturnTrueForTwoPositionsWithDiffrentX()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 2f);
            Assert.IsTrue(pos1 != pos2);
            Assert.IsTrue(pos2 != pos1);
        }

        [TestMethod]
        public void NotEqualOperator_ShouldReturnTrueForTwoPositionsWithDiffrentY()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(2f, 1f);
            Assert.IsTrue(pos1 != pos2);
            Assert.IsTrue(pos2 != pos1);
        }

        [TestMethod]
        public void NotEqualOperator_ShouldReturnTrueForTwoDiffrentPositions()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 1f);
            Assert.IsTrue(pos1 != pos2);
            Assert.IsTrue(pos2 != pos1);
        }

        [TestMethod]
        public void NotEqualOperator_ShouldReturnFalseForTwoSamePositions()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(2f, 2f);
            Assert.IsFalse(pos1 != pos2);
            Assert.IsFalse(pos2 != pos1);
        }

        [TestMethod]
        public void NotEqualOperator_ShouldReturnFalseForTheSamePosition()
        {
            Assert.AreSame(_position, _position);
            Assert.IsFalse(_position != _position);
        }

        [TestMethod]
        public void EqualsMethod_ShouldReturnFalseForTwoPositionsWithDiffrentX()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 2f);
            Assert.IsFalse(pos1.Equals(pos2));
            Assert.IsFalse(pos2.Equals(pos1));
        }

        [TestMethod]
        public void EqualsMethod_ShouldReturnFalseForTwoPositionsWithDiffrentY()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(2f, 1f);
            Assert.IsFalse(pos1.Equals(pos2));
            Assert.IsFalse(pos2.Equals(pos1));
        }

        [TestMethod]
        public void EqualsMethod_ShouldReturnFalseForTwoDiffrentPositions()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(1f, 1f);
            Assert.IsFalse(pos1.Equals(pos2));
            Assert.IsFalse(pos2.Equals(pos1));
        }

        [TestMethod]
        public void EqualsMethod_ShouldReturnTrueForTwoSamePositions()
        {
            Position pos1 = new(2f, 2f);
            Position pos2 = new(2f, 2f);
            Assert.IsTrue(pos1.Equals(pos2));
            Assert.IsTrue(pos2.Equals(pos1));
        }

        [TestMethod]
        public void EqualsMethod_ShouldReturnTrueForTheSamePosition()
        {
            Assert.AreSame(_position, _position);
            Assert.IsTrue(_position.Equals(_position));
        }

        [TestMethod]
        public void EqualsMethod_ShouldReturnFalseForPositionAndNewObject()
        {
            Assert.IsFalse(_position.Equals(new object()));
        }

        [TestMethod]
        public void GetHashCode_ShouldReturnConsistentValue()
        {
            var hash = 3 * _position.X.GetHashCode() + 5 * _position.Y.GetHashCode();
            Assert.AreEqual(hash, _position.GetHashCode(), 1);
        }

        [TestMethod]
        public void OnPositionChanged_ShouldTriggerEvent()
        {
            bool eventTriggered = false;

            _position.PositionChanged += (sender, args) =>
            {
                eventTriggered = true;
                Assert.AreEqual(_x, args.LastPosition.X, 1e-10f);
                Assert.AreEqual(_y, args.LastPosition.Y, 1e-10f);
                Assert.AreEqual(3f, args.NewPosition.X, 1e-10f);
                Assert.AreEqual(_y, args.NewPosition.Y, 1e-10f);
            };

            _position.X = 3f;

            Assert.IsTrue(eventTriggered, "PositionChanged event was not triggered.");
        }
    }
}