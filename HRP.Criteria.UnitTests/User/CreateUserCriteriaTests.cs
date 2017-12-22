namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class CreateUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateUserCriteria();
            var expected = "User_CreateUser";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "password", "myemail@email.com", false)]
        [InlineData("username", null, "myemail@email.com", false)]
        [InlineData("username", "password", null, false)]
        [InlineData("username", "password", "myemail@email.com", true)]
        public void IsValid_ReturnAsExpected(string username, string password, string email, bool expected)
        {
            // Arrange
            var criteria = new CreateUserCriteria
            {
                Username = username,
                Password = password,
                Email = email
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}