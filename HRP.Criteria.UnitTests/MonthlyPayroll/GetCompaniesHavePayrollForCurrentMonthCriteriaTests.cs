namespace HRP.Criteria.UnitTests.MonthlyPayroll
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class GetCompaniesHavePayrollForCurrentMonthCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetCompaniesHavePayrollForCurrentMonthCriteria();
            var expected = "MonthlyPayroll_GetCompaniesHavePayrollForCurrentMonth";

            var actual = criteria.GetSettingKey();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCompaniesHavePayrollForCurrentMonthCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}