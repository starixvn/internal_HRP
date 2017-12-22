namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateTokenOTPRegisterCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateTokenOTPRegister";
            var criteria = new UpdateTokenOTPRegisterCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string username, bool expected)
        {
            // Arrange
            var criteria = new UpdateTokenOTPRegisterCriteria
            {
                TokenOTPRegister = "test",
                Username = username
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.NotNull(criteria.TokenOTPRegister);
            Assert.Equal(expected, actual);
        }
    }
}