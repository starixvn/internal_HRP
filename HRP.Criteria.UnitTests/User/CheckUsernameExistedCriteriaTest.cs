namespace HRP.Criteria.UnitTests.User
{
    using HRP.Criteria;
    using Xunit;

    public class CheckUsernameExistedCriteriaTest
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("admin", true)]
        public void IsValid_ReturnAsExpected(string username, bool expected)
        {
            var criteria = new CheckUsernameExistedCriteria { Username = username };

            var actual = criteria.IsValid();
            Assert.Equal(expected, actual);
        }
    }
}
