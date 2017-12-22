namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetUserByTokenOTPRegisterCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_GetByTokenOTPRegister";
            var criteria = new GetUserByTokenOTPRegisterCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string token, bool expected)
        {
            // Arrange
            var criteria = new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = token
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}