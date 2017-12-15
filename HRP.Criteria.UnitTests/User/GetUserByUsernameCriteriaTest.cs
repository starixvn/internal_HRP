namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByUsernameCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUserByUsernameCriteria();
            var expected = "User_GetByUsername";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("abc", true)]
        public void IsValid_ReturnAsExpected(string username, bool expected)
        {
            // Arrange
            var criteria = new GetUserByUsernameCriteria
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