namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class UpdateTokenOTPRegisterCriteriaTests
    {
        private readonly UpdateTokenOTPRegisterCriteria criteria;
        private readonly string settingKey;

        public UpdateTokenOTPRegisterCriteriaTests()
        {
            criteria = new UpdateTokenOTPRegisterCriteria();
            settingKey = "User_UpdateTokenOTPRegister";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string username, bool expected)
        {
            // Arrange
            criteria.TokenOTPRegister = "test";
            criteria.Username = username;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.NotNull(criteria.TokenOTPRegister);
            Assert.Equal(expected, actual);
        }
    }
}