namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class GetUserByTokenResetPasswordCriteriaTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("abc", true)]
        public void IsValid_ReturnAsExpected(string token, bool expected)
        {
            var criteria = new GetUserByTokenResetPasswordCriteria
            {
                TokenResetPassword = token
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}