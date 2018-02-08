namespace HRP.Criteria.UnitTests.Country
{
    using HRP.Criteria.Country;
    using Xunit;

    public class GetCountryByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetCountryByIdCriteria();
            var expected = "Country_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_JobGradeIdIsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCountryByIdCriteria
            {
                CountryId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_JobGradeIdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetCountryByIdCriteria
            {
                CountryId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}