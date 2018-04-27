namespace HRP.Criteria.UnitTests.Employee
{
    using Criteria.Employee;
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
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int employeeId)
        {
            // Arrange
            var criteria = new GetEmploymentByEmployeeIdCriteria { EmployeeId = employeeId };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
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