namespace HRP.Criteria.UnitTests.Employee
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class UpdateEmployeeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "Employee_UpdateEmployee";
            var criteria = new UpdateEmployeeCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WithValidParams_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateEmployeeCriteria
            {
                EmployeeId = 1,
                Gender = 1,
                Nationality = 1,
                MaritalStatus = 1,
                NoOfChildren = 1,
                FullName = "tim",
                Nickname = "tim.trinh",
                OverseasPhoneNo = "0123",
                NRICNo = "0123",
                PassportNo = "0123",
                PermanentAddress = "test",
                ResidenceAddress = "test"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WithInvalidParams_ReturnFalse()
        {
            // Arrange
            var criteria = new UpdateEmployeeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}