namespace HRP.Criteria.UnitTests.Division
{
    using Xunit;

    public class UpdateDivisionCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateDivisionCriteria();
            var expected = "Division_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "dar", 0)]
        [InlineData(1, "", 1)]
        [InlineData(1, "", 0)]
        [InlineData(0, "dar", 1)]
        [InlineData(0, "dar", 0)]
        [InlineData(0, "", 1)]
        [InlineData(0, "", 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int divisionId, string code, int updatedBy)
        {
            // Arrange
            var criteria = new UpdateDivisionCriteria { DivisionId = divisionId, Code = code, UpdatedBy = updatedBy };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateDivisionCriteria { DivisionId = 1, Code = "DAR", UpdatedBy = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.IsAssignableFrom<CommandBaseCriteria>(criteria);
        }
    }
}