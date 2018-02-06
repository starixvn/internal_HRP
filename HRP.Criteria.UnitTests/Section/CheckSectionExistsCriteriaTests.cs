namespace HRP.Criteria.UnitTests.Section
{
    using HRP.Criteria.Section;
    using Xunit;

    public class CheckSectionExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckSectionExistsCriteria();
            var expected = "Section_CheckExists";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CheckSectionExistsCriteria
            {
                Code = "aw"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnTrue(string code)
        {
            // Arrange
            var criteria = new CheckSectionExistsCriteria
            {
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}