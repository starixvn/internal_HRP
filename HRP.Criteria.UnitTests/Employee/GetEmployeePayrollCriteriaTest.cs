namespace HRP.Criteria.UnitTests.Employee
{
    using Xunit;

    public class GetEmployeePayrollCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetEmployeePayrollCriteria();
            var expected = "Employee_GetPayroll";

            var actual = criteria.GetSettingKey();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        public void IsValid_ReturnAsExpected(int employeeId, int userId, bool expected)
        {
            var criteria = new GetEmployeePayrollCriteria
            {
                EmployeeId = employeeId,
                UserId = userId
            };

            Assert.Equal(expected, criteria.IsValid());
        }
    }
}