namespace HRP.Criteria.UnitTests.Office
{
    using HRP.Criteria.Office;
    using Xunit;

    public class GetOfficeByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetOfficeByIdCriteria();

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Office_GetById", actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_IdIsValid_ReturnTrue(int id)
        {
            // Arrange
            var criteria = new GetOfficeByIdCriteria
            {
                OfficeId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0)]
        public void IsValid_IdIsInvalid_ReturnFalse(int id)
        {
            // Arrange
            var criteria = new GetOfficeByIdCriteria
            {
                OfficeId = id
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}