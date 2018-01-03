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
        [InlineData(null, null)]
        [InlineData("", "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string companyCode, string employeeCode)
        {
            // Arrange
            var criteria = new CheckEmpCodeExistedCriteria { CompanyCode = companyCode, EmployeeCode = employeeCode };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CheckEmpCodeExistedCriteria { CompanyCode = "DAR", EmployeeCode = "123456" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}