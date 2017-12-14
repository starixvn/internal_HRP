namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class EditUserCriteriaTest
    {
        [Theory]
        [InlineData(0, "myemail@email.com", 1, false)]
        [InlineData(1, "myemail@email.com", 0, false)]
        [InlineData(1, "myemail@email.com", 1, true)]
        [InlineData(1, null, 1, false)]
        public void IsValid_ReturnAsExpected(int userId, string email, int editedBy, bool expected)
        {
            var criteria = new EditUserCriteria
            {
                UserId = userId,
                EditedBy = editedBy,
                Email = email         
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}
