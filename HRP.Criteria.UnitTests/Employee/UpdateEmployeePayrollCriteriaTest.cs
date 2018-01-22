namespace HRP.Criteria.UnitTests.Employee
{
    using Xunit;

    public class UpdateEmployeePayrollCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new UpdateEmployeePayrollCriteria();
            var expected = "Employee_UpdatePayroll";

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