namespace HRP.Criteria.UnitTests.JobGrade
{
    using HRP.Criteria.JobGrade;
    using Xunit;

    public class UpdateJobGradeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateJobGradeCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("JobGrade_Update", actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateJobGradeCriteria
            {
                JobGradeId = 1,
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
            var criteria = new UpdateJobGradeCriteria
            {
                JobGradeId = 0,
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