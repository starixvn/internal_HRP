namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmployeeSuggestionByNameCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeSuggestionByNameCriteria();
            var expected = "Employee_GetSuggestionByName";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(0, "a")]
        [InlineData(1, "")]
        [InlineData(1, "a")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int userId, string name)
        {
            // Arrange
            var criteria = new GetEmployeeSuggestionByNameCriteria { UserId = userId, Name = name };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmployeeSuggestionByNameCriteria { UserId = 1, Name = "DAR" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}