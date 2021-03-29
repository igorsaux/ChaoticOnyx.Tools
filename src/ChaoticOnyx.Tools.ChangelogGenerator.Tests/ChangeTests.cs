#region

using Xunit;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class ChangeTests
    {
        [Fact]
        public void ChangesRefsEqualsTest()
        {
            // Arrange
            var change1 = new Change();
            var change2 = change1;

            // Act
            var result = change1 == change2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ChangesDoesNotEqualToNullTest()
        {
            // Arrange
            var change1 = new Change();

            // Act
            var result = change1 != null;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ObjectRefsToChangeEqualsTest()
        {
            // Arrange
            var    change1 = new Change();
            object change2 = change1;

            // Act
            var result = change1 == change2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void NullObjectRefsAndChangeNotEqualsTest()
        {
            // Arrange
            var    change1 = new Change();
            object change2 = null;

            // Act
            var result = change1.Equals(change2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ObjectWithDifferentTypeNotEqualsToChangeTest()
        {
            // Arrange
            var    change1 = new Change();
            object change2 = new Changelog();

            // Act
            var result = change1.Equals(change2);

            // Assert
            Assert.False(result);
        }
    }
}
