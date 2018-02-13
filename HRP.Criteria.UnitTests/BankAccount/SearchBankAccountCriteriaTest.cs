namespace HRP.Criteria.UnitTests.BankAccount
{
    using Criteria.BankAccount;
    using Xunit;

    public class SearchBankAccountCriteriaTest
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            var criteria = new SearchBankAccountCriteria();
            var expected = "BankAccount_Search";

            var actual = criteria.GetSettingKey();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ValidData_ReturnTrue()
        {
            var criteria = new SearchBankAccountCriteria
            {
               BankTypeId = 1,
               EmployeeId = 1
            };

            Assert.True(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidBankType_ReturnFalse()
        {
            var criteria = new SearchBankAccountCriteria
            {
                BankTypeId = 0,
                EmployeeId = 1
            };

            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidEmployeeId_ReturnFalse()
        {
            var criteria = new SearchBankAccountCriteria
            {
                BankTypeId = 1,
                EmployeeId = 0
            };

            Assert.False(criteria.IsValid());
        }
    }
}
