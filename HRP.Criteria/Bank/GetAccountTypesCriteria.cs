namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class GetAccountTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "BankAccount_GetAccountTypes";

        public override bool IsValid() => true;
    }
}