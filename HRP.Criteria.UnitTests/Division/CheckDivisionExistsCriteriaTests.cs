namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class CheckDivisionExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckDivisionExistsCriteria();
            var expected = "Division_CheckExists";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 1, "")]
        [InlineData(1, 0, "")]
        [InlineData(0, 0, "")]
        [InlineData(0, -1, "dar")]
        [InlineData(-1, 0, "dar")]
        [InlineData(-1, -1, "dar")]
        [InlineData(-1, 0, "")]
        [InlineData(-1, -1, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int divisionId, int companyId, string code)
        {
            // Arrange
            var criteria = new CheckDivisionExistsCriteria { DivisionId = divisionId, CompanyId = companyId, Code = code };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public void IsValid_ParamsAreValid_ReturnTrue(int divisionId, int companyId)
        {
            // Arrange
            var criteria = new CheckDivisionExistsCriteria { DivisionId = divisionId, CompanyId = companyId, Code = "DAR" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}