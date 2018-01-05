namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmploymentByEmployeeIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmploymentByEmployeeIdCriteria();
            var expected = "Employment_GetByEmployeeId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_EmployeeIdIsInvalid_ReturnFalse(int employeeId)
        {
            // Arrange
            var criteria = new GetEmploymentByEmployeeIdCriteria { EmployeeId = employeeId };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_EmployeeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmploymentByEmployeeIdCriteria { EmployeeId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}