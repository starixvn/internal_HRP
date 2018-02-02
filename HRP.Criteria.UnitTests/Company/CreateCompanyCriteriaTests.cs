namespace HRP.Criteria.UnitTests.Company
{
    using Xunit;

    public class CreateCompanyCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateCompanyCriteria();
            var expected = "Company_Create";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("dar", 0)]
        [InlineData("", 1)]
        [InlineData("", 0)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string code, int createdBy)
        {
            // Arrange
            var criteria = new CreateCompanyCriteria { Code = code, CreatedBy = createdBy };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateCompanyCriteria { Code = "DAR", CreatedBy = 1 };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}