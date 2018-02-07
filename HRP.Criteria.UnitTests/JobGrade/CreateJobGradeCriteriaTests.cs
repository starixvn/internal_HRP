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
            Assert.IsAssignableFrom<CommandBaseCriteria>(criteria);
        }

        [Theory]
        [InlineData(-1, "   ")]
        [InlineData(-1, "")]
        [InlineData(-1, "asdasd")]
        [InlineData(0, "asdasd")]
        [InlineData(1, " ")]
        [InlineData(0, " ")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string name)
        {
            // Arrange
            var criteria = new CreateJobGradeCriteria
            {
                GradeLevelId = id,
                CreatedBy = id,
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}