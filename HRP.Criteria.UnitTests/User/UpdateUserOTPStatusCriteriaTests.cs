namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class UpdateUserOTPStatusCriteriaTests
    {
        private readonly UpdateUserOTPStatusCriteria criteria;
        private readonly string settingKey;

        public UpdateUserOTPStatusCriteriaTests()
        {
            criteria = new UpdateUserOTPStatusCriteria();
            criteria.IsOTPRegistered = true;
            settingKey = "User_UpdateUserOTPStatus";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, bool expected)
        {
            // Arrange
            criteria.UserId = userId;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(criteria.IsOTPRegistered);
            Assert.Equal(expected, actual);
        }
    }
}