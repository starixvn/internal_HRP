namespace HRP.Criteria.UnitTests.Division
{
    using Xunit;

    public class CreateDivisionCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateDivisionCriteria();
            var expected = "Division_Create";

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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int companyId, string code, int createdBy)
        {
            // Arrange
            var criteria = new CreateDivisionCriteria { CompanyId = companyId, Code = code, CreatedBy = createdBy };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateDivisionCriteria { CompanyId = 1, Code = "DAR", CreatedBy = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
            Assert.IsAssignableFrom<CommandBaseCriteria>(criteria);
        }
    }
}