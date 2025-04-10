using System;
using Xunit;
using static Engine.Items.ChecklistExtensions;

namespace Engine.Tests.Unit {
    // Ensures Position works correctly
    public class PositionTest {
        [Fact]
        public void PositionFromString() {
            AssertValid("P[0]");
            AssertValid("P[1.2.3.4]");
            AssertValid("P[1.24.3.4]");
            AssertValid($" \n P[1.24.3.4] \n ");
            AssertValid("p[0.1.2]");
            AssertInvalid("P[]");
            AssertInvalid("P[.0.1.2]");
            AssertInvalid("P[0.1.2.]");
        }

        private void AssertValid(string positionRepresentation) {
            var position = positionRepresentation.ToPosition();
            Assert.Equal(positionRepresentation.Trim().ToUpper(), position.ToString());
        }

        private void AssertInvalid(string positionRepresentation) {
            Assert.Throws<ArgumentException>(() => positionRepresentation.ToPosition());
        }
    }
}