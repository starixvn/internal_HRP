namespace HRP.Criteria.UnitTests
{
    using Xunit;

    public class CheckCompanyExistsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CheckCompanyExistsCriteria();
            var expected = "Company_CheckExists";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "")]
        [InlineData(0, "")]
        [InlineData(-1, "dar")]
        [InlineData(-1, "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int companyId, string code)
        {
            // Arrange
            var criteria = new CheckCompanyExistsCriteria { CompanyId = companyId, Code = code };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void IsValid_ParamsAreValid_ReturnTrue(int companyId)
        {
            // Arrange
            var criteria = new CheckCompanyExistsCriteria { CompanyId = companyId, Code = "DAR" };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}