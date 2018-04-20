namespace HRP.Criteria.UnitTests.EmergencyContact
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HRP.Criteria.EmergencyContact;
    using Xunit;

    public class GetEmergencyContactByEmployeeIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var expected = "EmergencyContact_GetByEmployeeId";
            var criteria = new GetEmergencyContactByEmployeeIdCriteria();
            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(99)]
        public void IsValid_WithValidParams_ReturnTrue(int employeeId)
        {
            // Arrange
            var criteria = new GetEmergencyContactByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-11)]
        public void IsValid_WithInValidParams_ReturnFalse(int employeeId)
        {
            // Arrange
            var criteria = new GetEmergencyContactByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}