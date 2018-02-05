namespace HRP.Criteria.UnitTests.GradeLevel
{
    using HRP.Criteria.GradeLevel;
    using Xunit;

    public class UpdateGradeLevelCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateGradeLevelCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("GradeLevel_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateGradeLevelCriteria
            {
                GradeLevelId = 1,
                UpdatedBy = 2,
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
        public void IsValid_ParamsAreInvalid_ReturnFalse(string name)
        {
            // Arrange
            var criteria = new UpdateGradeLevelCriteria
            {
                GradeLevelId = 0,
                UpdatedBy = 0,
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}