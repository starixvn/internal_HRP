namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class GetCityByCountryIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCityByCountryIdCriteria();
            var expected = "City_GetByCountryId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_CountryIdIsInvalid_ReturnFalse(int countryId)
        {
            // Arrange
            var criteria = new GetCityByCountryIdCriteria
            {
                CountryId = countryId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_CountryIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCityByCountryIdCriteria
            {
                CountryId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}