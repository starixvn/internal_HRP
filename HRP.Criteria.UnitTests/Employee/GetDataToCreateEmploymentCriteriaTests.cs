namespace HRP.Criteria.UnitTests.Employee
{
    using Criteria.Employee;
    using Xunit;

    public class GetDataToCreateEmploymentCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataToCreateEmploymentCriteria();
            var expected = "Employment_GetDataToCreate";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_UserIdIsInvalid_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new GetDataToCreateEmploymentCriteria
            {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataToCreateEmploymentCriteria
            {
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}