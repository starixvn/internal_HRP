namespace HRP.Models.Bank
{
    using System.Collections.Generic;

    public class EditBankAccountViewModel : CreateBankAccountViewModel
    {
        public EditBankAccountViewModel(
            IBankAccount bankAccount,
            IEnumerable<BankType> bankTypes,
            IEnumerable<BankAccountType> bankAccountTypes)
        {
            Initialize(bankAccount, bankTypes, bankAccountTypes);
        }

        public EditBankAccountViewModel()
        {
        }

        public void Initialize(
            IBankAccount bankAccount,
            IEnumerable<BankType> bankTypes,
            IEnumerable<BankAccountType> bankAccountTypes,
            IEnumerable<BankType> disabledBankTypes = null)
        {
            Initialize(bankTypes, bankAccountTypes, disabledBankTypes);
            BankAccount = bankAccount;
        }
    }
}