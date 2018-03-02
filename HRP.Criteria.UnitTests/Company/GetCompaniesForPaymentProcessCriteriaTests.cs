namespace HRP.Criteria.UnitTests.Company
{
    using Xunit;

    public class GetCompaniesForPaymentProcessCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new GetCompaniesForPaymentProcessCriteria();
            var expected = "Company_GetCompaniesForPaymentProcess";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ValidUserId_ReturnTrue()
        {
            var criteria = new GetCompaniesForPaymentProcessCriteria { UserId = 1};

            var actual = criteria.IsValid();
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_InvalidUserId_ReturnTrue()
        {
            var criteria = new GetCompaniesForPaymentProcessCriteria();

            var actual = criteria.IsValid();
            Assert.False(actual);
        }
    }
}
