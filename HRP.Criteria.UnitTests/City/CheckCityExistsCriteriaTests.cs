namespace HRP.Criteria.UnitTests.City
{
    using HRP.Criteria.City;
    using Xunit;

    public class CheckCityExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckCityExistsCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("City_CheckExists", actual);
        }

        [Theory]
        [InlineData(0, "ahihi")]
        [InlineData(1, "ahihi")]
        public void IsValid_ParamsAreValid_ReturnTrue(int id, string code)
        {
            // Arrange
            var criteria = new CheckCityExistsCriteria
            {
                CityId = id,
                CountryId = id,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(-1, "   ")]
        [InlineData(-1, "")]
        [InlineData(-1, "asdasd")]
        [InlineData(1, "")]
        [InlineData(0, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string code)
        {
            // Arrange
            var criteria = new CheckCityExistsCriteria
            {
                CityId = id,
                CountryId = id,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}