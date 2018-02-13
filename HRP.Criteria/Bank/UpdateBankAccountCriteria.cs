namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class UpdateBankAccountCriteria : BankAccountBaseCriteria, ICriteria
    {
        public override string GetSettingKey() => "BankAccount_Update";

        public override bool IsValid()
        {
            return GetIsValidBase();
        }
    }
}