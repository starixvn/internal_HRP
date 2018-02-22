namespace HRP.Criteria.BankAccount
{
    public class UpdateBankAccountCriteria : BankAccountBaseCriteria
    {
        public override string GetSettingKey() => "BankAccount_Update";

        public override bool IsValid()
        {
            if (Id == 0)
            {
                return false;
            }

            return base.IsValid();
        }
    }
}