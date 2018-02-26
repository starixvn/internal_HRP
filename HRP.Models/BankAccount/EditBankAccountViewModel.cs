namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;

    public class EditBankAccountViewModel : BaseFormBankAccountViewModel
    {
        public EditBankAccountViewModel()
        {
            BankTypeResources = new List<ReferenceResourceModel>();
            BankAccountTypeResources = new List<ReferenceResourceModel>();
        }

        public bool IsSuccessful { get; set; }

        public int BankAccountId { get; set; }
    }
}