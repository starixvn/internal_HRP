namespace HRP.Criteria.UnitTests.Employee
{
    using System;
    using Criteria.Employee;
    using Xunit;

    public class CheckEmployeePayrollChangedInMonthCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckEmployeePayrollChangedInMonthCriteria();
            var expected = "Employee_CheckEmployeePayrollChangedInMonth";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreInvalid_ReturnFalse()
        {
            // Arrange
            var criteria = new CheckEmployeePayrollChangedInMonthCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnFalse()
        {
            // Arrange
            var criteria = new CheckEmployeePayrollChangedInMonthCriteria { Month = DateTime.Today, EmployeeId = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}