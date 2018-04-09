namespace HRP.Criteria.UnitTests.Report
{
    using System;
    using Criteria.Report;
    using Xunit;

    public class BankInReportCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new BankInReportCriteria();
            var expected = "Report_GetBankInByMonthAndCompanyId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void IsValid_CompanyId_ReturnExpected(int companyId, bool expectedResult)
        {
            // Arrange
            var criteria = new BankInReportCriteria
            {
                CompanyId = companyId,
                Month = DateTime.Now
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actual);
        }
    }
}
