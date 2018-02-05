namespace HRP.Models.Bank
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateBankAccountViewModel
    {
        public CreateBankAccountViewModel(
            IEnumerable<BankType> bankTypes,
            IEnumerable<BankAccountType> bankAccountTypes)
        {
            BankAccount = new BankAccount();
            Initialize(bankTypes, bankAccountTypes);
        }

        public CreateBankAccountViewModel()
        {
            BankAccount = new BankAccount();
        }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public SelectList BankTypes { get; set; }

        public SelectList BankAccountTypes { get; set; }

        public IBankAccount BankAccount { get; set; }

        public void Initialize(
            IEnumerable<BankType> bankTypes,
            IEnumerable<BankAccountType> bankAccountTypes,
            IList<BankType> disabledBankTypes = null)
        {
            BankTypes = disabledBankTypes != null
                ? new SelectList(
                    bankTypes.Except(disabledBankTypes, new RefResourceContructableComparer()),
                    nameof(BankType.Id),
                    nameof(BankType.Name))
                : new SelectList(
                    bankTypes,
                    nameof(BankType.Id),
                    nameof(BankType.Name));

            BankAccountTypes = new SelectList(
                bankAccountTypes,
                nameof(BankAccountType.Id),
                nameof(BankAccountType.Name));
        }
    }

    public enum BankAccountError
    {
        [Display(Name = "Account Name and Account Number existed. Please enter another value")]
        AccountExisted = 1
    }
}