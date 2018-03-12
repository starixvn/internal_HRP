namespace HRP.Criteria.UnitTests.MonthlyPayroll
{
    using HRP.Criteria.MonthlyPayroll;
    using Xunit;

    public class GetMonthlyAllowanceCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetMonthlyAllowanceCriteria();
            var expected = "MonthlyPayroll_GetAllowances";

            var actual = criteria.GetSettingKey();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ValidId_ReturnTrue()
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = 1 };

            var actual = criteria.IsValid();

            Assert.True(actual);
        }

        [Fact]
        public void IsValid_InvalidId_ReturnTrue()
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = 0 };

            var actual = criteria.IsValid();

            Assert.False(actual);
        }
    }
}