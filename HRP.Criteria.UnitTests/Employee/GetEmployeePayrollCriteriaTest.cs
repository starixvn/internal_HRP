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

        [Fact]
        public void IsValid_ValidData_ReturnTrue()
        {
            var criteria = new GetEmployeePayrollCriteria
            {
                EmployeeId = 1,
                UserId = 1
            };

            Assert.True(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidEmployeeId_ReturnFalse()
        {
            var criteria = new GetEmployeePayrollCriteria
            {
                EmployeeId = 0,
                UserId = 1
            };

            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnFalse()
        {
            var criteria = new GetEmployeePayrollCriteria
            {
                EmployeeId = 1,
                UserId = 0
            };

            Assert.False(criteria.IsValid());
        }
    }
}