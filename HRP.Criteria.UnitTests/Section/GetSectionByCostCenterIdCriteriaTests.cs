namespace HRP.Criteria.UnitTests.Section
{
    using Xunit;

    public class GetSectionByCostCenterIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetSectionByCostCenterIdCriteria();
            var expected = "Section_GetByCostCenterId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_CostCenterIdIsInvalid_ReturnFalse(int costCenterId)
        {
            // Arrange
            var criteria = new GetSectionByCostCenterIdCriteria
            {
                CostCenterId = costCenterId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_CostCenterIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetSectionByCostCenterIdCriteria
            {
                CostCenterId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}