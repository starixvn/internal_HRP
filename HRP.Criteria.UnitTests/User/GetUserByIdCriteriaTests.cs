namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByIdCriteriaTests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void IsValid_ReturnAsExpected(int userId, bool expected)
        {
            var criteria = new GetUserByIdCriteria
            {
                UserId = userId
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}