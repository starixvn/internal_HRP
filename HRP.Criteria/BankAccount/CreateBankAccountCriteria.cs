namespace HRP.Criteria.BankAccount
{
    public class CreateBankAccountCriteria : BankAccountBaseCriteria
    {
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "BankAccount_Create";

        public override bool IsValid()
        {
            if (CreatedBy == 0)
            {
                return false;
            }

            return base.IsValid();
        }
    }
}
