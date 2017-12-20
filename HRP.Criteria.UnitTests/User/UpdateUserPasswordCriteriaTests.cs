namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class UpdateUserPasswordCriteriaTests
    {
        private readonly UpdateUserPasswordCriteria criteria;
        private readonly string settingKey;

        public UpdateUserPasswordCriteriaTests()
        {
            criteria = new UpdateUserPasswordCriteria();
            settingKey = "User_UpdatePassword";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData(0, "", false)]
        [InlineData(1, "", false)]
        [InlineData(-1, "", false)]
        [InlineData(0, null, false)]
        [InlineData(-1, null, false)]
        [InlineData(1, null, false)]
        [InlineData(0, "test", false)]
        [InlineData(-1, "test", false)]
        [InlineData(1, "test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, string password, bool expected)
        {
            // Arrange
            criteria.UserId = userId;
            criteria.Password = password;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}