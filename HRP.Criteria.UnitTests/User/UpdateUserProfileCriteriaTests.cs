namespace HRP.Criteria.UnitTests.User
{
    using HRP.Criteria.User;
    using Xunit;

    public class UpdateUserProfileCriteriaTests
    {
        private readonly UpdateUserProfileCriteria criteria;
        private readonly string settingKey;

        public UpdateUserProfileCriteriaTests()
        {
            criteria = new UpdateUserProfileCriteria();
            settingKey = "User_UpdateProfile";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData(0, "", false)]
        [InlineData(-1, "", false)]
        [InlineData(1, "", false)]
        [InlineData(0, null, false)]
        [InlineData(-1, null, false)]
        [InlineData(1, null, false)]
        [InlineData(0, "test", false)]
        [InlineData(-1, "test", false)]
        [InlineData(1, "test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, string email, bool expected)
        {
            // Arrange
            criteria.UserId = userId;
            criteria.Email = email;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}