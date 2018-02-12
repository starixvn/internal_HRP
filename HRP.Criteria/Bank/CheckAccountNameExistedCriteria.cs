namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class CheckAccountNameExistedCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public int EmployeeId { get; set; }

        public int BankType { get; set; }

        public override string GetSettingKey() => "BankAccount_CheckNameExisted";

        public override bool IsValid()
        {
            if (EmployeeId == 0)
            {
                return false;
            }

            if (BankType == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(AccountName) || string.IsNullOrWhiteSpace(AccountName))
            {
                return false;
            }

            return true;
        }
    }
}