namespace HRP.Models.Bank
{
    using System.Collections.Generic;

    public class EditBankAccountViewModel : CreateBankAccountViewModel
    {
        public EditBankAccountViewModel(
            IBankAccount bankAccount,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            Initialize(bankAccount, bankTypes, bankAccountTypes);
        }

        public EditBankAccountViewModel()
        {
        }

        public void Initialize(
            IBankAccount bankAccount,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            Initialize(bankTypes, bankAccountTypes);
            BankAccount = bankAccount;
        }
    }
}