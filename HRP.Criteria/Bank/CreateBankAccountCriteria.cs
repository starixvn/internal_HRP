namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class CreateBankAccountCriteria : BankAccountBaseCriteria, ICriteria
    {
        public override string GetSettingKey() => "BankAccount_CreateBankAccount";

        public override bool IsValid()
        {
            return GetIsValidBase();
        }
    }
}
