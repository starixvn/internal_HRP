namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetCostCenterBySectionIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCostCenterBySectionIdCriteria();
            var expected = "CostCenter_GetBySectionId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_SectionIdIdIsInvalid_ReturnFalse(int sectionId)
        {
            // Arrange
            var criteria = new GetCostCenterBySectionIdCriteria
            {
                SectionId = sectionId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_SectionIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCostCenterBySectionIdCriteria
            {
                SectionId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}