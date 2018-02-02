namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class UpdateUserPasswordCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdatePassword";
            var criteria = new UpdateUserPasswordCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "")]
        [InlineData(-1, "")]
        [InlineData(0, "test")]
        [InlineData(-1, "test")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, string password)
        {
            // Arrange
            var criteria = new UpdateUserPasswordCriteria
            {
                UserId = userId,
                Password = password
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateUserPasswordCriteria
            {
                UserId = 1,
                Password = "password"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}