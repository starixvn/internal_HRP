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

        [Fact]
        public void IsValid_ValidData_ReturnAsExpected()
        {
            var criteria = new UpdateEmployeePayrollCriteria
            {
                EmployeeId = 1,
                UpdatedBy = 1
            };

            Assert.True(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidEmployeeId_ReturnFalse()
        {
            var criteria = new UpdateEmployeePayrollCriteria
            {
                EmployeeId = 0,
                UpdatedBy = 1
            };

            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnFalse()
        {
            var criteria = new UpdateEmployeePayrollCriteria
            {
                EmployeeId = 1,
                UpdatedBy = 0
            };

            Assert.False(criteria.IsValid());
        }
    }
}