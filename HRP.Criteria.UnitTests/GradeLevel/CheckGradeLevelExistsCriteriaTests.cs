namespace HRP.Criteria.UnitTests.GradeLevel
{
    using HRP.Criteria.GradeLevel;
    using Xunit;

    public class CheckGradeLevelExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckGradeLevelExistsCriteria();
            var expected = "GradeLevel_CheckExists";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CheckGradeLevelExistsCriteria
            {
                Name = "aw"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnTrue(string name)
        {
            // Arrange
            var criteria = new CheckGradeLevelExistsCriteria
            {
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}