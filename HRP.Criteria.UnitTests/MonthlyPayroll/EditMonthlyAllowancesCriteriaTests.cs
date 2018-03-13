namespace HRP.Criteria.UnitTests.MonthlyPayroll
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class EditMonthlyAllowancesCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new EditMonthlyAllowancesCriteria();
            var expected = "MonthlyPayroll_UpdateAllowances";

            var actual = criteria.GetSettingKey();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ValidData_ReturnTrue()
        {
            var criteria = new EditMonthlyAllowancesCriteria { PayrollId = 1, UserId = 1 };

            var actual = criteria.IsValid();

            Assert.True(actual);
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnTrue()
        {
            var criteria = new EditMonthlyAllowancesCriteria { PayrollId = 1 };

            var actual = criteria.IsValid();

            Assert.False(actual);
        }

        [Fact]
        public void IsValid_InvalidPayrollId_ReturnTrue()
        {
            var criteria = new EditMonthlyAllowancesCriteria { UserId = 1 };

            var actual = criteria.IsValid();

            Assert.False(actual);
        }
    }
}