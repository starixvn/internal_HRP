namespace HRP.Criteria.UnitTests.City
{
    using HRP.Criteria.City;
    using Xunit;

    public class GetCityByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCityByIdCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("City_GetById", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_IdIsValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetCityByIdCriteria
            {
                CityId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_IdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetCityByIdCriteria
            {
                CityId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}