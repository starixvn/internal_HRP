namespace HRP.Criteria.UnitTests.Office
{
    using HRP.Criteria.Office;
    using Xunit;

    public class UpdateOfficeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateOfficeCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Office_Update", actual);
        }

        [Theory]
        [InlineData(1, "ahihi")]
        public void IsValid_ParamsAreValid_ReturnTrue(int id, string code)
        {
            // Arrange
            var criteria = new UpdateOfficeCriteria
            {
                OfficeId = id,
                UpdatedBy = id,
                Code = code
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
        public void IsValid_ParamsAreInvalid_ReturnFalse(int id, string code)
        {
            // Arrange
            var criteria = new UpdateOfficeCriteria
            {
                OfficeId = id,
                UpdatedBy = id,
                Code = code
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}