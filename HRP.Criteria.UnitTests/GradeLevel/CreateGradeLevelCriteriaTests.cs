namespace HRP.Criteria.UnitTests.GradeLevel
{
    using HRP.Criteria.GradeLevel;
    using Xunit;

    public class CreateGradeLevelCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateGradeLevelCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("GradeLevel_Create", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateGradeLevelCriteria
            {
                JobCategoryId = 1,
                CreatedBy = 2,
                Name = "aa"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string code)
        {
            // Arrange
            var criteria = new CreateGradeLevelCriteria
            {
                JobCategoryId = 0,
                CreatedBy = 0,
                Name = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}