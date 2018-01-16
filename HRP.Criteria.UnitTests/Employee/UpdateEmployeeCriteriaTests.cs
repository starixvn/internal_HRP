namespace HRP.Criteria.UnitTests.Employee
{
    using AutoFixture;
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
            var fixture = new Fixture();
            var criteria = fixture.Create<UpdateEmployeeCriteria>();

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