namespace HRP.Criteria.UnitTests.Office
{
    using Xunit;

    public class GetOfficeByCityIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetOfficeByCityIdCriteria();
            var expected = "Office_GetByCityId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_CityIdIsInvalid_ReturnFalse(int cityId)
        {
            // Arrange
            var criteria = new GetOfficeByCityIdCriteria
            {
                CityId = cityId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_CityIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetOfficeByCityIdCriteria
            {
                CityId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}