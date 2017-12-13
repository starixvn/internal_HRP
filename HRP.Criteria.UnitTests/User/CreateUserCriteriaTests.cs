namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class CreateUserCriteriaTests
    {
        [Theory]
        [InlineData(null, "password", "myemail@email.com", false)]
        [InlineData("username", null, "myemail@email.com", false)]
        [InlineData("username", "password", null, false)]
        [InlineData("username", "password", "myemail@email.com", true)]
        public void IsValid_ReturnAsExpected(string username, string password, string email, bool expected)
        {
            var criteria = new CreateUserCriteria
            {
                Username = username,
                Password = password,
                Email = email
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}
