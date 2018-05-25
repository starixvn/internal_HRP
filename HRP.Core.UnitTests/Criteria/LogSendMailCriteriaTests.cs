namespace HRP.Core.UnitTests.Criteria
{
    using HRP.Core.Criteria;
    using Xunit;

    public class LogSendMailCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new LogSendMailCriteria();
            var expected = "MailRecord_Create";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new LogSendMailCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}