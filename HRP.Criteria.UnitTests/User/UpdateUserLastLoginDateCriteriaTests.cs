namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class UpdateUserLastLoginDateCriteriaTests
    {
        private readonly UpdateUserLastLoginDateCriteria criteria;
        private readonly string settingKey;

        public UpdateUserLastLoginDateCriteriaTests()
        {
            criteria = new UpdateUserLastLoginDateCriteria();
            settingKey = "User_UpdateLastLoginDate";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        [InlineData(1, true)]
        public void IsValid_CheckParams_ReturnAsExpected(int value, bool expected)
        {
            // Arrange
            criteria.UserId = value;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}