namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetUserByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetUserByIdCriteria();
            var expected = "User_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void IsValid_ReturnAsExpected(int userId, bool expected)
        {
            // Arrange
            var criteria = new GetUserByIdCriteria
            {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}