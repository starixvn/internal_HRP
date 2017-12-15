namespace HRP.Criteria.UnitTests.User
{
    using Xunit;

    public class EditUserCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new EditUserCriteria();
            var expected = "User_EditUser";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "myemail@email.com", 1, false)]
        [InlineData(1, "myemail@email.com", 0, false)]
        [InlineData(1, "myemail@email.com", 1, true)]
        [InlineData(1, null, 1, false)]
        public void IsValid_ReturnAsExpected(int userId, string email, int editedBy, bool expected)
        {
            // Arrange
            var criteria = new EditUserCriteria
            {
                UserId = userId,
                EditedBy = editedBy,
                Email = email
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}