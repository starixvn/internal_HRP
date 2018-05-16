namespace HRP.Criteria.UnitTests.Report
{
    using HRP.Criteria.Report;
    using Xunit;

    public class GetEmployeeDataForDynamicReportCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeDataForDynamicReportCriteria();
            var expected = "Report_GetEmployeeDataForDynamicReport";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "")]
        [InlineData(0, "1,2")]
        [InlineData(0, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int companyId, string employeeStatusIds)
        {
            // Arrange
            var criteria = new GetEmployeeDataForDynamicReportCriteria
            {
                CompanyId = companyId,
                EmployeeStatusIds = employeeStatusIds
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetEmployeeDataForDynamicReportCriteria
            {
                CompanyId = 1,
                EmployeeStatusIds = "1,2"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}