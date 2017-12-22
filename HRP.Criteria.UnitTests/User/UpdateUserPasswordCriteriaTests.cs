namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class UpdateUserPasswordCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdatePassword";
            var criteria = new UpdateUserPasswordCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "", false)]
        [InlineData(1, "", false)]
        [InlineData(0, "test", false)]
        [InlineData(1, "test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, string password, bool expected)
        {
            // Arrange
            var criteria = new UpdateUserPasswordCriteria
            {
                UserId = userId,
                Password = password
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}