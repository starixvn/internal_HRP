namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByUsernameCriteriaTest
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("abc", true)]
        public void IsValid_ReturnAsExpected(string username, bool expected)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}