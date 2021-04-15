namespace HRP.Models.BankAccount
{
    using System.Collections.Generic;
    using HRP.Core.Model;

    public class CreateBankAccountViewModel : BaseFormBankAccountViewModel
    {
        public CreateBankAccountViewModel()
        {
            BankTypeResources = new List<ReferenceResourceModel>();
            BankAccountTypeResources = new List<ReferenceResourceModel>();
            IsActive = true;
            IsSuccessful = false;
            AccountTypeId = (short)Core.Enumerations.BankAccountType.Savings;
        }
    }
}