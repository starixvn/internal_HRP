namespace HRP.Criteria.UnitTests.Office
{
    using HRP.Criteria.Office;
    using Xunit;

    public class CheckOfficeExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckOfficeExistsCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Office_CheckExists", actual);
        }

        [Theory]
        [InlineData(0, "ahihi")]
        [InlineData(1, "ahihi")]
        public void IsValid_ParamsAreValid_ReturnTrue(int id, string code)
        {
            // Arrange
            var criteria = new CheckOfficeExistsCriteria
            {
                CityId = id,
                OfficeId = id,
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
            var criteria = new CheckOfficeExistsCriteria
            {
                CityId = id,
                OfficeId = id,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}