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
        [InlineData(0, "")]
        [InlineData(1, "")]
        [InlineData(-1, "")]
        [InlineData(0, "test")]
        [InlineData(-1, "test")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, string email)
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
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateUserProfileCriteria
            {
                UserId = 1,
                Email = "test"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}