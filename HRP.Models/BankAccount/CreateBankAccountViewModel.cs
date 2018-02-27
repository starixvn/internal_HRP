namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;

    public class CreateBankAccountViewModel : BaseFormBankAccountViewModel
    {
        public CreateBankAccountViewModel()
        {
            BankTypeResources = new List<ReferenceResourceModel>();
            BankAccountTypeResources = new List<ReferenceResourceModel>();
            IsActive = true;
            IsSuccessful = false;

            // Default bank type: savings
            AccountTypeId = 1;
        }
    }
}