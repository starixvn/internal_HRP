namespace HRP.Criteria.Bank
{
    using Models.Bank;

    public class UpdateBankAccountCriteria : BankAccountCriteria
    {
        public UpdateBankAccountCriteria(IBankAccount bankAccount) : base(bankAccount)
        {
            Id = bankAccount.Id;
        }

        public override string GetSettingKey() => "BankAccount_Update";

        public override bool IsValid()
        {
            return Id > 0 && base.IsValid();
        }
    }
}