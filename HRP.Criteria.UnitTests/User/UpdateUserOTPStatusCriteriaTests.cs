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
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, bool expected)
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
            Assert.True(criteria.IsOTPRegistered);
            Assert.Equal(expected, actual);
        }
    }
}