namespace HRP.Criteria.UnitTests.JobGrade
{
    using HRP.Criteria.JobGrade;
    using Xunit;

    public class CreateJobGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateJobGradeCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("JobGrade_Create", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateJobGradeCriteria
            {
                GradeLevelId = 1,
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
            var criteria = new CreateJobGradeCriteria
            {
                GradeLevelId = 0,
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