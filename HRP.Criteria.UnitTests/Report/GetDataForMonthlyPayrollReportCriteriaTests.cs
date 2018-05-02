namespace HRP.Criteria.UnitTests.Report
{
    using HRP.Criteria.Report;
    using Xunit;

    public class GetDataForMonthlyPayrollReportCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataForMonthlyPayrollReportCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Report_GetDataForMonthlyPayrollReport", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_ParamsAreValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetDataForMonthlyPayrollReportCriteria
            {
                CompanyId = id,
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetDataForMonthlyPayrollReportCriteria
            {
                CompanyId = id,
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}