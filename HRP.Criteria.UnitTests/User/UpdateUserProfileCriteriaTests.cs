namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.User;
    using Xunit;

    public class UpdateUserProfileCriteriaTests
    {
        [Fact]
        public virtual void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "User_UpdateProfile";
            var criteria = new UpdateUserProfileCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "", false)]
        [InlineData(1, "", false)]
        [InlineData(0, "test", false)]
        [InlineData(1, "test", true)]
        public void IsValid_CheckParams_ReturnAsExpected(int userId, string email, bool expected)
        {
            // Arrange
            var criteria = new UpdateUserProfileCriteria
            {
                UserId = userId,
                Email = email
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}