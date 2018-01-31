namespace HRP.Criteria.Bank
{
    using Models.Bank;

    public class CreateBankAccountCriteria : BankAccountCriteria
    {
        public CreateBankAccountCriteria(IBankAccount bankAccount) : base(bankAccount)
        {
        }

        public override string GetSettingKey() => "BankAccount_CreateBankAccount";
    }
}
