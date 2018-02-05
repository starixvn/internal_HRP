namespace HRP.Criteria.UnitTests.CostCenter
{
    using HRP.Criteria.CostCenter;
    using Xunit;

    public class GetAllCostCenterCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllCostCenterCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("CostCenter_GetAll", actual);
        }

        [Fact]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetAllCostCenterCriteria
            {
                IsActive = true
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}