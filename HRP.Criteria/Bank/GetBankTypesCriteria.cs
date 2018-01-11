namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class GetBankTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "BankAccount_GetBankTypes";

        public override bool IsValid() => true;
    }
}