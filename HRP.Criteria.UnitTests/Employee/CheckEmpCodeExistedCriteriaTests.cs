namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class CheckEmpCodeExistedCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckEmpCodeExistedCriteria();
            var expected = "Employee_CheckEmpCodeExisted";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "dar", "")]
        [InlineData(1, "", "1")]
        [InlineData(1, "", "")]
        [InlineData(0, "dar", "")]
        [InlineData(0, "", "1")]
        [InlineData(0, "", "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int employeeId, string companyCode, string employeeCode)
        {
            // Arrange
            var criteria = new CheckEmpCodeExistedCriteria { EmployeeId = employeeId, CompanyCode = companyCode, EmployeeCode = employeeCode };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void IsValid_ParamsAreValid_ReturnTrue(int employeeId)
        {
            // Arrange
            var criteria = new CheckEmpCodeExistedCriteria { EmployeeId = employeeId, CompanyCode = "DAR", EmployeeCode = "123456" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}