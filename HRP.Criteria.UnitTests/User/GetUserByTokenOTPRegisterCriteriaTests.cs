namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByTokenOTPRegisterCriteriaTests
    {
        private readonly GetUserByTokenOTPRegisterCriteria criteria;
        private readonly string settingKey;

        public GetUserByTokenOTPRegisterCriteriaTests()
        {
            criteria = new GetUserByTokenOTPRegisterCriteria();
            settingKey = "User_GetByTokenOTPRegister";
        }

        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            var actual = criteria.GetSettingKey();
            Assert.Equal(settingKey, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string value, bool expected)
        {
            // Arrange
            criteria.TokenOTPRegister = value;

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}