namespace HRP.Models.Bank
{
    using System.Collections.Generic;

    public class EditBankAccountViewModel : CreateBankAccountViewModel
    {
        public EditBankAccountViewModel(
            string employeeName,
            IBankAccount bankAccount,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            Initialize(employeeName, bankAccount, bankTypes, bankAccountTypes);
        }

        public EditBankAccountViewModel()
        {
        }

        public void Initialize(
            string employeeName,
            IBankAccount bankAccount,
            IEnumerable<ReferenceResourceModel> bankTypes,
            IEnumerable<ReferenceResourceModel> bankAccountTypes)
        {
            Initialize(employeeName, bankTypes, bankAccountTypes);
            BankAccount = bankAccount;
        }
    }
}