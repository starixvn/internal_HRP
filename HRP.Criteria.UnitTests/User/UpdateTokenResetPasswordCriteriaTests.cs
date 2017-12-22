namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateTokenResetPasswordCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateTokenResetPassword";
            var criteria = new UpdateTokenResetPasswordCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(string username, bool expected)
        {
            // Arrange
            var criteria = new UpdateTokenResetPasswordCriteria
            {
                Username = username
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}