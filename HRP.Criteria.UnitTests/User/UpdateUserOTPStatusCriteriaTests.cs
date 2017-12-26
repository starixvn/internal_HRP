namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateUserOTPStatusCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateUserOTPStatus";
            var criteria = new UpdateUserOTPStatusCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_UserIdIsInvalid_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new UpdateUserOTPStatusCriteria
            {
                UserId = userId,
                IsOTPRegistered = true
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateUserOTPStatusCriteria
            {
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}