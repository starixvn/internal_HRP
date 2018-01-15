namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class GetBankAccountByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "BankAccount_GetBankAccountById";

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}