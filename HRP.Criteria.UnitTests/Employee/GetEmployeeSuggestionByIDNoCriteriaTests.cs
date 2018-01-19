namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmployeeSuggestionByIDNoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeSuggestionByIDNoCriteria();
            var expected = "Employee_GetSuggestionByIDNo";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("")]
        [InlineData(null)]
        public void IsValid_IDNoIsInvalid_ReturnFalse(string idNo)
        {
            // Arrange
            var criteria = new GetEmployeeSuggestionByIDNoCriteria
            {
                IDNo = idNo
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_IDNoIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmployeeSuggestionByIDNoCriteria
            {
                IDNo = "111"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}