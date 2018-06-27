namespace HRP.Criteria.UnitTests.User
{
    using HRP.Criteria.User;
    using Xunit;

    public class GetUserFilePasswordCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUserFilePasswordCriteria();
            var expected = "User_GetFilePassword";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnFalse()
        {
            // Arrange
            var criteria = new GetUserFilePasswordCriteria { UserId = 0 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ValidUserId_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUserFilePasswordCriteria { UserId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}