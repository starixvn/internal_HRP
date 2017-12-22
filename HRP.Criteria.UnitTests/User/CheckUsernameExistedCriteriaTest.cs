namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria;
    using Xunit;

    public class CheckUsernameExistedCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckUsernameExistedCriteria();
            var expected = "User_CheckUsernameExisted";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("admin", true)]
        public void IsValid_ReturnAsExpected(string username, bool expected)
        {
            // Arrange
            var criteria = new CheckUsernameExistedCriteria { Username = username };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
