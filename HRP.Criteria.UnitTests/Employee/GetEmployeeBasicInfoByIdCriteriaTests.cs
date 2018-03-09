namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class GetEmployeeBasicInfoByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeBasicInfoByIdCriteria();
            var expected = "Employee_GetBasicInfoById";

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
            var criteria = new GetEmployeeBasicInfoByIdCriteria
            {
                EmployeeId = employeeId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_EmployeeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmployeeBasicInfoByIdCriteria
            {
                EmployeeId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}