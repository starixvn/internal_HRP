namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;
    using HRP.Core.Model;

    public class EditBankAccountViewModel : BaseFormBankAccountViewModel
    {
        public EditBankAccountViewModel()
        {
            BankTypeResources = new List<ReferenceResourceModel>();
            BankAccountTypeResources = new List<ReferenceResourceModel>();
        }

        public int BankAccountId { get; set; }
    }
}