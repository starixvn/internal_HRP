namespace HRP.Criteria.UnitTests.CostCenter
{
    using HRP.Criteria.CostCenter;
    using Xunit;

    public class GetCostCenterByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCostCenterByIdCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("CostCenter_GetById", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_CostCenterIdIsValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetCostCenterByIdCriteria
            {
                CostCenterId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_CostCenterIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetCostCenterByIdCriteria
            {
                CostCenterId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}