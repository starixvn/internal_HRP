namespace HRP.Criteria.BankAccount
{
    using Fanex.Data.Repository;

    public class GetBankAccountByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "BankAccount_GetBankAccountById";

        public override bool IsValid() => Id > 0;
    }
}