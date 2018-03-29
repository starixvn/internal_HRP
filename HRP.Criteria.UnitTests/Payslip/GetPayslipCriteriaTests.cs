namespace HRP.Criteria.UnitTests.Payslip
{
    using HRP.Criteria.Payslip;
    using Xunit;

    public class GetPayslipCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPayslipCriteria();
            var expected = "Payslip_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_UserIdIsInvalid_ReturnFalse(int userId)
        {
            // Arrange
            var criteria = new GetPayslipCriteria
            {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_UserIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetPayslipCriteria
            {
                UserId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}