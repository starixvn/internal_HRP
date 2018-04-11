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
    }
}
