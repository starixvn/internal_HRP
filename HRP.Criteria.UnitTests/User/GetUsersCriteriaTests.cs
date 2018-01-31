namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUsersCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUsersCriteria();
            var expected = "User_GetUsers";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUsersCriteria
            {
                IsActive = true
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.True(criteria.IsActive);
        }
    }
}