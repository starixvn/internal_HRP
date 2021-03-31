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
            Assert.IsAssignableFrom<BaseNonQueryCommand>(criteria);
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
            var criteria = new UpdateJobGradeCriteria
            {
                JobGradeId = id,
                UpdatedBy = id,
                Name = name
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}