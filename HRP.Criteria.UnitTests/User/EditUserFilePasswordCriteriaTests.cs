namespace HRP.Criteria.UnitTests.User
{
    using HRP.Criteria.User;
    using Xunit;

    public class EditUserFilePasswordCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditUserFilePasswordCriteria();
            var expected = "User_UpdateFilePassword";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnFalse()
        {
            // Arrange
            var criteria = new EditUserFilePasswordCriteria { UserId = 0, Password = "123" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_InvalidPassword_ReturnFalse()
        {
            // Arrange
            var criteria = new EditUserFilePasswordCriteria { UserId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ValidUserIdAndPassword_ReturnTrue()
        {
            // Arrange
            var criteria = new EditUserFilePasswordCriteria { UserId = 1, Password = "123" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}